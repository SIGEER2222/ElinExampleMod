[HarmonyPatch]
public class ContainerSizeChange {

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Game), nameof(Game.OnGameInstantiated))]
    public static void OnGameInstantiated_Prefix() {
        // 修改多个容器的大小
        ModifyContainerSize("冷蔵庫", 12, 18);
        ModifyContainerSize("木箱", 12, 18);
        ModifyContainerSize("頑丈な箱", 14, 10);
        ModifyContainerSize("出荷箱", 16, 10);
        ModifyContainerSize("宅配便", 10, 10);
    }

    // 在 Game 实例化后的后置补丁中修正出荷箱的容器大小
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Game), nameof(Game.OnGameInstantiated))]
    public static void OnGameInstantiated_Postfix() {
        var trait = ((Card)EClass.game?.cards?.container_shipping)?.trait as TraitShippingChest;
        if (trait == null)
            return;

        trait.owner?.things?.SetSize(16, 10);
    }

    // 修改特定容器的大小
    private static void ModifyContainerSize(string containerName, int width, int height) {
        var matchingThings = FindMatchingThings(containerName);
        foreach (var thing in matchingThings) {
            FixContainerSize(thing, width, height);
        }
    }

    // 查找符合条件的物品
    private static List<SourceThing.Row> FindMatchingThings(string containerName) {
        return ((SourceData<SourceThing.Row, string>)EClass.sources.things).rows?
            .FindAll(x => ((RenderRow)x).name_JP.Contains(containerName)) ?? new List<SourceThing.Row>();
    }

    // 修改容器的大小
    private static void FixContainerSize(SourceThing.Row thing, int width, int height) {
        string[] traits = ((CardRow)thing)?.trait;
        if (traits == null)
            return;

        for (int i = 0; i < traits.Length - 2; ++i) {
            try {
                if (Regex.IsMatch(traits[i], "^(Container.*)|(.*Chest)|(Fridge)$")) {
                    int.Parse(traits[i + 1]); // 尝试解析宽度
                    int.Parse(traits[i + 2]); // 尝试解析高度
                    traits[i + 1] = width.ToString();
                    traits[i + 2] = height.ToString();
                    i += 2;
                }
            }
            catch (Exception ex) {
                SigeerBaseLoad.Log.LogError($"Error parsing container size for {thing}: {ex.Message}");
            }
        }
    }

}
