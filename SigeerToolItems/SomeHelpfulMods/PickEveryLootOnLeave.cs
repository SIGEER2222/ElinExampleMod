[HarmonyPatch]
public class PickEveryLootOnLeave {

    [HarmonyPrefix, HarmonyPatch(typeof(Player), nameof(Player.MoveZone))]
    public static bool Prefix(Player __instance) {
        var err = "";
        var allThings = EClass._map.things
            .Where(x => !x.ignoreAutoPick)
            ;

        var roamingThings = allThings.Where(x => x.placeState == PlaceState.roaming).ToList();

        SigeerBaseLoad.Log.LogInfo("roamingThings : " + string.Join(", ", roamingThings));

        if (roamingThings.Count > 0) {
            Msg.Say("roamingThings : " + string.Join(", ", roamingThings));
        }

        foreach (var thing in roamingThings) {
            try {
                EClass.pc.Pick(thing, true, true);
            }
            catch (System.Exception ex) {
                err += ",  " + ex.Message;
            }
        }
        SigeerBaseLoad.Log.LogInfo(err);

        return true;
    }
}