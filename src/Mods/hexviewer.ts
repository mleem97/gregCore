/**
 * hexviewer.ts
 * JADE-style telemetry overlay using gregCore TypeScript Bridge
 */

interface MetadataEntry {
    label: string;
    value: string;
    color: string;
}

interface MetadataResponse {
    title: string;
    subHeader: string;
    entries: MetadataEntry[];
}

greg.events.onUpdate((dt: number) => {
    // 1. Raycast to find target
    const target = greg.target.getTargetInfo(10.0);
    
    if (target.type === "None") {
        greg.hud.hideJadeBox();
        return;
    }

    // 2. Extract metadata via unified SDK service
    const meta = greg.metadata.getMetadata(10.0) as MetadataResponse;

    // 3. Render to HUD
    greg.hud.updateJadeBox(
        meta.title, 
        meta.subHeader, 
        meta.entries
    );
});

greg.log("HexViewer TypeScript Engine Started.");

