[BepInPlugin("net.kuronekotei.af_cats_goods", "AfCatsGoods", "1.0.0.0")]
public class Mod_TpAfCatsGoods : BaseUnityPlugin {
	private void Awake() {
		new Harmony("AfCatsGoods").PatchAll();
	}

	public void OnStartCore() {
		AfCatsGoods.OnStartCore();
	}
}
