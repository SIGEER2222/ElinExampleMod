namespace Neutron3529.Utils {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field, AllowMultiple = false)]
    public class Desc : Attribute {
        public string desc;
        public string str;
        public double val;
        public Neutron3529Enable enable = Neutron3529Enable.gt;
        public Type ty = typeof(void);

        public Desc(string desc, string str, Neutron3529Enable enable = Neutron3529Enable.gt, double val = 0.0) {
            this.desc = desc;
            this.str = str;
            this.enable = enable;
            this.val = val;
        }

        public Desc(string desc, double val) : this(desc, "", Neutron3529Enable.gt, val) { }

        public Desc(string desc = null, Neutron3529Enable enable = Neutron3529Enable.gt, double val = 0.0) : this(desc, "", enable, val) { }

        public Desc(string desc, string enable, double val) {
            this.desc = desc;
            this.str = "";
            this.val = val;
            this.enable = ParseEnable(enable);
        }

        private Neutron3529Enable ParseEnable(string enable) {
            return enable.ToLower() switch {
                "ge" => Neutron3529Enable.ge,
                "gt" => Neutron3529Enable.gt,
                "lt" => Neutron3529Enable.lt,
                "le" => Neutron3529Enable.le,
                "ne" => Neutron3529Enable.ne,
                "never" => Neutron3529Enable.never,
                "always" => Neutron3529Enable.always,
                _ => throw new Exception("Desc项目禁用选项" + enable + "无效")
            };
        }

        public string desc_with_enable(string now = "") {
            return Type.GetTypeCode(this.ty) switch {
                TypeCode.Object => this.desc + now,
                TypeCode.Boolean => this.desc + now + "（" + GetBooleanEnableDescription() + "）",
                TypeCode.String => this.desc + now + "（字符串长度在除去多余空白字符后" + this.enable_op + "）",
                _ => this.desc + now + "（" + this.enable_op + "）"
            };
        }

        private string GetBooleanEnableDescription() {
            return this.enable switch {
                Neutron3529Enable.gt => "为true时启用此修改",
                Neutron3529Enable.always => "总是启用",
                Neutron3529Enable.never => "不影响此项启用状态",
                _ => throw new Exception("bool的Enable情况只能为默认（其实是gt），always与never")
            };
        }

        public string enable_op {
            get {
                string prefix = string.IsNullOrEmpty(this.str) ? "" : "字符串长度";
                return prefix + this.enable switch {
                    Neutron3529Enable.lt => $"小于{this.val}时启用此修改",
                    Neutron3529Enable.le => $"小于等于{this.val}时启用此修改",
                    Neutron3529Enable.ne => $"不等于{this.val}时启用此修改",
                    Neutron3529Enable.ge => $"大于等于{this.val}时启用此修改",
                    Neutron3529Enable.gt => $"大于{this.val}时启用此修改",
                    Neutron3529Enable.always => "总是启用此修改",
                    Neutron3529Enable.never => "不影响当前项目的启用状态",
                    _ => throw new Exception($"枚举错误，this.enable的值{this.enable}不正确")
                };
            }
        }

        public string desc_with_disable(string now = "") {
            return Type.GetTypeCode(this.ty) switch {
                TypeCode.Object => this.desc + now,
                TypeCode.Boolean => this.desc + now + "（" + GetBooleanDisableDescription() + "）",
                TypeCode.String => this.desc + now + "（字符串长度在除去多余空白字符后" + this.disable_op + "）",
                _ => this.desc + now + "（" + this.disable_op + "）"
            };
        }

        private string GetBooleanDisableDescription() {
            return this.enable switch {
                Neutron3529Enable.gt => "为false时禁用此修改",
                Neutron3529Enable.always => "总是启用",
                Neutron3529Enable.never => "不影响此项启用状态",
                _ => throw new Exception("bool的Enable情况只能为默认（其实是gt），always与never")
            };
        }

        public string disable_op {
            get {
                string prefix = string.IsNullOrEmpty(this.str) ? "" : "字符串长度";
                return prefix + this.enable switch {
                    Neutron3529Enable.lt => $"大于等于{this.val}时不影响当前项目的启用状态",
                    Neutron3529Enable.le => $"大于{this.val}时不影响当前项目的启用状态",
                    Neutron3529Enable.ne => $"等于{this.val}时不影响当前项目的启用状态",
                    Neutron3529Enable.ge => $"小于{this.val}时不影响当前项目的启用状态",
                    Neutron3529Enable.gt => $"小于等于{this.val}时不影响当前项目的启用状态",
                    Neutron3529Enable.always => "总是启用此修改",
                    Neutron3529Enable.never => "不影响当前项目的启用状态",
                    _ => throw new Exception($"枚举错误，this.enable的值{this.enable}不正确")
                };
            }
        }

        public bool match<T>(T v) {
            return v switch {
                string str => match_str(str),
                bool b => match_bool(b),
                _ => match_value(Convert.ToDouble(v))
            };
        }

        private bool match_value(double v) {
            return this.enable switch {
                Neutron3529Enable.lt => v < this.val,
                Neutron3529Enable.le => v <= this.val,
                Neutron3529Enable.ne => v != this.val,
                Neutron3529Enable.ge => v >= this.val,
                Neutron3529Enable.gt => v > this.val,
                Neutron3529Enable.always => true,
                Neutron3529Enable.never => false,
                _ => false
            };
        }

        public bool match_str(string v) => match(v.Length);

        public bool match_bool(bool v) {
            return this.enable switch {
                Neutron3529Enable.gt => v,
                Neutron3529Enable.always => true,
                Neutron3529Enable.never => false,
                _ => throw new Exception("bool的Enable情况只能为默认（其实是gt），always与never")
            };
        }
    }
}

public enum Neutron3529Enable {
    lt,
    le,
    ne,
    ge,
    gt,
    always,
    never,
}