[BepInPlugin("sigeer.SomeHelpfulMods", "SomeHelpfulMods", "1.0.0.0")]
public class ModsBaseLoad : SigeerBaseLoad {
    public new void Awake() {
        base.Awake();
        new Harmony("sigeer.SomeHelpfulMods").PatchAll();
    }

    public new void Unload() {
        base.Unload();
        new Harmony("sigeer.SomeHelpfulMods").UnpatchSelf();
    }
}