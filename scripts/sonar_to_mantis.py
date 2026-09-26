#!/usr/bin/env python3
"""Sync SonarQube issues -> Mantis tickets, fully automatic (both directions).

- Creates one Mantis ticket per open SonarQube issue (deduplicated via a
  `[sonar:<project>:<key>]` marker in the ticket summary).
- Resolves Mantis tickets whose SonarQube issue is CLOSED/RESOLVED
  (fix verified by SonarQube), with a closing note.
- Never fails: network/auth problems print a warning and exit 0, so CI
  stays green when the ticket systems are unreachable.

Environment (all optional unless noted):
    SONAR_URL            e.g. http://192.168.178.128:9000 (or public URL)
    SONAR_TOKEN          SonarQube user token (required)
    SONAR_PROJECT        SonarQube project key, default: gregCore
    SONAR_BRANCH         branch to query (Jenkins BRANCH_NAME); ignored for PRs
    SONAR_PR             pull-request key (Jenkins CHANGE_ID); wins over branch
    MANTIS_URL           e.g. http://192.168.178.127
    MANTIS_TOKEN         Mantis API token (required)
    MANTIS_PROJECT_ID    numeric Mantis project id (required)
    MANTIS_CATEGORY_ID   numeric Mantis category id (required)
    MANTIS_RESOLVED_STATUS_ID  default: 80 (Mantis 'resolved')
    MAX_TICKETS_PER_RUN  cap for created tickets per run, default: 50
                         (the sync converges over consecutive builds)
    DRY_RUN=1            log actions without calling Mantis (read-only)

Only the Python standard library is used (no pip needed on agents).
"""

import json
import os
import re
import sys
import urllib.parse
import urllib.request
import urllib.error

MARKER_RE = re.compile(r"\[sonar:[^:\]]+:([A-Za-z0-9_\-]+)\]")


def env(name, default=None, required=False):
    value = os.environ.get(name, default)
    if required and not value:
        print(f"[sonar-mantis] missing required env {name} - skipping sync.")
        raise SystemExit(0)
    return value


def http(method, url, token=None, bearer=False, payload=None, timeout=30):
    data = None
    scheme = urllib.parse.urlsplit(url).scheme.lower()
    if scheme not in ("http", "https"):
        return -1, {"_error": f"refused non-http(s) URL scheme: {scheme!r}"}
    headers = {"Content-Type": "application/json", "Accept": "application/json"}
    if token:
        headers["Authorization"] = (f"Bearer {token}" if bearer else token)
    if payload is not None:
        data = json.dumps(payload).encode("utf-8")
    req = urllib.request.Request(url, data=data, headers=headers, method=method)
    try:
        with urllib.request.urlopen(req, timeout=timeout) as resp:
            body = resp.read().decode("utf-8", "replace")
            return resp.status, (json.loads(body) if body.strip() else {})
    except urllib.error.HTTPError as ex:
        detail = ex.read().decode("utf-8", "replace")[:300]
        return ex.code, {"_error": detail}
    except Exception as ex:  # network down, DNS, timeout, ...
        return -1, {"_error": f"{type(ex).__name__}: {ex}"}


def sonar_get(base, token, path, params):
    url = f"{base.rstrip('/')}{path}?{urllib.parse.urlencode(params)}"
    status, data = http("GET", url, token=token, bearer=True)
    if status != 200:
        print(f"[sonar-mantis] SonarQube {path} -> HTTP {status}: {data}")
        raise SystemExit(0)
    return data


def fetch_sonar_open_issues(base, token, project, branch, pr):
    """All OPEN/CONFIRMED/REOPENED SonarQube issues for project+branch/PR."""
    issues, page, page_size = [], 1, 500
    while True:
        params = {
            "componentKeys": project,
            "statuses": "OPEN,CONFIRMED,REOPENED",
            "ps": page_size,
            "p": page,
        }
        if pr:
            params["pullRequest"] = pr
        elif branch:
            params["branch"] = branch
        data = sonar_get(base, token, "/api/issues/search", params)
        issues.extend(data.get("issues", []))
        paging = data.get("paging", {})
        total = paging.get("total", len(issues))
        if page * page_size >= total or not data.get("issues"):
            break
        page += 1
    return issues


def sonar_issue_states(base, token, keys):
    """Map SonarQube issue key -> status (batch, 100 per call)."""
    states = {}
    keys = list(keys)
    for i in range(0, len(keys), 100):
        chunk = keys[i:i + 100]
        data = sonar_get(base, token, "/api/issues/search",
                         {"issues": ",".join(chunk), "ps": 100})
        for issue in data.get("issues", []):
            states[issue["key"]] = issue.get("status", "OPEN")
    return states


def mantis_get(base, token, path, params=None):
    url = f"{base.rstrip('/')}{path}"
    if params:
        url += "?" + urllib.parse.urlencode(params)
    status, data = http("GET", url, token=token)
    if status != 200:
        print(f"[sonar-mantis] Mantis GET {path} -> HTTP {status}: {data}")
        raise SystemExit(0)
    return data


def fetch_mantis_open_sonar_tickets(base, token, project_id):
    """Open Mantis tickets carrying our marker: {sonar_key: ticket_id}."""
    tickets, page = {}, 1
    while True:
        data = mantis_get(base, token, "/api/rest/issues/",
                          {"project_id": project_id,
                           "page_size": 100, "page": page})
        batch = data.get("issues", [])
        if not batch:
            break
        for issue in batch:
            status_id = (issue.get("status") or {}).get("id", 0)
            if status_id >= 80:  # resolved/closed already
                continue
            match = MARKER_RE.search(issue.get("summary", ""))
            if match:
                tickets[match.group(1)] = issue["id"]
        if len(batch) < 100:
            break
        page += 1
    return tickets


def short_path(component, project):
    prefix = f"{project}:"
    path = component[len(prefix):] if component.startswith(prefix) else component
    return path.replace("\\", "/")


def sonar_link(base, project, key, pr):
    params = {"id": project, "issues": key, "open": key}
    if pr:
        params["pullRequest"] = pr
    return f"{base.rstrip('/')}/project/issues?{urllib.parse.urlencode(params)}"


def load_config():
    return {
        "dry": os.environ.get("DRY_RUN", "") == "1",
        "sonar_url": env("SONAR_URL", required=True),
        "sonar_token": env("SONAR_TOKEN", required=True),
        "project": env("SONAR_PROJECT", "gregCore"),
        "pr": os.environ.get("SONAR_PR", "") or None,
        "branch": os.environ.get("SONAR_BRANCH", "") or None,
        "mantis_url": env("MANTIS_URL", required=True),
        "mantis_token": env("MANTIS_TOKEN", required=True),
        "mantis_project": env("MANTIS_PROJECT_ID", required=True),
        "mantis_category": env("MANTIS_CATEGORY_ID", required=True),
        "resolved_status": int(env("MANTIS_RESOLVED_STATUS_ID", "80")),
        "max_new": int(env("MAX_TICKETS_PER_RUN", "50")),
    }


def build_ticket_body(cfg, issue):
    project, key = cfg["project"], issue.get("key", "")
    rule = issue.get("rule", "?")
    severity = issue.get("severity", "?")
    kind = issue.get("type", "?")
    comp = short_path(issue.get("component", "?"), project)
    line = issue.get("line", "?")
    message = (issue.get("message", "") or "").strip()
    marker = f"[sonar:{project}:{key}]"
    summary = f"{marker} [{severity}/{kind}] {rule}: {comp}:{line}"
    summary = summary[:125] + "..." if len(summary) > 128 else summary
    return summary, {
        "summary": summary,
        "description": (
            f"SonarQube issue {key} ({severity}/{kind}, rule {rule})\n"
            f"File: {comp}, line: {line}\n\n{message}\n\n"
            f"Open in SonarQube: "
            f"{sonar_link(cfg['sonar_url'], project, key, cfg['pr'])}"
        ),
        "project": {"id": int(cfg["mantis_project"])},
        "category": {"id": int(cfg["mantis_category"])},
    }


def create_tickets(cfg, issues, existing):
    """Create tickets for untracked issues (capped per run)."""
    created = 0
    for issue in issues:
        key = issue.get("key", "")
        if not key or key in existing:
            continue
        if created >= cfg["max_new"]:
            print(f"[sonar-mantis] cap reached ({cfg['max_new']}/run), "
                  f"remainder converges on later builds.")
            break
        summary, body = build_ticket_body(cfg, issue)
        if cfg["dry"]:
            print(f"[sonar-mantis] DRY-RUN create: {summary[:100]}")
        else:
            status, data = http(
                "POST", f"{cfg['mantis_url'].rstrip('/')}/api/rest/issues/",
                token=cfg["mantis_token"], payload=body)
            if status in (200, 201):
                print(f"[sonar-mantis] created Mantis "
                      f"#{(data.get('issue') or {}).get('id', '?')} for {key}")
            else:
                print(f"[sonar-mantis] create failed for {key}: "
                      f"HTTP {status}: {data}")
                continue
        existing[key] = -1
        created += 1
    return created


def resolve_tickets(cfg, existing):
    """Resolve tickets whose SonarQube issue is fixed."""
    tracked = {k: v for k, v in existing.items() if v != -1}
    if not tracked:
        return 0
    states = sonar_issue_states(cfg["sonar_url"], cfg["sonar_token"],
                                list(tracked))
    resolved = 0
    for key, ticket_id in tracked.items():
        state = states.get(key)
        if state not in ("CLOSED", "RESOLVED"):
            continue
        note = (f"SonarQube reports this issue as {state} - "
                f"auto-resolving (verified by analysis).")
        if cfg["dry"]:
            print(f"[sonar-mantis] DRY-RUN resolve #{ticket_id} "
                  f"({key} is {state})")
            continue
        http("POST",
             f"{cfg['mantis_url'].rstrip('/')}/api/rest/issues/"
             f"{ticket_id}/notes",
             token=cfg["mantis_token"], payload={"text": note})
        status, data = http(
            "PUT",
            f"{cfg['mantis_url'].rstrip('/')}/api/rest/issues/{ticket_id}",
            token=cfg["mantis_token"],
            payload={"status": {"id": cfg["resolved_status"]}})
        if status == 200:
            print(f"[sonar-mantis] resolved Mantis "
                  f"#{ticket_id} ({key} is {state})")
            resolved += 1
        else:
            print(f"[sonar-mantis] resolve failed for "
                  f"#{ticket_id}: HTTP {status}: {data}")
    return resolved


def main():
    cfg = load_config()
    try:
        issues = fetch_sonar_open_issues(cfg["sonar_url"], cfg["sonar_token"],
                                         cfg["project"], cfg["branch"],
                                         cfg["pr"])
        existing = fetch_mantis_open_sonar_tickets(cfg["mantis_url"],
                                                   cfg["mantis_token"],
                                                   cfg["mantis_project"])
    except SystemExit:
        raise
    except Exception as ex:
        print(f"[sonar-mantis] sync aborted (non-blocking): {ex}")
        return 0

    print(f"[sonar-mantis] SonarQube open issues: {len(issues)}, "
          f"tracked Mantis tickets: {len(existing)}"
          + (" (DRY RUN)" if cfg["dry"] else ""))

    created = create_tickets(cfg, issues, existing)
    resolved = resolve_tickets(cfg, existing)
    print(f"[sonar-mantis] done: {created} created, {resolved} resolved.")
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except SystemExit as ex:
        # env() exits 0 on missing config (deliberate skip, stays green)
        sys.exit(ex.code if isinstance(ex.code, int) else 0)
    except Exception as ex:
        print(f"[sonar-mantis] unexpected error (non-blocking): {ex}")
        sys.exit(0)
