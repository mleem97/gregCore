# CI: Jenkins + SonarQube + Mantis

End-to-end pipeline for gregCore (pilot, later template for `gregMod.*`).
Default addresses (Marvin's LAN, override in `Jenkinsfile` site config):

| Service   | URL                           |
|-----------|-------------------------------|
| Jenkins   | http://192.168.178.129:8080/  |
| SonarQube | http://192.168.178.128:9000/  |
| Mantis    | http://192.168.178.127/       |

Flow: GitHub push/PR → Jenkins Multibranch → build + test + SonarQube +
Quality Gate → GitHub checks (`tests`, `build-linux`, `docs`) → Mantis
ticket on red builds. Commit messages referencing `Fixes #123` link
Mantis issues automatically (Mantis Source plugin).

## 1. Jenkins plugins

Manage Jenkins → Plugins → Available, install:

- **Pipeline** (`workflow-aggregator`), **Git**, **GitHub Branch Source**
  (`github-branch-source`), **GitHub Checks API** (for the `publishChecks`
  step), **SonarQube Scanner** (`sonar`), **MSTest** (TRX reports)

Restart if prompted.

## 2. Build agent (`greg-dotnet` label)

The `Jenkinsfile` targets `agent { label 'greg-dotnet' }` (currently on the
built-in node — move to a dedicated agent/VM later). Required tooling:

- Git, .NET SDK 8 (`dotnet --list-sdks`), JDK 17 (`java -version`),
  Node 22 LTS (optional, only for the `docs` stage)
- **Game references (critical):** `references/*.dll` are symlinks into a
  Steam install and do not exist on a fresh agent. The pipeline fails fast
  with a pointer here if they are missing. One-time setup, pick one:
  - (a) Install Data Center via Steam/SteamCMD on the agent at the same
        path layout and run the repo's assembly sync, or
  - (b) Copy `Assembly-CSharp.dll`, `MelonLoader.dll`, `0Harmony.dll`,
        `Il2CppInterop.*.dll`, `Il2CppSystem*.dll` (+ game-version note)
        from a local install into the workspace `references/` before build
        (private files — never commit them; `.gitignore` already excludes
        `references/*.dll`, only `references/.gitkeep` is tracked).

## 3. SonarQube

1. Log in at `http://192.168.178.128:9000/` → **Projects → Create
   Project → Manually**, project key `gregCore`.
2. **My Account → Security → Generate Token** (User token) → in Jenkins
   as **Secret text** credential with ID `sonar-token`.
3. Jenkins → Manage → System → **SonarQube servers** → Add: name exactly
   `sonarqube`, server URL `http://192.168.178.128:9000/`, token credential
   `sonar-token` (or leave token empty there — the `Jenkinsfile` passes it
   explicitly via `withCredentials`, which also works with newer plugin
   versions).
4. **Quality Gate** (SonarQube → Quality Gates): suggested starter —
   *New Code:* 0 Blocker/Critical issues, tests must exist. The pipeline
   step `waitForQualityGate abortPipeline: true` enforces it.
5. The .NET scanner (`dotnet-sonarscanner` global tool) is installed by the
   pipeline itself; coverage comes from the already-referenced
   `coverlet.collector/msbuild` packages (OpenCover format).

## 4. GitHub wiring (Multibranch + checks)

1. Jenkins → New Item → **Multibranch Pipeline** → Branch Sources → GitHub:
   repository `mleem97/gregCore`, Behaviours: *Discover branches* + *Discover
   pull requests from origin*. The `Jenkinsfile` at repo root is picked up
   automatically.
2. **Webhook (empfohlen): cloudflared-Tunnel (Zero Trust).** Jenkins und
   Mantis hängen im LAN und bekommen je einen öffentlichen Hostnamen,
   SonarQube bleibt bewusst LAN-only (Jenkins→SonarQube läuft
   Server-zu-Server; SonarQubes PR-Decoration nach GitHub ist outbound
   und braucht kein Inbound):
   ```bash
   # einmalig (Account mit Domain in Cloudflare Zero Trust):
   cloudflared tunnel login
   cloudflared tunnel create homelab
   cloudflared tunnel route dns homelab jenkins.example.com
   cloudflared tunnel route dns homelab mantis.example.com
   # ~/.cloudflared/config.yml:
   # tunnel: homelab
   # ingress:
   #   - hostname: jenkins.example.com
   #     service: http://192.168.178.129:8080
   #   - hostname: mantis.example.com
   #     service: http://192.168.178.127:80
   #   - service: http_status:404
   cloudflared service install && systemctl enable --now cloudflared
   ```
   Danach: Jenkins → Manage → System → **Jenkins URL** =
   `https://jenkins.example.com` (sonst zeigen Status-Links und Mantis-
   Tickets auf die LAN-Adresse). GitHub-Webhook =
   `https://jenkins.example.com/github-webhook/` (Events: push + pull
   request; der `/github-webhook/`-Endpoint braucht kein CSRF-Crumb).
   Mantis: `config_inc.php` → `$g_path =
   'https://mantis.example.com/';` und `$g_allow_signup = OFF;`
   (Signup aus, HTTPS kommt von Cloudflare Edge, Jenkins→Mantis läuft
   weiter über die LAN-URL im `Jenkinsfile`).
3. **Fallbacks ohne Tunnel:** (a) Multibranch → *Scan → Periodically if
   not otherwise run* (alle 15 Min), (b) `smee.io`-Relay auf
   `http://192.168.178.129:8080/github-webhook/`.
3. The pipeline publishes checks named **`tests`**, **`build-linux`**,
   **`docs`** via the Checks API — exactly the names branch protection
   should require (see section 6). PR builds additionally get the standard
   Branch-Source statuses for free.

## 5. Mantis wiring

1. Mantis → My Account → **API Tokens → Create** (needs a Mantis ≥ 2.x with
   REST enabled — default on) → Jenkins **Secret text** credential ID
   `mantis-api-token`.
2. Find numeric IDs: `GET /api/rest/projects/` (Authorization: token) lists
   projects; categories via `GET /api/rest/projects/{id}`. Put them in the
   `Jenkinsfile` site config (`MANTIS_PROJECT_ID`, `MANTIS_CATEGORY_ID` —
   defaults are `1`/`1`, adjust to your instance).
3. Install the **Mantis Source Control Integration** plugin and point it at
   the GitHub repos. Team convention from then on: commit messages carry
   `Fixes #<issue>` / `Related to #<issue>` (Conventional Commits stay as
   prefix, e.g. `fix: crash on empty save (Fixes #123)`).
4. Red builds file an issue `[CI] <job> #<n> failed (<branch>)` with job
   URL, branch and commit (guarded with `|| true` — reporting never fails
   the build).

## 6. Branch protection mapping (dev)

Replace the dead required contexts with ones Jenkins actually reports:

- Require: `tests`, `build-linux`, `docs` (from this pipeline)
- Keep: 1 approving review, linear history
- Drop (until real workflows exist): `contracts`, `policy/branch-flow`,
  `build-windows`
- Codacy check: keep or drop once the SonarQube gate is trusted —
  running both gates stale-mate merges (two sources of truth).

## 7. Rollout to mod repos

`gregMod.*` repos share one shape (`build.sh`, `src/`, `references/`).
Copy this `Jenkinsfile`, changing only `SONAR_PROJECT_KEY` (one SonarQube
project per mod, e.g. `gregMod-Trainer`) and the build command
(`./build.sh Trainer` instead of `dotnet build`). Later: extract a Shared
Library (`vars/gregModPipeline.groovy`) so all mods share one definition.

## Troubleshooting

| Symptom | Cause → fix |
|---|---|
| `MISSING/EMPTY: references/*.dll` | Agent game refs missing → section 2 |
| `sonar-token` not found | Credential ID mismatch → section 3.2 |
| `waitForQualityGate` timeout | SonarQube webhook to Jenkins missing: SonarQube → Administration → Configuration → Webhooks → `http://192.168.178.129:8080/sonarqube-webhook/` |
| No PR builds trigger | LAN webhook unreachable → polling/relay per section 4.2 |
| `mstest publisher unavailable` | MSTest plugin missing → install (section 1); TRX is still archived |
| Mantis `401/403` on failure post | API token invalid or REST disabled → section 5.1 |
