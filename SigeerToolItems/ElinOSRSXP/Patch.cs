using System;
using HarmonyLib;

namespace ElinOSRSXP {
    [HarmonyPatch(typeof(ElementContainer), "ModExp")]
    internal class Patch {
        public static void Postfix(int ele, int a, bool chain, ElementContainer __instance) {
            if (EClass.pc?.Chara?.elements != null) {
                Element element = EClass.pc.Chara.elements.GetElement(ele);
                if (element != null && !string.IsNullOrEmpty(element.Name)) {
                    float num = (float)a / 1000f;
                    if (__instance.Chara == EClass.pc.Chara && num >= OSRSXPDrops.xpThresh.Value) {
                        WidgetPopText.Say($"+{num} {element.Name} XP", FontColor.Good, null);
                    }
                }
            }
        }
    }
}