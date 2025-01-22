[BepInPlugin("sigeer.GanExtendDisplay", "GanExtendDisplay", "1.0.0.0")]
public class ModsBaseLoad : SigeerBaseLoad {
    public new void Awake() {
        base.Awake();
        new Harmony("sigeer.GanExtendDisplay").PatchAll();
    }

    public new void Unload() {
        base.Unload();
        new Harmony("sigeer.GanExtendDisplay").UnpatchSelf();
    }
}