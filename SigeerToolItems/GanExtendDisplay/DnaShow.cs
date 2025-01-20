namespace GanExtendDisplay {
  internal class DnaShow {
    public static void DNA_WriteNote_Prefix(DNA __instance, UINote n) {
      if (__instance.slot >= 1)
        n.AddText(ClassExtension.lang("isGeneReqSlots", __instance.slot.ToString() ?? "", (string)null, (string)null, (string)null, (string)null), (FontColor)12);
      if (!__instance.CanRemove())
        n.AddText(ClassExtension.lang("isPermaGene"), (FontColor)12);
      n.Space(4, 1);
      if (__instance.type == DNA.Type.Brain) {
        SourceChara.Row row = ClassExtension.TryGetValue<string, SourceChara.Row>((IDictionary<string, SourceChara.Row>)((SourceData<SourceChara.Row, string>)EClass.sources.charas).map, __instance.id, (SourceChara.Row)null);
        if (row != null) {
          string key = ClassExtension.IsEmpty(row.tactics, ClassExtension.TryGetValue<string, SourceTactics.Row>((IDictionary<string, SourceTactics.Row>)((SourceData<SourceTactics.Row, string>)EClass.sources.tactics).map, ((CardRow)row).id, (SourceTactics.Row)null)?.id ?? ClassExtension.TryGetValue<string, SourceTactics.Row>((IDictionary<string, SourceTactics.Row>)((SourceData<SourceTactics.Row, string>)EClass.sources.tactics).map, row.job, (SourceTactics.Row)null)?.id ?? "predator");
          n.AddText(ClassExtension.lang("gene_info", ClassExtension.ToTitleCase(((SourceData.BaseRow)((SourceData<SourceTactics.Row, string>)EClass.sources.tactics).map[key]).GetName(), false), "", (string)null, (string)null, (string)null), (FontColor)21);
        }
        for (int index = 0; index < __instance.vals.Count; index += 2) {
          int val1 = __instance.vals[index];
          int val2 = __instance.vals[index + 1];
          FontColor fontColor = val2 >= 0 ? (FontColor)5 : (FontColor)6;
          string str1 = (val1 + 1).ToString() ?? "";
          string str2 = "";
          int num = Mathf.Abs(val2 / 20) + 1;
          string str3 = str2 + "[" + ClassExtension.Repeat("*", Mathf.Clamp(num, 1, 5)) + (num > 5 ? "+" : "") + "]";
          n.AddText(ClassExtension.lang("gene_info_brain", str1, str3, (string)null, (string)null, (string)null), fontColor);
        }
      }
      else {
        for (int index = 0; index < __instance.vals.Count; index += 2) {
          Element element = Element.Create(__instance.vals[index], __instance.vals[index + 1]);
          string str4 = "";
          int num = element.Value / 10;
          FontColor fontColor = (FontColor)5;
          switch (element.source.category) {
            case "slot":
              fontColor = (FontColor)104;
              num = -1;
              break;
            case "feat":
              fontColor = (FontColor)106;
              num = -1;
              break;
            case "ability":
              fontColor = (FontColor)15;
              num = -1;
              break;
          }
          if (num >= 0)
            str4 = str4 + "[" + ClassExtension.Repeat("*", Mathf.Clamp(num, 1, 5)) + (num > 5 ? "+" : "") + "]";
          string str5 = str4 + " " + string.Format("({0})", (object)element.Value);
          n.AddText(ClassExtension.lang("gene_info", ClassExtension.ToTitleCase(element.Name, true), str5, (string)null, (string)null, (string)null), fontColor);
        }
      }
    }
  }
}
