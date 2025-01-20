[HarmonyPatch(typeof(TraitAltar), nameof(TraitAltar._OnOffer))]
internal class offer_without_limit_patch {
    private static bool Prefix(TraitAltar __instance, Chara c, Thing t, int takeoverMod = 0) {

        bool isSpecialItem = t.GetBool(115);
        int offeringValue = __instance.Deity.GetOfferingValue(t, t.Num);
        offeringValue = offeringValue * (c.HasElement(1228, 1) ? 130 : 100) / 100;

        if (takeoverMod == 0) {
            if (offeringValue >= 200) {
                Msg.Say("god_offer1", t, "", null, null);
                EClass.pc.faith.Talk("offer", null, null);
            }
            else if (offeringValue >= 100) {
                Msg.Say("god_offer2", t, "", null, null);
            }
            else if (offeringValue >= 50) {
                Msg.Say("god_offer3", t, "", null, null);
            }
            else {
                Msg.Say("god_offer4", t, "", null, null);
            }
        }
        else {
            Msg.Say("god_offer1", t, "", null, null);
            offeringValue += __instance.Deity.GetOfferingValue(t, 1) * takeoverMod;
        }

        int faithValue = Mathf.Max(c.Evalue(306), 1);
        if (offer_without_limit.NoFaithLimint.Value && offeringValue > 5000) {
            Element faithElement = c.elements.GetOrCreateElement(306);
            int faithBase = faithElement.vBase;
            for (int i = 0; i < offeringValue / 5000; i++) {
                c.elements.ModBase(faithElement.id, 1);
                c.elements.OnLevelUp(faithElement, faithBase);
            }
            c.elements.ModExp(306, offeringValue % 1000, false);
            faithValue = Mathf.Max(c.Evalue(306), 1) + 1;
        }

        Element pietyElement = c.elements.GetOrCreateElement(85);
        int pietyValue = pietyElement.Value;
        if (offeringValue > 1500) {
            int pietyBase = pietyElement.vBase;
            for (int j = 0; j < offeringValue / 1500; j++) {
                c.elements.ModBase(pietyElement.id, 1);
                c.elements.OnLevelUp(pietyElement, pietyBase);
            }
            c.elements.ModExp(pietyElement.id, offeringValue % 1000, false);
        }
        else {
            c.elements.ModExp(pietyElement.id, offeringValue * 2 / 3, false);
        }

        int pietyLevel = 4;
        if (pietyElement.vBase >= faithValue) {
            int pietyExp = pietyElement.vExp;
            c.elements.SetBase(pietyElement.id, faithValue, 0);
            c.elements.ModExp(pietyElement.id, pietyExp, false);
        }
        else {
            pietyLevel = Mathf.Clamp(pietyElement.vBase * 100 / faithValue / 25, 0, 3);
        }

        if (pietyLevel == 4 || pietyElement.Value != pietyValue) {
            Msg.Say("piety" + pietyLevel.ToString(), c, c.faith.TextGodGender, null, null);
        }

        Debug.Log($"{offeringValue}/{pietyElement.Value}/{pietyElement.vExp}");
        Msg.Say($"{pietyElement.FullName} LV:{pietyElement.Value} Exp:{pietyElement.vExp}", t, "", null, null);

        if (pietyElement.Value > faithValue * 8 / 10) {
            c.elements.ModExp(306, offeringValue / 5, false);
        }

        c.RefreshFaithElement();

        if (isSpecialItem) {
            EClass.player.ModKarma(-1);
        }

        return false;
    }
}