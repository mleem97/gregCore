# gregCore API — Modder Guide

## Einstieg

```csharp
using gregCore.PublicApi;

public class MyMod : GregMod
{
    public override void OnLoad()
    {
        // Geld hinzufügen
        greg.Economy.AddMoney(500f);

        // Auf Tagesende reagieren
        greg.Time.OnDayEnd += OnNewDay;

        // UI-Feedback
        greg.UI.ShowToast("MyMod geladen!");
    }

    private void OnNewDay()
    {
        greg.UI.ShowToast("Neuer Tag!");
    }
}
```
