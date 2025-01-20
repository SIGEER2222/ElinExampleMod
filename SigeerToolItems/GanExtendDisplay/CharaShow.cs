namespace GanExtendDisplay {
  internal class CharaShow {
    public static string Chara_GetHoverText_Postfix(Chara __instance, string __result) {
      var builder = new StringBuilder(__result);

      builder.AppendLine();
      builder.AppendLine();
      builder.AppendLine();
      builder.AppendLine();

      builder.AppendLine(ElementShow.Show_Affinity(__instance, __result));
      // builder.AppendLine(ElementShow.Show_Rarity(__instance, __result));
      builder.AppendLine(ElementShow.Show_Lv(__instance) + __result);
      builder.AppendLine(ElementShow.Show_RaceJob(__instance));
      builder.AppendLine(ElementShow.StyleShow(__instance));
      builder.AppendLine(ElementShow.Show_HP(__instance) + "    " + ElementShow.Show_SP(__instance));
      builder.AppendLine(ElementShow.DVPV(__instance));
      builder.AppendLine(ClassExtension.TagSize(ElementShow.Show_Speed(__instance), 16) + "    " + ElementShow.Show_Hunger(__instance));
      // builder.AppendLine(ElementShow.Show_SP(__instance));
      // builder.AppendLine(ElementShow.Show_Hunger(__instance));
      builder.AppendLine(ElementShow.Show_Works(__instance));

      if (((Card)__instance).IsPCFaction) {
        builder.AppendLine(ElementShow.Show_MP(__instance));
        builder.AppendLine(ElementShow.Show_Weight(__instance));
        builder.AppendLine(ElementShow.Show_EXP(__instance));
      }

      builder.AppendLine(ClassExtension.TagSize(ElementShow.Show_Resist(__instance), 16));


      builder.AppendLine(ClassExtension.TagSize(ElementShow.Show_Attributes(__instance), 16));
      string abilities = string.Join(" ", __instance.ability.list.items.Select(x => x.act.GetText("")));
      if (!string.IsNullOrEmpty(abilities)) {
        builder.AppendLine(ClassExtension.TagSize(abilities, 16));
      }

      return builder.ToString();
    }

    public static string Chara_GetHoverText2_Prefix(Chara __instance, string __result) {
      var builder = new StringBuilder(__result);

      if (__instance.knowFav) {
        builder.AppendLine("<size=14>" + ClassExtension.lang("favgift",
            ((SourceData.BaseRow)__instance.GetFavCat()).GetName().ToLower(),
            ((SourceData.BaseRow)__instance.GetFavFood()).GetName(), null, null, null) + "</size>");
      }

      if (EClass.debug.showExtra) {
        builder.AppendLine("Global:" + ((Card)__instance).IsGlobal.ToString() + "  AI:" + __instance.ai?.ToString());
        builder.AppendLine(GetTacticsInfo(__instance));
      }

      List<Condition> conditions = __instance.conditions;
      BaseStats[] second = ((Card)__instance).IsPCFaction ? new[] { (BaseStats)__instance.hunger, (BaseStats)__instance.stamina } : new BaseStats[0];

      var stats = conditions.Concat<BaseStats>(second).ToList();
      if (stats.Any()) {
        builder.AppendLine(GetConditionInfo(stats, __instance));
      }

      return builder.ToString();
    }

    private static string GetTacticsInfo(Chara __instance) {
      EClass.sources.tactics.map.TryGetValue(__instance.id, out var tactics);
      return string.Join(" ", new[]
      {
                ((Card)__instance).uid.ToString(),
                ((Card)__instance).IsMinion.ToString(),
                ((Card)__instance).c_uidMaster.ToString(),
                __instance.master?.ToString(),
                tactics?.id ?? "predator"
      });
    }

    private static string GetConditionInfo(List<BaseStats> stats, Chara __instance) {
      var builder = new StringBuilder();
      foreach (var baseStats in stats) {
        string phaseStr = baseStats.GetPhaseStr();
        if (!ClassExtension.IsEmpty(phaseStr) && phaseStr != "#") {
          Color color = baseStats.source.group switch {
            "Bad" or "Debuff" or "Disease" => EClass.Colors.colorDebuff,
            "Buff" => EClass.Colors.colorBuff,
            _ => Color.white
          };

          string statsText = $"{phaseStr}({baseStats.GetValue()})";
          if (__instance.resistCon?.ContainsKey(baseStats.id) == true) {
            statsText += "{" + __instance.resistCon[baseStats.id] + "}";
          }

          builder.Append(ClassExtension.TagColor(statsText, color) + ", ");
        }
      }

      return ClassExtension.TagSize(builder.ToString().TrimEnd(", ".ToCharArray()), 14);
    }
  }
}
