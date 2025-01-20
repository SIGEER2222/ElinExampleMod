[HarmonyPatch(typeof(Thing), nameof(Thing.WriteNote))]
class Thing_WriteNote_Patch {
    public const int SKILL_TAMING = 237;

    // Fetches a specified number of stats from the queue and returns them as a comma-separated string
    static string FetchStats(Queue<string> s, int count) {
        return string.Join(", ", s.Take(count));
    }

    static void Postfix(Thing __instance, UINote n) {
        if (__instance.trait is TraitDrinkMilkMother && !__instance.trait.owner.c_idRefCard.IsNull()) {
            // Temporarily set the UID and seed for character generation
            int tmp_uidNext = EClass.game.cards.uidNext;
            EClass.game.cards.uidNext = 1;
            Rand.SetSeed(1);

            // Create a character and set its level based on the owner's level and taming skill
            Chara c = CharaGen.Create(__instance.trait.owner.c_idRefCard);
            c.SetLv(Mathf.Clamp(5 + __instance.trait.owner.encLV * 5, 1, 20 + EClass.pc.Evalue(SKILL_TAMING)));

            // Reset the seed and UID
            Rand.SetSeed();
            EClass.game.cards.uidNext = tmp_uidNext;

            // Collect the best attributes and skills
            Queue<string> s = new Queue<string>();
            foreach (Element attribute in c.elements.ListBestAttributes()) {
                if ((attribute.ValueWithoutLink / 2) > 0) {
                    s.Enqueue($"{attribute.Name} {(attribute.ValueWithoutLink / 2)}");
                }
            }
            foreach (Element skill in c.elements.ListBestSkills()) {
                if ((skill.ValueWithoutLink / 2) > 0) {
                    s.Enqueue($"{skill.Name} {(skill.ValueWithoutLink / 2)}");
                }
            }

            // Add the milk bonuses to the note
            n.AddText("Milk bonuses: ", FontColor.DontChange);
            while (s.Count > 0) {
                n.AddText(FetchStats(s, 7), FontColor.DontChange);
            }
        }
    }
}