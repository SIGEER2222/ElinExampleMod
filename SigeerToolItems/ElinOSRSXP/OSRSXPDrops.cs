[BepInPlugin("nguyen.elin.plugins.XPDrops", "OSRS XP Drops", "1.0.0")]
public class OSRSXPDrops : BaseUnityPlugin {
    private void Awake() {
        xpThresh = Config.Bind("General", "Notification Threshold", 0.1f, new ConfigDescription("Intensity of the feature.", new AcceptableValueRange<float>(0.01f, 100f)));
        new Harmony("nguyen.elin.plugins.XPDrops").PatchAll();
    }

    public static ConfigEntry<float> xpThresh;
}