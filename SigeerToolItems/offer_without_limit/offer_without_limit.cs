[BepInPlugin("greenwlan.offer_without_limit", "offer_piety_without_limit", "1.0.2.1")]
public class offer_without_limit : BaseUnityPlugin {
    private void Start() {
        offer_without_limit.NoFaithLimint = base.Config.Bind<bool>("config", "NoFaithLimint", true, "解除献祭时信仰经验获取上限/Release the upper limit of faith experience obtained during sacrifice");
        Harmony harmony = new Harmony("offer_without_limit");
        harmony.PatchAll();
    }

    public static ConfigEntry<bool> NoFaithLimint;
}
