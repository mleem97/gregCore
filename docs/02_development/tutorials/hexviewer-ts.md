---
id: hexviewer-ts
title: HexViewer Mod Tutorial (TypeScript)
sidebar_label: HexViewer in TypeScript
slug: /development/tutorials/hexviewer-ts
description: Technical writing tutorial for the HexViewer TypeScript HUD layer with gregCore.UI and event-driven binding.
---

This page implements the **TypeScript UI layer** of HexViewer.

Goal: render a JADE-like panel at top center and bind it to enriched target events from gregCore.

> Layer ownership: this page belongs to **Language Bridge UI layer** and consumes `greg.*` events only.

## Introduction

HexViewer HUD receives `greg.HEXVIEWER.TargetEnriched` payload and displays:

- object type (`Rack`, `Cable`, `CableSpinner`)
- object id (hex)
- color
- owner/customer

## Project structure (TS segment)

```text
gregMod.HexViewer/ts/
├─ package.json
├─ tsconfig.json
├─ src/
│  ├─ hud.ts
│  ├─ events.ts
│  └─ bootstrap.ts
└─ dist/
   └─ hexviewer_hud.js
```

## UI implementation using gregCore.UI

`ts/src/hud.ts`:

```ts
type HexViewerPayload = {
  entityId: number;
  entityType: string;
  colorHex: string;
  ownerName: string;
  distanceMeters?: number;
};

export function mountHexViewerHud(gregCore: {
  UI: {
    createPanel: (options: {
      id: string;
      anchor: 'TopCenter' | string;
      width: number;
      height: number;
      title: string;
    }) => { setText: (value: string) => void };
  };
  Events: {
    on: (eventName: string, handler: (payload: HexViewerPayload) => void) => void;
  };
}): void {
  // gregCore-specific: create HUD panel anchored to top center
  const panel = gregCore.UI.createPanel({
    id: 'hexviewer.panel',
    anchor: 'TopCenter',
    width: 580,
    height: 96,
    title: 'HexViewer',
  });

  // gregCore-specific: subscribe to event bus for cross-language payloads
  gregCore.Events.on('greg.HEXVIEWER.TargetEnriched', (payload) => {
    const hexId = `0x${payload.entityId.toString(16).toUpperCase().padStart(8, '0')}`;
    panel.setText(
      `${payload.entityType} | ${hexId} | ${payload.colorHex} | ${payload.ownerName}`,
    );
  });
}
```

`ts/src/bootstrap.ts`:

```ts
import { mountHexViewerHud } from './hud';

export function registerHexViewerUi(gregCore: unknown): void {
  // gregCore-specific: bridge-level registration entrypoint
  // If register function name differs in your branch, keep behavior and naming style.
  mountHexViewerHud(gregCore as never);
}
```

## Build pipeline

`ts/package.json`:

```json
{
  "name": "hexviewer-ts",
  "private": true,
  "scripts": {
    "build": "tsup src/bootstrap.ts --format iife --out-dir dist --global-name HexViewerUI"
  },
  "devDependencies": {
    "tsup": "^8.0.0",
    "typescript": "^5.0.0"
  }
}
```

Build command:

```powershell
pnpm --dir .\ts install
pnpm --dir .\ts run build
```

## Integration contract

TS does not query game objects directly. It only listens to `greg.HEXVIEWER.TargetEnriched` emitted by C# + Rust pipeline.

If `gregCore.UI.createPanel` is unavailable in your branch, document a placeholder adapter in the same namespace:

- `gregCore.UI.createPanel` (preferred)
- `gregCore.UI.renderPanel` (alternate naming convention)

## Deploy and test

Copy output bundle to your bridge plugin module folder.

Validation checklist:

1. UI layer initializes without errors.
2. Panel is visible at top center.
3. Hovering rack/cable updates text in real time.

## Troubleshooting

- Panel missing: verify bridge bootstrap registration.
- No updates: check exact event name `greg.HEXVIEWER.TargetEnriched`.
- Encoding issues: ensure UTF-8 output and no transpile runtime mismatch.

## 1) Prerequisites

- Node.js + package manager (`pnpm` or `npm`)
- A JS/TS bridge plugin in your gregCore stack
- Event transport from gregCore to TS (WebSocket, IPC, or bridge API)

## 2) Initialize project

```powershell
mkdir hexviewer-ts
cd hexviewer-ts
pnpm init
pnpm add -D typescript tsup
```

Create `tsconfig.json`:

```json
{
  "compilerOptions": {
    "target": "ES2020",
    "module": "ESNext",
    "strict": true,
    "moduleResolution": "Bundler",
    "outDir": "dist"
  },
  "include": ["src"]
}
```

## 3) Implement HexViewer logic

`src/index.ts`:

```ts
type HookPayload = {
  method?: string;
  instance_handle?: number;
};

const HOOKS = [
  'greg.UI.OnHoverOver',
  'greg.SYSTEM.OnHoverOver',
  'greg.PLAYER.InteractOnClick',
];

function toHex(value: number | undefined): string {
  return `0x${(value ?? 0).toString(16).toUpperCase().padStart(8, '0')}`;
}

function onHook(hookName: string, payload: HookPayload): void {
  const method = payload.method ?? 'unknown';
  const hex = toHex(payload.instance_handle);
  console.log(`[HexViewer/TS] hook=${hookName} method=${method} hex=${hex}`);
}

export function registerHexViewer(gregBus: {
  on: (hook: string, cb: (payload: HookPayload) => void) => void;
}): void {
  for (const hook of HOOKS) {
    gregBus.on(hook, (payload) => onHook(hook, payload));
  }
}
```

## 4) Build

```powershell
pnpm tsup src/index.ts --format cjs --out-dir dist
```

## 5) Deploy to bridge

Copy `dist/index.js` to your bridge/plugin module location (depends on plugin setup). Then restart game + bridge.

## 6) Test in game

1. Launch game and bridge process/plugin.
2. Trigger hover/click events.
3. Verify TS console/bridge logs:

```text
[HexViewer/TS] hook=greg.UI.OnHoverOver method=OnHoverOver hex=0x00123FAB
```

## 7) Troubleshooting

- No TS output: bridge did not load bundle path.
- No hooks: event subscription API mismatch between bridge and tutorial sample.
- Wrong payload shape: inspect raw payload and update `HookPayload` typing.

## 8) Next improvements

- Display hex value in an in-game HTML overlay.
- Add filter by domain (`UI`, `SYSTEM`, `PLAYER`).
- Add debounce to reduce spam.
