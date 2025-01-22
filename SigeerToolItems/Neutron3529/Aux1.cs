using Neutron3529.Utils;
namespace Neutron3529.Aux {
  [BepInPlugin("Neutron3529.Aux", "Neutron3529.Aux", "0.1.0")]
  public class Aux : ModEntry {
    public Aux()
      : base("Neutron3529.Aux") {
    }

    public override void Awake() {
      base.Awake();
      ModEntry.logger("叮~Mod启动成功，请安心游戏");
    }

    public static string JoinList<T>(IEnumerable<T> content, ModEntry.Dump dump = null) {
      return (dump ?? ModEntry.Dump.instance).JoinList<T>(content);
    }

    public static string Detailed(object content, ModEntry.Dump dump = null) {
      return (dump ?? ModEntry.Dump.instance).Detailed(content);
    }

    public static void Dumping<T>(IEnumerable<T> items, Type[] as_type, string[] delayed, params (string, Func<T, object>)[] special) {
      dumping(items, as_type, delayed, special, null, default, default);
    }

    static void dumping<T>(IEnumerable<T> items, Type[] as_type = null, string[] delayed = null, (string, Func<T, object>)[] special = null, Func<object, string> to_string = null, (char, string) sep = default, (string, string) nl = default) {
      if (Dump.instance == null) {
        Dump.instance = new Dump();
      }
      var dump = Dump.instance;
      dump.toString = to_string;
      if (sep != default) { dump.cellSep = sep; }
      if (nl != default) { dump.nl = nl; }
      dump.Dumping(items, as_type, delayed, special);
    }

    public static string EvalFields(
      object item,
      bool include_field_name = true,
      char wsep = '\t',
      params string[] par) {
      return string.Join<string>(Environment.NewLine, par.Select(x => !include_field_name ? Detailed(item.GetType().GetField(x).GetValue(item)) ?? "" : x + ":" + Detailed(item.GetType().GetField(x).GetValue(item))));
    }
  }
}
