[HarmonyPatch]
public class SleepRecipeX10 {
	[HarmonyPrefix, HarmonyPatch(typeof(ConSleep), nameof(ConSleep.OnRemoved))]
	public static void OnRemoved(ConSleep __instance) {
		Debug.Log("SleepRecipeX10");
		bool flag = __instance.owner.IsPC && __instance.slept;
		if (flag) {
			for (int i = 0; i < 9; i++) {
				EClass.player.recipes.OnSleep(__instance.pcPillow?.trait is TraitPillowEhekatl);
			}
		}
	}
}
