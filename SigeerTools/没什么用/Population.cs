[HarmonyPatch]
public class Population {
	[HarmonyPrefix, HarmonyPatch(typeof(FactionBranch), nameof(FactionBranch.MaxPopulation), MethodType.Getter)]
	public static bool MaxPopulation(FactionBranch __instance, ref int __result) {
		__result = 100 + __instance.Evalue(2204);
		return false;
	}
}
