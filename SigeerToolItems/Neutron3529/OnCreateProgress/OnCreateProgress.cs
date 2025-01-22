using Neutron3529.Utils;
namespace Neutron3529.Aux;
[Desc("干渴之壶汲水速度最大化", Neutron3529Enable.gt, 0.0)]
[HarmonyPatch(typeof(TaskDrawWater), nameof(TaskDrawWater.OnCreateProgress))]
public class TaskDrawWaterOnCreateProgress : ModEntry.Entry {
    public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
}

[Desc("干渴之壶倒水速度最大化", Neutron3529Enable.gt, 0.0)]
[HarmonyPatch(typeof(TaskPourWater), nameof(TaskPourWater.OnCreateProgress))]
public class TaskPourWaterOnCreateProgress : ModEntry.Entry {
    public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
}

// 采集速度最大化
[Desc("采集速度最大化", Neutron3529Enable.gt, 0.0)]
[HarmonyPatch(typeof(TaskHarvest), nameof(TaskHarvest.OnCreateProgress))]
public class TaskHarvestOnCreateProgress : ModEntry.Entry {
    public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
}

[Desc("挖掘速度最大化", Neutron3529Enable.gt, 0.0)]
[HarmonyPatch(typeof(TaskDig), nameof(TaskDig.OnCreateProgress))]
public class TaskDigOnCreateProgress : ModEntry.Entry {
    public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
}

[Desc("光速钓鱼", Neutron3529Enable.gt, 0.0)]
[HarmonyPatch(typeof(AI_Fish.ProgressFish), nameof(AI_Fish.ProgressFish.OnProgress))]
public class FastFish : ModEntry.Entry {
    public static void Prefix(AI_Fish.ProgressFish __instance) {
        if (!((Card)((AIAct)__instance).owner).IsPC)
            return;
        __instance.hit = 100;
    }
}