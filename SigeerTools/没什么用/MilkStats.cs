[HarmonyPatch(typeof(Thing), nameof(Thing.WriteNote))]
class Thing_WriteNote_Patch {
    public const int SKILL_TAMING = 237;

    static string FetchStats(Queue<string> s, int count) {
        string r = "";
        for (int i = 1; i < count && s.Count > 1; i++) {
            r += s.Dequeue() + ", ";
        }
        r += s.Dequeue();
        return r;
    }

    static void Postfix(Thing __instance, UINote n) {
        n.AddText("Value: " + __instance.GetPrice(CurrencyType.Money, false, PriceType.Default, null).ToString(), FontColor.DontChange);

        if (__instance.trait is TraitDrinkMilkMother) {
            int tmp_uidNext = EClass.game.cards.uidNext;
            EClass.game.cards.uidNext = 1;
            Rand.SetSeed(1);
            Chara c = CharaGen.Create(__instance.trait.owner.c_idRefCard);
            c.SetLv(Mathf.Clamp(5 + __instance.trait.owner.encLV * 5, 1, 20 + EClass.pc.Evalue(SKILL_TAMING)));
            Rand.SetSeed();
            EClass.game.cards.uidNext = tmp_uidNext;

            Queue<string> s = new Queue<string>();
            foreach (Element attribute in c.elements.ListBestAttributes()) {
                if ((attribute.ValueWithoutLink / 2) > 0) {
                    s.Enqueue(attribute.Name + " " + (attribute.ValueWithoutLink / 2).ToString());
                }
            }
            foreach (Element skill in c.elements.ListBestSkills()) {
                if ((skill.ValueWithoutLink / 2) > 0) {
                    s.Enqueue(skill.Name + " " + (skill.ValueWithoutLink / 2).ToString());
                }
            }

            n.AddText("Milk bonuses: ", FontColor.DontChange);
            while (s.Count > 0) {
                n.AddText(FetchStats(s, 7), FontColor.DontChange);
            }
        }
    }
}