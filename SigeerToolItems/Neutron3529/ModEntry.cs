namespace Neutron3529.Utils
{
    public abstract class ModEntry : BaseUnityPlugin
    {
        public static Harmony harmony;
        public static DescConfig config;
        public static Action<string> logger;
        private static Action<string> _logwarn;
        private static bool needDelayedInit = true;
        private static Base[] classes = new Base[0];
        private static bool needInit = true;
        private string pluginId;

        public static void VLogger(string s)
        {
        }

        public static void LogWarn(string s, Exception ex = null) =>
            _logwarn(ex == null ? s : s + StrException(ex));

        public static void LogException(Exception ex) => _logwarn(StrException(ex));

        public static string StrException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            Strexception(sb, ex, 0);
            return sb.ToString();
        }

        private static void Strexception(StringBuilder sb, Exception ex, int depth)
        {
            string newValue = depth == 0 ? "\n" : "\n" + new string(' ', depth * 4);
            sb.Append("\n").Append(ex.GetType().Name).Append(": ").Append(ex.Message).Append("\n").Append(ex.StackTrace).Replace("\n", newValue);
            if (ex.InnerException != null)
            {
                Strexception(sb, ex.InnerException, depth + 1);
            }
        }

        public virtual void Awake() => Init();

        public virtual void Start() => InitDelayed();

        public virtual void InitDelayed()
        {
            if (needInit) Init();
            if (!needDelayedInit) return;

            foreach (var baseClass in classes.Where(x => x.GetType().GetMethod(nameof(InitDelayed), BindingFlags.Instance | BindingFlags.Public).DeclaringType != typeof(Base)))
            {
                if (baseClass.enable)
                {
                    if (needDelayedInit)
                    {
                        logger("开始二段注入");
                        needDelayedInit = false;
                    }
                    try
                    {
                        baseClass.InitDelayed();
                        logger($"{baseClass} InitDelayed 完成");
                    }
                    catch (Exception ex)
                    {
                        LogWarn($"{baseClass} InitDelayed 出错：", ex);
                    }
                }
            }
        }

        public virtual void Init()
        {
            if (!needInit) return;
            needInit = false;
            logger("开始注入");

            bool whitelistEnabled = DescConfig.ConfigEntry("config", "..Whitelist.Enable..", false, "会启用自动白名单算法...").Value;
            var configEntry = DescConfig.ConfigEntry("config", "..Whitelist..", "", "当白名单不为空且启用时...");

            bool useWhitelist = string.IsNullOrWhiteSpace(configEntry.Value) || !whitelistEnabled;
            var whitelist = new HashSet<string>(configEntry.Value.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)));

            var baseList = new List<Base>();
            foreach (var type in GetType().Module.GetTypes())
            {
                if (!type.IsAbstract && type.IsSubclassOf(typeof(Base)))
                {
                    try
                    {
                        var baseInstance = (Base)Activator.CreateInstance(type);
                        if (baseInstance != null)
                        {
                            if (useWhitelist || whitelist.Contains(baseInstance.GetType().Name))
                            {
                                try
                                {
                                    baseInstance.Init();
                                    if (baseInstance.enable) baseList.Add(baseInstance);
                                }
                                catch (Exception ex)
                                {
                                    LogWarn($"{baseInstance} Init出错", ex);
                                }
                            }
                            else
                            {
                                logger($"功能{baseInstance.GetType().Name}不在启用列表..Whitelist..中，因而不启用这一功能");
                            }
                        }
                        else
                        {
                            LogWarn($"{(type.IsSubclassOf(typeof(Entry)) ? "Entry" : "Base")}：{type}的type.GetConstructor().Invoke()是null，这多半是mod出了问题，如果你看到这个，请联系mod作者。");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogWarn($"{pluginId}({type})出现了意料之外的错误，或许你可以联系Mod作者修复。");
                        LogException(ex);
                    }
                }
            }
            classes = baseList.ToArray();

            if (!useWhitelist) return;
            string newWhitelist = whitelistEnabled ? string.Join(",", classes.Select(x => x.GetType().Name)) : "";
            if (configEntry.Value != newWhitelist) configEntry.Value = newWhitelist;
        }

        public ModEntry(string pluginId)
        {
            this.pluginId = pluginId;
            logger = new Action<string>(Logger.LogInfo);
            _logwarn = new Action<string>(Logger.LogWarning);
            Logger.LogInfo("此Mod使用AGPL-v3许可发布，如果你使用了这里的代码，请按照相同许可发布你修改后的mod。");
            harmony = new Harmony(pluginId);
            DescConfig.Init(this);
        }

        public abstract class Entry : Base
        {
            public override void Enable() => harmony.PatchAll(GetType());
        }

        public class DescConfig
        {
            public static ConfigFile config;

            public static void Init(ModEntry entry) => config = entry.Config;

            public static ConfigEntry<T> ConfigEntry<T>(string arg1, string arg2, T arg3, string arg4)
            {
                return config.Bind(arg1, arg2, arg3, arg4);
            }

            public static bool Match<T>(Desc desc, T t) => desc.match(t);
        }

        public abstract class Base
        {
            public bool enable;
            public string fullDesc = "";
            private static readonly Dictionary<FieldInfo, object> dict = new Dictionary<FieldInfo, object>();
            public static readonly MethodInfo ConfigBind = typeof(DescConfig).GetMethod("ConfigEntry");
            public static readonly MethodInfo DescMatch = typeof(DescConfig).GetMethod("Match");

            // 设置配置值
            public static void SetConfigValue<T>(FieldInfo f, T val)
            {
                if (dict.TryGetValue(f, out var obj) && obj is ConfigEntry<T> configEntry)
                {
                    configEntry.Value = val;
                }
                else
                {
                    LogWarn($"either dict contains no key of {f}, or {f} does not has the type {typeof(T)}");
                }
            }

            // 分配配置值
            public virtual bool Assign(FieldInfo f, Desc desc)
            {
                desc.ty = f.FieldType;
                string str = GetType().Name + "." + f.Name;
                try
                {
                    var methodInfo = ConfigBind.MakeGenericMethod(f.FieldType);
                    var getMethod = methodInfo.ReturnType.GetProperty("Value").GetGetMethod();
                    if (getMethod.ReturnType != f.FieldType) throw new Exception($"获取属性方法出错， prop:{getMethod.ReturnType} != field:{f.FieldType}");

                    dict[f] = methodInfo.Invoke(config, new object[] { "config", str, f.GetValue(this), desc.desc_with_enable() });
                    var v = getMethod.Invoke(dict[f], null);
                    f.SetValue(this, v);

                    return getMethod.ReturnType switch
                    {
                        Type t when t == typeof(string) => desc.match((string)v),
                        Type t when t == typeof(bool) => desc.match((bool)v),
                        _ => (bool)DescMatch.MakeGenericMethod(getMethod.ReturnType).Invoke(null, new object[] { desc, v })
                    };
                }
                catch (Exception ex)
                {
                    LogWarn($"使用了不支持的ConfigEntry: {f.Name} : {f.FieldType} ，错误如下", ex);
                    return true;
                }
            }

            // 延迟初始化
            public virtual void InitDelayed() { }

            // 初始化
            public virtual void Init() => Init(true);

            // 初始化并检查启用状态
            public virtual void Init(bool checkEnable)
            {
                Type type = GetType();
                fullDesc = "";
                var desc = (Desc)Attribute.GetCustomAttribute(type, typeof(Desc));
                if (desc != null)
                {
                    if (desc.desc == null) desc.desc = type.Name;
                    enable = desc.val >= 0.0;
                    Assign(type.GetField("enable"), desc);
                    fullDesc = desc.desc;
                }

                foreach (var field in type.GetFields(~BindingFlags.Default))
                {
                    var customAttribute = (Desc)Attribute.GetCustomAttribute(field, typeof(Desc));
                    if (customAttribute != null)
                    {
                        if (customAttribute.desc == null) customAttribute.desc = fullDesc.Length > 0 ? field.Name : type.Name + "." + field.Name;
                        bool flag = Assign(field, customAttribute);
                        enable |= flag;
                        string now = $"={field.GetValue(null)}";
                        fullDesc = fullDesc.Length > 0 ? fullDesc + (desc == null ? " & " : " ： ") + (flag ? customAttribute.desc_with_disable(now) : customAttribute.desc_with_enable(now)) : flag ? customAttribute.desc_with_disable(now) : customAttribute.desc_with_enable(now);
                        desc = null;
                    }
                }

                if (checkEnable)
                {
                    if (enable)
                    {
                        try
                        {
                            Enable();
                            logger("已启用 " + fullDesc);
                        }
                        catch (Exception ex)
                        {
                            LogWarn($"({GetType()})在执行Enable时出错，这导致Mod的「{fullDesc}」功能失效。具体错误如下：");
                            LogException(ex);
                        }
                    }
                    else
                    {
                        logger("未启用 " + fullDesc);
                    }
                }
                else
                {
                    logger("开始手工处理 " + fullDesc + " 的数据");
                }
            }

            // 抽象方法，子类实现启用功能
            public abstract void Enable();
        }


        public class Dump
        {
            public static Dump instance = new Dump();
            public string left = "";
            public string right = "";
            public Func<object, string> toString = x => x.ToString();
            public string sep = ", ";
            public (char, string) cellSep = ('\t', "\\t");
            public (string, string) nl = ("\n", "\\n");
            private static readonly Type selfTy = MethodBase.GetCurrentMethod().DeclaringType;
            private static readonly MethodInfo joinListGen = selfTy.GetMethod("JoinList");
            private static readonly MethodInfo processDictGen = selfTy.GetMethod("ProcessDict");

            public string JoinList<T>(IEnumerable<T> content)
            {
                return content.Any() ? left + string.Join(sep, content.Select(x => Detailed(x))) + right : "";
            }

            public string ProcessDict<K, V>(KeyValuePair<K, V> content)
            {
                return Detailed(content.Key) + " => " + Detailed(content.Value);
            }

            public string Detailed(object content)
            {
                if (content == null) return "";
                if (content is string str) return str;

                try
                {
                    var interfaces = content.GetType().FindInterfaces(Module.FilterTypeName, "IEnumerable`1*");
                    if (interfaces.Length != 0 && interfaces[0].GenericTypeArguments.Length != 0)
                    {
                        return (string)joinListGen.MakeGenericMethod(interfaces[0].GenericTypeArguments).Invoke(this, new object[] { content });
                    }
                }
                catch (Exception ex)
                {
                    LogWarn($"在处理{content} as IEnumerable时出现异常", ex);
                }

                try
                {
                    var interfaces = content.GetType().FindInterfaces(Module.FilterTypeName, "KeyValuePair`2*");
                    if (interfaces.Length != 0 && interfaces[0].GenericTypeArguments.Length != 0)
                    {
                        return (string)processDictGen.MakeGenericMethod(interfaces[0].GenericTypeArguments).Invoke(this, new object[] { content });
                    }
                }
                catch (Exception ex)
                {
                    LogWarn($"在处理{content} as KeyValuePair时出现异常", ex);
                }

                return toString != null ? toString(content) : content.ToString();
            }

            public void Dumping<T>(IEnumerable<T> items, Type[] asType, string[] delayed, (string, Func<T, object>)[] special)
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var bindingFlags2 = asType == null ? bindingFlags | BindingFlags.FlattenHierarchy : bindingFlags | BindingFlags.DeclaredOnly;
                asType ??= new Type[0];
                delayed ??= new string[0];
                special ??= new (string, Func<T, object>)[0];

                var hs = new HashSet<string>(delayed.Concat(special.Select(x => x.Item1)));
                bool flag = true;
                var sb = new StringBuilder();
                Action<T>[] actionArray = new Action<T>[0];
                Func<object, StringBuilder> sbAppend = x => sb.Append($"{Detailed(x).Replace(nl.Item1, nl.Item2).Replace(cellSep.Item1.ToString(), cellSep.Item2)}{cellSep.Item1}");
                Action action1 = () =>
                {
                    while (sb.Length > 0 && sb[^1] == cellSep.Item1) sb.Remove(sb.Length - 1, 1);
                    sb.Append(nl.Item1);
                };
                action1();

                foreach (var obj in items)
                {
                    if (flag)
                    {
                        var actionList = new List<Action<T>>();
                        foreach (var sp in special)
                        {
                            sbAppend(sp.Item1);
                            actionList.Add(x => sbAppend(Detailed(sp.Item2(x))));
                        }
                        foreach (var type in asType.Prepend(obj.GetType()))
                        {
                            foreach (var field in type.GetFields().Where(field => !hs.Contains(field.Name)))
                            {
                                sbAppend(field.Name);
                                actionList.Add(x => sbAppend(field.GetValue(x)));
                            }
                        }
                        foreach (var name in delayed)
                        {
                            var field = obj.GetType().GetField(name, bindingFlags2);
                            if (field != null)
                            {
                                sbAppend(name);
                                actionList.Add(x => sbAppend(field.GetValue(x)));
                            }
                            else
                            {
                                sbAppend("");
                                LogWarn("找不到field " + name);
                            }
                        }
                        actionArray = actionList.ToArray();
                        action1();
                        flag = false;
                    }
                    foreach (var action2 in actionArray) action2(obj);
                    action1();
                }
                logger(sb.ToString());
            }
        }
    }
}