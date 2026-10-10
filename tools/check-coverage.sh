#!/usr/bin/env bash
# Coverage-Gate fuer gregCore.
#
#   ./check-coverage.sh <coverage.cobertura.xml>
#
# Regeln:
#   1. Reine, spiel-freie Logik (NEW_CODE_FILES) muss 100 % Zeilen UND
#      Branches halten - das ist die 100 %-Idealwert-Zone.
#   2. Gesamt-Coverage darf nicht unter die Baseline in
#      coverage-baseline.txt fallen (Ratchet, kein stiller Verfall).
#   3. Das 80 %-Pass-Ziel fuer das GESAMT-Repo ist dokumentiert, aber
#      bewusst kein harter Fail: spiel-gebundener Code (Il2Cpp/Unity)
#      ist per Unit-Test nicht erreichbar - dafuer braucht es
#      Game-in-the-Loop-Tests unter Proton. Der Abstand wird reported.
#
# Erzeugt mit: dotnet test /p:CollectCoverage=true
#   /p:CoverletOutputFormat=cobertura /p:CoverletOutput=<dir>/
set -u

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
BASELINE_FILE="$ROOT/coverage-baseline.txt"
NEW_CODE_FILES=(
  "PublicApi/Modules/DemandPlanner.cs"
  "Infrastructure/Performance/RamAlertEvaluator.cs"
)
TARGET_TOTAL="0.80"

if [[ $# -lt 1 ]]; then
  echo "Usage: $0 <coverage.cobertura.xml>" >&2
  exit 2
fi
XML="$1"
[[ -f "$XML" ]] || { echo "GATE-FAIL: Datei nicht gefunden: $XML" >&2; exit 2; }
[[ -f "$BASELINE_FILE" ]] || { echo "GATE-FAIL: Baseline fehlt: $BASELINE_FILE" >&2; exit 2; }

python3 - "$XML" "$BASELINE_FILE" "$TARGET_TOTAL" "${NEW_CODE_FILES[@]}" <<'EOF'
import sys, xml.etree.ElementTree as ET

xml_path, baseline_path, target_total = sys.argv[1], sys.argv[2], float(sys.argv[3])
new_files = sys.argv[4:]

with open(baseline_path) as f:
    baseline = float(f.read().strip().split()[0])

root = ET.parse(xml_path).getroot()
total = round(float(root.get("line-rate", 0)), 4)

by_file = {}
for c in root.findall("packages/package/classes/class"):
    fn = c.get("filename", "")
    key = next((n for n in new_files if fn.endswith(n)), None)
    if key is None:
        continue
    lr, br = round(float(c.get("line-rate", 0)), 4), round(float(c.get("branch-rate", 0)), 4)
    prev = by_file.get(key, (1.0, 1.0))
    by_file[key] = (min(prev[0], lr), min(prev[1], br))

failures = []
for n in new_files:
    if n not in by_file:
        failures.append(f"NEU-CODE fehlt im Report: {n}")
        continue
    lr, br = by_file[n]
    print(f"NEU-CODE {n}: line={lr:.4f} branch={br:.4f} (Pflicht: 1.0000)")
    if lr < 1.0 or br < 1.0:
        failures.append(f"NEU-CODE unter 100 %: {n} (line={lr:.4f}, branch={br:.4f})")

print(f"GESAMT line={total:.4f} baseline={baseline:.4f} ziel={target_total:.2f}")
if total < baseline:
    failures.append(f"GESAMT unter Baseline: {total:.4f} < {baseline:.4f}")
if total < target_total:
    print(f"HINWEIS: Gesamtziel {target_total:.0%} noch nicht erreicht "
          f"({total:.2%}) - braucht Game-in-the-Loop-Tests oder "
          f"systematische Excludes spiel-gebundenen Codes. Kein Fail.")

if failures:
    print("GATE-FAIL:")
    for f in failures:
        print("  - " + f)
    sys.exit(1)
print("GATE-PASS")
EOF
