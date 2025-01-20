[HarmonyPatch]
public class InfinityMagic {
    private static int vPot;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Chara), "UseAbility", new Type[] { typeof(Act), typeof(Card), typeof(Point), typeof(bool) })]
    public static void UseAbility_Prefix(Chara __instance, Act a) {
        InfinityMagic.vPot = ((Element)a).vPotential;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chara), "UseAbility", new Type[] { typeof(Act), typeof(Card), typeof(Point), typeof(bool) })]
    public static void UseAbility_Postfix(Chara __instance, Act a) {
        if (a is Spell && ((Card)__instance).IsPC) {
            Msg.Say(InfinityMagic.vPot.ToString() + " 数量");
            if (InfinityMagic.vPot < 10) {
                ((Element)a).vPotential = 10;
            }
            else {
                ((Element)a).vPotential = InfinityMagic.vPot;
            }

            LayerAbility.SetDirty((Element)a);
        }
    }
}

