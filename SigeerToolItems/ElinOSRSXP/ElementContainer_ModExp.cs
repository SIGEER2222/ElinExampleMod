[HarmonyPatch(typeof(ElementContainer), "ModExp")]
internal class Patch {
    public static void Postfix(int ele, int a, bool chain, ElementContainer __instance) {
        if (EClass.pc?.Chara?.elements?.GetElement(ele) is Element { Name: { Length: > 0 } } element) {
            float num = a / 1000f;
            if (num >= OSRSXPDrops.xpThresh.Value) {
                WidgetPopText.Say($"+{num} {element.Name} XP", FontColor.Good, null);
            }
        }
    }
}