# IPC Bridge — alles anbinden (localhost)

`GregIpcBridge` (`src/gregCore.Bridge/Ipc/`) ist der universelle Adapter:
Jedes externe Werkzeug (MCP-Server, Skripte, Agenten, `greg-repo-manager`-Game-Backend,
`curl`) spricht über TCP + NDJSON mit dem laufenden Spiel — ohne Unity-Editor,
ohne neue Game-Logik (nur bestehende `GregAPI`-Flächen).

## Start

Opt-in, Standard **aus**. Umgebungsvariablen vor dem Spielstart:

```sh
GREG_IPC_PORT=27960        # beliebig frei; startet die Bridge
GREG_IPC_TOKEN=geheim      # optional; wird dann pro Request verlangt
```

Gebunden wird ausschließlich `127.0.0.1`. Jede Anfrage läuft über den
Main-Thread-Dispatcher (max. 5 s), alle Handler sind try-caught mit Safe Defaults.

## Protokoll (eine JSON-Zeile pro Request)

`{"id":1,"method":"ping","params":{}}` →
`{"ok":true,"id":1,"result":"pong"}` oder `{"ok":false,"id":1,"error":"..."}`.

| Methode | Params | Ergebnis |
|---|---|---|
| `ping` | — | `"pong"` |
| `api.version` | — | Core-Version |
| `mods.list` | — | `[{id,name,version}]` aus `GregModRegistry` |
| `hook.fire` | `{"hook": name}` | feuert Hook, `true` |
| `ui.toast` | `{"text","duration?"}` | Notification, `true` |
| `ui.notify` | `{"text","duration?"}` | Notification, `true` |
| `scripts.reload` | — | C#-HotReload, `true`/`false` |
| `log.tail` | `{"n?"}` | letzte ≤200 DevConsole-Zeilen |

Mit Token: `"token"` pro Request mitsenden.

## Beispiel

```sh
printf '%s\n' '{"id":1,"method":"mods.list"}' > /dev/tcp/127.0.0.1/27960
```

## Erweitern

Neue Methode = ein `case` in `GregIpcBridge.Handle` + Zeile in dieser Tabelle
+ Test in `tests/Bridge/GregIpcProtocolTests.cs`. Kein Reflection im Protokoll
(IL2CPP-sicher); nur bestehende Facades aufrufen.
