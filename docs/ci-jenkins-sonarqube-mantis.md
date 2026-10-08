# CI: Jenkins + SonarQube + Mantis

End-to-end pipeline for gregCore (pilot, later template for `gregMod.*`).
Default addresses (Marvin's LAN, override in `Jenkinsfile` site config):

| Service   | Public (Cloudflare)                | LAN (Server-zu-Server)          |
|-----------|--------------------------------------|---------------------------------|
| Jenkins   | https://build.gregframework.eu/      | http://192.168.178.129:8080/    |
| SonarQube | https://check.gregframework.eu/      | http://192.168.178.128:9000/    |
| Mantis    | https://bugs.gregframework.eu/       | http://192.168.178.127/         |

Interne Calls (Jenkins→SonarQube, Jenkins→Mantis, SonarQube-Webhook)
laufen über LAN; Browser und GitHub-Webhooks nutzen die Public-URLs.

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
   step `waitForQualityGate abortPipeline: true` enforces it. For fast
   feedback (instead of polling) add SonarQube → Administration →
   Configuration → Webhooks → `http://192.168.178.129:8080/sonarqube-webhook/`
   (LAN is fine — server to server).
5. The .NET scanner (`dotnet-sonarscanner` global tool) is installed by the
   pipeline itself; coverage comes from the already-referenced
   `coverlet.collector/msbuild` packages (OpenCover format).

## 4. GitHub wiring (Multibranch + checks)

1. Jenkins → New Item → **Multibranch Pipeline** → Branch Sources → GitHub:
   repository `mleem97/gregCore`, Behaviours: *Discover branches* + *Discover
   pull requests from origin*. The `Jenkinsfile` at repo root is picked up
   automatically.
2. **Webhook (done):** GitHub → Settings → Webhooks →
   `https://build.gregframework.eu/github-webhook/` (push + pull_request,
   JSON) is registered and active. Jenkins braucht dafür das
   *GitHub Branch Source*-Plugin; der `/github-webhook/`-Endpoint braucht
   kein CSRF-Crumb. Payload-Zustellung prüfen: GitHub → Webhook →
   *Recent Deliveries*.
3. The pipeline publishes checks named **`tests`**, **`build-linux`**,
   **`docs`** via the Checks API — exactly the names branch protection
   should require (see section 6). PR builds additionally get the standard
   Branch-Source statuses for free.
3. The pipeline publishes checks named **`tests`**, **`build-linux`**,
   **`docs`** via the Checks API — exactly the names branch protection
   should require (see section 6). PR builds additionally get the standard
   Branch-Source statuses for free.

## 5. Mantis wiring

1. Jenkins → Manage → System → **Jenkins URL** =
   `https://build.gregframework.eu/` (sonst zeigen Check-Run-Links und
   Mantis-Tickets auf die LAN-Adresse).
2. Mantis `config_inc.php` (public Betrieb hinter Cloudflare):
   `$g_path = 'https://bugs.gregframework.eu/';` und `$g_allow_signup =
   OFF;` (HTTPS terminiert an der Cloudflare Edge; Jenkins→Mantis läuft
   weiter über die LAN-URL im `Jenkinsfile`).
3. **REST-API-Routing (Pflicht für den Jenkins-Reporter):**
   `https://bugs.gregframework.eu/api/rest/` antwortet aktuell mit der
   Mantis-404-Seite — der Webserver reicht `/api/rest/*` nicht an
   `index.php` weiter. Nginx-Fix (sinngemäß auch für andere Server):
   ```nginx
   location /api/rest/ {
       try_files $uri $uri/ /api/rest/index.php?$query_string;
   }
   ```
   Danach antwortet `GET /api/rest/` mit JSON statt HTML.
4. Mantis → My Account → **API Tokens → Create** → Jenkins **Secret text**
   credential ID `mantis-api-token`.
5. Find numeric IDs: `GET /api/rest/projects/` (Authorization: token) lists
   projects; categories via `GET /api/rest/projects/{id}`. Put them in the
   `Jenkinsfile` site config (`MANTIS_PROJECT_ID`, `MANTIS_CATEGORY_ID` —
   defaults are `1`/`1`, adjust to your instance).
6. Install the **Mantis Source Control Integration** plugin and point it at
   the GitHub repos. Team convention from then on: commit messages carry
   `Fixes #<issue>` / `Related to #<issue>` (Conventional Commits stay as
   prefix, e.g. `fix: crash on empty save (Fixes #123)`).
7. Red builds file an issue `[CI] <job> #<n> failed (<branch>)` with job
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

## 8. SonarQube → Mantis issue sync (fully automatic)

Every pipeline run executes `scripts/sonar_to_mantis.py` (stdlib-only
Python, no pip needed) right after the SonarQube analysis:

- **Create:** one Mantis ticket per open SonarQube issue, summary
  `[sonar:<project>:<key>] [SEVERITY/TYPE] rule: file:line`, description
  with full message + deep link to the issue
  (`https://check.gregframework.eu/project/issues?...`).
- **Dedup:** the `[sonar:…]` marker is searched in open project tickets —
  no duplicates, ever.
- **Resolve-back:** tickets whose SonarQube issue is CLOSED/RESOLVED get a
  closing note and are set to resolved (status id configurable via
  `MANTIS_RESOLVED_STATUS_ID`, default `80`).
- **Throttling:** `MAX_TICKETS_PER_RUN` (default `50`) caps creations per
  run — a large backlog converges over consecutive builds instead of
  flooding Mantis.
- **Never red:** missing tokens, unreachable hosts, API errors → warning
  in the log, exit 0. `DRY_RUN=1` logs actions without touching Mantis.
- **PR decoration:** on pull-request builds the scanner gets
  `sonar.pullrequest.{key,branch,base}` automatically, so SonarQube
  annotates the PR *and* the sync queries PR-scoped issues
  (`SONAR_PR` wins over `SONAR_BRANCH`).

Required: the same `sonar-token` / `mantis-api-token` credentials plus
`MANTIS_PROJECT_ID` / `MANTIS_CATEGORY_ID` in the `Jenkinsfile` site
config (the sync reuses them, no extra setup).

## Troubleshooting

| Symptom | Cause → fix |
|---|---|
| `MISSING/EMPTY: references/*.dll` | Agent game refs missing → section 2 |
| `sonar-token` not found | Credential ID mismatch → section 3.2 |
| `waitForQualityGate` timeout | SonarQube webhook to Jenkins missing: SonarQube → Administration → Configuration → Webhooks → `http://192.168.178.129:8080/sonarqube-webhook/` |
| No PR builds trigger | LAN webhook unreachable → polling/relay per section 4.2 |
| `mstest publisher unavailable` | MSTest plugin missing → install (section 1); TRX is still archived |
| Mantis `401/403` on failure post | API token invalid or REST disabled → section 5.1 |
