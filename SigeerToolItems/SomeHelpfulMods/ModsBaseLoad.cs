[BepInPlugin("sigeer.SomeHelpfulMods", "SomeHelpfulMods", "1.0.0.0")]
public class ModsBaseLoad : SigeerBaseLoad {
    // ========== 配置项定义 ==========
    private static ConfigEntry<bool> _enableFeature;
    private static ConfigEntry<float> _speedMultiplier;
    private static ConfigEntry<string> _filterKeywords;
    private static ConfigEntry<KeyboardShortcut> _toggleShortcut;
    private static ConfigEntry<string> _keywords;


    public new void Awake() {
        base.Awake();

        // ========== 初始化配置项 ==========
        // 基本开关
        _enableFeature = Config.Bind("通用设置",
            "启用功能",
            true,
            "是否启用特定功能");

        // 数值调整（带范围限制）
        _speedMultiplier = Config.Bind("数值调整",
            "速度倍率",
            1.5f,
            new ConfigDescription("速度调整系数",
                new AcceptableValueRange<float>(0.5f, 3.0f)));

        // 关键词过滤（逗号分隔）
        _filterKeywords = Config.Bind("过滤设置",
            "关键词",
            "作弊,外挂",
            "需要过滤的关键词列表（逗号分隔）");

        // 快捷键设置
        _toggleShortcut = Config.Bind("快捷键",
            "切换快捷键",
            new KeyboardShortcut(KeyCode.F12),
            "快速切换功能的快捷键");

        // 需过滤的关键词
        _keywords = Config.Bind("过滤设置",
            "Keywords",
            "推开了",
            "需过滤的关键词");

        // ========== 配置动态更新 ==========
        _enableFeature.SettingChanged += (s, e) =>
            Logger.LogInfo($"功能状态已切换为：{_enableFeature.Value}");

        // 初始化Harmony补丁
        new Harmony("sigeer.SomeHelpfulMods").PatchAll();
    }

    public new void Unload() {
        base.Unload();
        new Harmony("sigeer.SomeHelpfulMods").UnpatchSelf();
    }
}