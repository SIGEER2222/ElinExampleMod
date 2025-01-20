namespace GanExtendDisplay {
  [HarmonyPatch]
  public class CharaDisplay {
    // 用于存储最后一次触发时间的字典
    public static ConcurrentDictionary<int, DateTime> lastTriggerTime = new ConcurrentDictionary<int, DateTime>();
    public static readonly TimeSpan debounceInterval = TimeSpan.FromMilliseconds(50);

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Chara), "GetHoverText")]
    public static void Chara_GetHoverText_Postfix(Chara __instance, ref string __result) {
      int instanceId = __instance.GetHashCode();
      DateTime currentTime = DateTime.Now;

      // 检查是否超过防抖间隔
      if (lastTriggerTime.TryGetValue(instanceId, out DateTime lastTime)) {
        if (currentTime - lastTime < debounceInterval) {
          // 如果小于防抖间隔，忽略这次调用
          return;
        }
      }

      // 如果超过防抖间隔，执行操作并更新最后触发时间
      lastTriggerTime[instanceId] = currentTime;
      __result = CharaShow.Chara_GetHoverText_Postfix(__instance, __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Chara), "GetHoverText2")]
    public static bool Chara_GetHoverText2_Prefix(Chara __instance, ref string __result) {
      int instanceId = __instance.GetHashCode();
      DateTime currentTime = DateTime.Now;

      // 检查是否超过防抖间隔
      if (lastTriggerTime.TryGetValue(instanceId, out DateTime lastTime)) {
        if (currentTime - lastTime < debounceInterval) {
          // 如果小于防抖间隔，忽略这次调用
          return false; // 返回false，阻止后续方法执行
        }
      }

      // 如果超过防抖间隔，执行操作并更新最后触发时间
      lastTriggerTime[instanceId] = currentTime;
      __result = CharaShow.Chara_GetHoverText2_Prefix(__instance, __result);
      return true; // 返回true，允许后续方法执行
    }
  }
}
