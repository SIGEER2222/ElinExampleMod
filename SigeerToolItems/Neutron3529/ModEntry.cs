using System.Linq.Expressions;

namespace Neutron3529.Utils {
    public abstract class ModEntry : BaseUnityPlugin {
        public static Harmony harmony;
        public static DescConfig config;
        public static Action<string> logger;
        private static Action<string> _logwarn;
        private static bool needDelayedInit = true;
        private static List<Base> classes = new();
        private static bool needInit = true;
        private string pluginId;

        public static void VLogger(string s) {
        }

        public static void LogWarn(string s, Exception ex = null) =>
            _logwarn(ex == null ? s : s + StrException(ex));

        public static void LogException(Exception ex) => _logwarn(StrException(ex));

        public static string StrException(Exception ex) {
            StringBuilder sb = new StringBuilder();
            Strexception(sb, ex, 0);
            return sb.ToString();
        }

        private static void Strexception(StringBuilder sb, Exception ex, int depth) {
            string newValue = depth == 0 ? "\n" : "\n" + new string(' ', depth * 4);
            sb.Append("\n").Append(ex.GetType().Name).Append(": ").Append(ex.Message).Append("\n").Append(ex.StackTrace).Replace("\n", newValue);
            if (ex.InnerException != null) {
                Strexception(sb, ex.InnerException, depth + 1);
            }
        }

        public virtual void Awake() => Init();

        public virtual void Start() => InitDelayed();

        public virtual void InitDelayed() {
            if (needInit) Init();
            if (!needDelayedInit) return;

            foreach (var baseClass in classes.Where(x => x.GetType().GetMethod(nameof(InitDelayed), BindingFlags.Instance | BindingFlags.Public).DeclaringType != typeof(Base))) {
                if (baseClass.enable) {
                    if (needDelayedInit) {
                        logger("开始二段注入");
                        needDelayedInit = false;
                    }
                    try {
                        baseClass.InitDelayed();
                        logger($"{baseClass} InitDelayed 完成");
                    }
                    catch (Exception ex) {
                        LogWarn($"{baseClass} InitDelayed 出错：", ex);
                    }
                }
            }
        }

        public virtual void Init() {
            if (!needInit) return;
            needInit = false;
            logger("开始注入");

            // 配置读取模块化
            var (whitelistEnabled, whitelistConfig) = LoadWhitelistConfig();
            var useWhitelist = ShouldUseWhitelist(whitelistEnabled, whitelistConfig);
            var whitelist = ParseWhitelist(whitelistConfig.Value);

            // 类型加载与初始化流程
            classes = LoadEnabledModules(whitelist, useWhitelist).ToList();

            // 自动维护白名单配置
            UpdateWhitelistConfig(whitelistEnabled, whitelistConfig, classes);
        }

        // region 配置处理 - - - - - - - - - - - - - - - - - - - - - -
        private (bool enabled, ConfigEntry<string> config) LoadWhitelistConfig() {
            var enabledConfig = DescConfig.ConfigEntry(
                "config",
                "..Whitelist.Enable..",
                false,
                "启用自动白名单算法（自动管理功能模块）"
            );

            var whitelistConfig = DescConfig.ConfigEntry(
                "config",
                "..Whitelist..",
                "",
                "启用模块列表（逗号分隔），留空时自动管理"
            );

            return (enabledConfig.Value, whitelistConfig);
        }

        private HashSet<string> ParseWhitelist(string configValue) =>
            new HashSet<string>(
                configValue.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x)),
                StringComparer.OrdinalIgnoreCase
            );

        private bool ShouldUseWhitelist(bool enabled, ConfigEntry<string> config) =>
            !enabled || string.IsNullOrWhiteSpace(config.Value);
        // endregion

        // region 模块加载 - - - - - - - - - - - - - - - - - - - - - -
        private IEnumerable<Base> LoadEnabledModules(HashSet<string> whitelist, bool useWhitelist) {
            foreach (var type in GetType().Module.GetTypes()) {
                if (!IsValidBaseType(type)) continue;

                var (instance, error) = TryCreateInstance(type);
                if (instance == null) {
                    LogInstanceError(type, error);
                    continue;
                }

                if (!ShouldEnableModule(instance, whitelist, useWhitelist)) continue;

                if (InitializeModule(instance)) {
                    yield return instance;
                }
            }
        }

        private bool IsValidBaseType(Type type) =>
            !type.IsAbstract && type.IsSubclassOf(typeof(Base));

        private (Base instance, Exception error) TryCreateInstance(Type type) {
            try {
                return (Activator.CreateInstance(type) as Base, null);
            }
            catch (Exception ex) {
                return (null, ex);
            }
        }

        private bool ShouldEnableModule(Base instance, HashSet<string> whitelist, bool useWhitelist) {
            if (useWhitelist) return true;

            var typeName = instance.GetType().Name;
            if (whitelist.Contains(typeName)) return true;

            logger($"功能 {typeName} 未在白名单中，已跳过");
            return false;
        }

        private bool InitializeModule(Base instance) {
            try {
                instance.Init();
                return instance.enable;
            }
            catch (Exception ex) {
                LogWarn($"{instance.GetType().Name} 初始化失败", ex);
                return false;
            }
        }
        // endregion

        // region 配置维护 - - - - - - - - - - - - - - - - - - - - - -
        private void UpdateWhitelistConfig(bool enabled, ConfigEntry<string> config, List<Base> modules) {
            if (!enabled) return;

            var newWhitelist = string.Join(",", modules.Select(m => m.GetType().Name));
            if (config.Value == newWhitelist) return;

            config.Value = newWhitelist;
            logger("已自动更新白名单配置");
        }
        // endregion

        // region 错误处理 - - - - - - - - - - - - - - - - - - - - - -
        private void LogInstanceError(Type type, Exception ex) {
            var typeCategory = type.IsSubclassOf(typeof(Entry)) ? "入口" : "功能模块";
            var errorMessage = $"{typeCategory} [{type.Name}] 初始化失败: {ex?.Message ?? "未知原因"}";

            LogWarn(errorMessage);
            if (ex != null) LogException(ex);
        }
        // endregion

        public ModEntry(string pluginId) {
            this.pluginId = pluginId;
            logger = new Action<string>(Logger.LogInfo);
            _logwarn = new Action<string>(Logger.LogWarning);
            Logger.LogInfo("此Mod使用AGPL-v3许可发布，如果你使用了这里的代码，请按照相同许可发布你修改后的mod。");
            harmony = new Harmony(pluginId);
            DescConfig.Init(this);
        }

        public abstract class Entry : Base {
            public override void Enable() => harmony.PatchAll(GetType());
        }

        public class DescConfig {
            public static ConfigFile config;

            public static void Init(ModEntry entry) => config = entry.Config;

            public static ConfigEntry<T> ConfigEntry<T>(string arg1, string arg2, T arg3, string arg4) {
                return config.Bind(arg1, arg2, arg3, arg4);
            }

            public static bool Match<T>(Desc desc, T t) => desc.match(t);
        }

        public abstract class Base {
            public bool enable;
            public string fullDesc = "";
            private static readonly Dictionary<FieldInfo, object> dict = new Dictionary<FieldInfo, object>();
            public static readonly MethodInfo ConfigBind = typeof(DescConfig).GetMethod("ConfigEntry");
            public static readonly MethodInfo DescMatch = typeof(DescConfig).GetMethod("Match");

            // 设置配置值
            public static void SetConfigValue<T>(FieldInfo f, T val) {
                if (dict.TryGetValue(f, out var obj) && obj is ConfigEntry<T> configEntry) {
                    configEntry.Value = val;
                }
                else {
                    LogWarn($"either dict contains no key of {f}, or {f} does not has the type {typeof(T)}");
                }
            }

            // 分配配置值
            public virtual bool Assign(FieldInfo f, Desc desc) {
                desc.ty = f.FieldType;
                string str = GetType().Name + "." + f.Name;
                try {
                    var methodInfo = ConfigBind.MakeGenericMethod(f.FieldType);
                    var getMethod = methodInfo.ReturnType.GetProperty("Value").GetGetMethod();
                    if (getMethod.ReturnType != f.FieldType) throw new Exception($"获取属性方法出错， prop:{getMethod.ReturnType} != field:{f.FieldType}");

                    dict[f] = methodInfo.Invoke(config, new object[] { "config", str, f.GetValue(this), desc.desc_with_enable() });
                    var v = getMethod.Invoke(dict[f], null);
                    f.SetValue(this, v);

                    return getMethod.ReturnType switch {
                        Type t when t == typeof(string) => desc.match((string)v),
                        Type t when t == typeof(bool) => desc.match((bool)v),
                        _ => (bool)DescMatch.MakeGenericMethod(getMethod.ReturnType).Invoke(null, new object[] { desc, v })
                    };
                }
                catch (Exception ex) {
                    LogWarn($"使用了不支持的ConfigEntry: {f.Name} : {f.FieldType} ，错误如下", ex);
                    return true;
                }
            }

            // 延迟初始化
            public virtual void InitDelayed() { }

            // 初始化
            public virtual void Init() => Init(true);

            // 初始化并检查启用状态
            public virtual void Init(bool checkEnable) {
                Type type = GetType();
                fullDesc = "";
                var desc = (Desc)Attribute.GetCustomAttribute(type, typeof(Desc));
                if (desc != null) {
                    if (desc.desc == null) desc.desc = type.Name;
                    enable = desc.val >= 0.0;
                    Assign(type.GetField("enable"), desc);
                    fullDesc = desc.desc;
                }

                foreach (var field in type.GetFields(~BindingFlags.Default)) {
                    var customAttribute = (Desc)Attribute.GetCustomAttribute(field, typeof(Desc));
                    if (customAttribute != null) {
                        if (customAttribute.desc == null) customAttribute.desc = fullDesc.Length > 0 ? field.Name : type.Name + "." + field.Name;
                        bool flag = Assign(field, customAttribute);
                        enable |= flag;
                        string now = $"={field.GetValue(null)}";
                        fullDesc = fullDesc.Length > 0 ? fullDesc + (desc == null ? " & " : " ： ") + (flag ? customAttribute.desc_with_disable(now) : customAttribute.desc_with_enable(now)) : flag ? customAttribute.desc_with_disable(now) : customAttribute.desc_with_enable(now);
                        desc = null;
                    }
                }

                if (checkEnable) {
                    if (enable) {
                        try {
                            Enable();
                            logger("已启用 " + fullDesc);
                        }
                        catch (Exception ex) {
                            LogWarn($"({GetType()})在执行Enable时出错，这导致Mod的「{fullDesc}」功能失效。具体错误如下：");
                            LogException(ex);
                        }
                    }
                    else {
                        logger("未启用 " + fullDesc);
                    }
                }
                else {
                    logger("开始手工处理 " + fullDesc + " 的数据");
                }
            }

            // 抽象方法，子类实现启用功能
            public abstract void Enable();
        }


        public class Dump {
            public static Dump instance = null;
            public string left = "";
            public string right = "";
            public Func<object, string> toString = x => x.ToString();
            public string sep = ", ";
            public (char, string) cellSep = ('\t', "\\t");
            public (string, string) nl = ("\n", "\\n");
            private static readonly Type selfTy = MethodBase.GetCurrentMethod().DeclaringType;
            private static readonly MethodInfo joinListGen = selfTy.GetMethod("JoinList");
            private static readonly MethodInfo processDictGen = selfTy.GetMethod("ProcessDict");

            public string JoinList<T>(IEnumerable<T> content) {
                return content.Any() ? left + string.Join(sep, content.Select(x => Detailed(x))) + right : "";
            }

            public string ProcessDict<K, V>(KeyValuePair<K, V> content) {
                return Detailed(content.Key) + " => " + Detailed(content.Value);
            }

            public string Detailed(object content) {
                if (content == null) return "";
                if (content is string str) return str;

                try {
                    var interfaces = content.GetType().FindInterfaces(Module.FilterTypeName, "IEnumerable`1*");
                    if (interfaces.Length != 0 && interfaces[0].GenericTypeArguments.Length != 0) {
                        return (string)joinListGen.MakeGenericMethod(interfaces[0].GenericTypeArguments).Invoke(this, new object[] { content });
                    }
                }
                catch (Exception ex) {
                    LogWarn($"在处理{content} as IEnumerable时出现异常", ex);
                }

                try {
                    var interfaces = content.GetType().FindInterfaces(Module.FilterTypeName, "KeyValuePair`2*");
                    if (interfaces.Length != 0 && interfaces[0].GenericTypeArguments.Length != 0) {
                        return (string)processDictGen.MakeGenericMethod(interfaces[0].GenericTypeArguments).Invoke(this, new object[] { content });
                    }
                }
                catch (Exception ex) {
                    LogWarn($"在处理{content} as KeyValuePair时出现异常", ex);
                }

                return toString != null ? toString(content) : content.ToString();
            }

            public void Dumping<T>(
    IEnumerable<T> items,
    Type[] asType,
    string[] delayed,
    (string, Func<T, object>)[] special) {
                const BindingFlags BaseFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var flags = asType == null ? BaseFlags | BindingFlags.FlattenHierarchy : BaseFlags | BindingFlags.DeclaredOnly;

                // 初始化参数
                asType ??= Array.Empty<Type>();
                delayed ??= Array.Empty<string>();
                special ??= Array.Empty<(string, Func<T, object>)>();

                // 预处理字段集合
                var excludedFields = new HashSet<string>(delayed.Concat(special.Select(x => x.Item1)));
                var sb = new StringBuilder();
                var isFirstRow = true;
                Action<T>[] valueFormatters = Array.Empty<Action<T>>();

                // 替换规则
                var (newLineOrig, newLineRepl) = nl;    // 原始换行符和替换字符
                var (cellSepOrig, cellSepRepl) = cellSep; // 原始分隔符和替换字符

                // 值处理函数
                string ProcessValue(object value) {
                    return Detailed(value)
                        .Replace(newLineOrig, newLineRepl)
                        .Replace(cellSepOrig.ToString(), cellSepRepl);
                }

                // 行结束处理
                void FinalizeLine() {
                    if (sb.Length > 0 && sb[^1] == cellSepOrig)
                        sb.Remove(sb.Length - 1, 1);
                    sb.Append(newLineOrig);
                }

                FinalizeLine(); // 初始空行

                foreach (var item in items) {
                    if (isFirstRow) {
                        var formatters = new List<Action<T>>();

                        // 特殊字段处理
                        foreach (var (name, selector) in special) {
                            sb.Append(ProcessValue(name)).Append(cellSepOrig);
                            formatters.Add(x => sb.Append(ProcessValue(selector(x))).Append(cellSepOrig));
                        }

                        // 类型相关字段
                        var processedTypes = new HashSet<Type>();
                        foreach (var type in asType.Prepend(item.GetType()).Where(t => t != null)) {
                            if (!processedTypes.Add(type)) continue;

                            foreach (var field in type.GetFields(flags).Where(f => !excludedFields.Contains(f.Name))) {
                                var accessor = CompileFieldAccessor<T>(field);
                                sb.Append(ProcessValue(field.Name)).Append(cellSepOrig);
                                formatters.Add(x => sb.Append(ProcessValue(accessor(x))).Append(cellSepOrig));
                            }
                        }

                        // 延迟加载字段
                        foreach (var fieldName in delayed) {
                            var field = item.GetType().GetField(fieldName, flags);
                            if (field == null) {
                                LogWarn($"Field not found: {fieldName}");
                                sb.Append(cellSepOrig);
                                formatters.Add(_ => sb.Append(cellSepOrig));
                                continue;
                            }

                            var accessor = CompileFieldAccessor<T>(field);
                            sb.Append(ProcessValue(fieldName)).Append(cellSepOrig);
                            formatters.Add(x => sb.Append(ProcessValue(accessor(x))).Append(cellSepOrig));
                        }

                        valueFormatters = formatters.ToArray();
                        FinalizeLine();
                        isFirstRow = false;
                    }

                    // 处理数据行
                    foreach (var formatter in valueFormatters) formatter(item);
                    FinalizeLine();
                }

                logger(sb.ToString());
            }

            // 编译字段访问器（性能提升关键）
            private static Func<T, object> CompileFieldAccessor<T>(FieldInfo field) {
                var param = Expression.Parameter(typeof(T));
                var access = Expression.Field(param, field);
                var convert = Expression.Convert(access, typeof(object));
                return Expression.Lambda<Func<T, object>>(convert, param).Compile();
            }
        }
    }
}