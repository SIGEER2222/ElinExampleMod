namespace GanExtendDisplay;
public class ElementShow {
  private static string ApplyTagSizeAndColor(string content, int size, Color color) {
    return ClassExtension.TagColor(ClassExtension.TagSize(content, size), color);
  }

  public static string Show_Affinity(Chara __instance, string __result) {
    int affinity = __instance._affinity;

    string affinitySymbol;
    Color color;

    // 处理负数好感度
    if (affinity < 0) {
      affinitySymbol = "✖";  // 负好感度符号
      color = new Color(0.7f, 0.1f, 0.1f);  // 红色，用于敌对或非常差的关系
    }
    // 极高的好感度
    else if (affinity > 90) {
      affinitySymbol = "❤";
      color = new Color(1f, 0.2f, 0.2f);  // 深红色
    }
    // 高好感度
    else if (affinity > 75) {
      affinitySymbol = "♥";
      color = new Color(1f, 0.5f, 0.5f);  // 浅红色
    }
    // 中等好感度
    else if (affinity > 50) {
      affinitySymbol = "♡";
      color = new Color(1f, 1f, 0.5f);  // 黄色
    }
    // 较低好感度
    else if (affinity > 25) {
      affinitySymbol = "♡";
      color = new Color(1f, 1f, 0.2f);  // 淡黄色
    }
    // 非常低的好感度
    else {
      affinitySymbol = "☆";  // 极差的关系
      color = new Color(0.5f, 0.5f, 0.5f);  // 灰色
    }

    // 返回带有标签的好感度文本
    __result = ApplyTagSizeAndColor(affinitySymbol, 20, color) + " " + affinity;
    return __result;
  }

  public static string Show_Rarity(Chara __instance, string __result) {
    string raritySymbol = "";
    Color color = Color.black;
    switch ((__instance).rarity + 1) {
      case Rarity.Crude:
        raritySymbol = "x";
        color = CS.Color_Crude;
        break;
      case Rarity.Normal:
        raritySymbol = "";
        color = CS.Color_Normal;
        break;
      case Rarity.Superior:
        raritySymbol = "△";
        color = CS.Color_Superior;
        break;
      case Rarity.Legendary:
        raritySymbol = "◇";
        color = CS.Color_Legendary;
        break;
      case Rarity.Mythical:
        raritySymbol = "☆";
        color = CS.Color_Mythical;
        break;
      case Rarity.Artifact:
        raritySymbol = "★";
        color = CS.Color_Artifact;
        break;
    }
    __result = ClassExtension.TagColor(raritySymbol, color) + " " + __result;
    return __result;
  }

  public static string Show_Lv(Chara __instance) {
    int num = 2;
    int lv = (EClass.pc).LV;
    int currentLv = (__instance).LV;

    if (currentLv >= lv * 5) num = 0;
    else if (currentLv >= lv * 2) num = 1;
    else if (currentLv <= lv / 4) num = 4;
    else if (currentLv <= lv / 2) num = 3;

    string str = num == 0 ? " ☠ " : "";
    Color levelColor = EClass.Colors.gradientLVComparison.Evaluate(0.25f * num);
    return ApplyTagSizeAndColor(" Lv." + currentLv.ToString(), 30, levelColor) + ApplyTagSizeAndColor(str, 30, levelColor);
  }

  public static string Show_HP(Chara __instance) {
    int currentHP = (__instance).hp;
    int maxHP = (__instance).MaxHP;
    Color hpColor = currentHP > maxHP * 0.2 ? new Color(0.73f, 1f, 0.82f) : new Color(1f, 0.67f, 0.67f);

    return ClassExtension.TagColor("HP:", Color.red) + ClassExtension.TagColor(currentHP + "/" + maxHP, hpColor);
  }

  public static string Show_MP(Chara __instance) {
    int currentMP = __instance.mana.value;
    int maxMP = __instance.mana.max;
    Color mpColor = currentMP > maxMP * 0.2 ? new Color(0.73f, 1f, 0.82f) : new Color(1f, 0.67f, 0.67f);

    return ClassExtension.TagColor(" MP:", Color.blue) + ClassExtension.TagColor(currentMP + "/" + maxMP, mpColor);
  }

  public static string Show_SP(Chara __instance) {
    int currentSP = __instance.stamina.value;
    int maxSP = __instance.stamina.max;
    Color spColor = currentSP > maxSP * 0.2 ? new Color(0.73f, 1f, 0.82f) : new Color(1f, 0.67f, 0.67f);

    return ClassExtension.TagColor(" SP: ", Color.green) + ClassExtension.TagColor(currentSP + "/" + maxSP, spColor);
  }

  public static string Show_RaceJob(Chara __instance) {
    string raceJobInfo = Lang._gender((__instance).bio.gender) + " " +
                         Lang.Parse("age", (__instance).bio.TextAge(__instance)) + " [" +
                         ((SourceData.BaseRow)__instance.race).GetName() + " " +
                         ((SourceData.BaseRow)__instance.job).GetName() + " " +
                         ((SourceData.BaseRow)__instance.tactics.source).GetName() + "]";
    return " " + raceJobInfo;
  }

  public static string Show_Hunger(Chara __instance) {
    return string.Format(" {0}:{1}/{2}", __instance.hunger.name, __instance.hunger.value, __instance.hunger.max);
  }

  public static string DVPV(Chara __instance) {
    return string.Format(" DV:{0} PV:{1}", (__instance).DV, (__instance).PV);
  }

  public static string StyleShow(Chara __instance) {
    string armorSkillName = ((ElementContainer)(__instance).elements).GetOrCreateElement((__instance).GetArmorSkill()).Name;
    string styleName = ClassExtension.lang("style" + __instance.body.GetAttackStyle().ToString());
    return " " + armorSkillName + " " + styleName;
  }

  public static string Show_Works(Chara __instance) {
    string worksAndHobbies = string.Join(" ", __instance.ListWorks(true).Select(work => work.Name)) +
                             string.Join(" ", __instance.ListHobbies(true).Select(hobby => hobby.Name));
    return worksAndHobbies;
  }

  public static string Show_Attributes(Chara __instance) {
    var elements = (ElementContainer)(__instance).elements;
    var elementValues = string.Join(" ", Enumerable.Range(70, 8).Select(i =>
        string.Format(" {0}:{1}", elements.GetElement(i).Name, elements.GetElement(i).Value)));
    return elementValues;
  }

  public static string Show_Debug(Chara __instance) {
    EClass.sources.tactics.map.TryGetValue(__instance.id, out var tactics);
    var tacticsId = tactics?.id ?? "predator";
    return $"Global:{(__instance).IsGlobal}  AI:{__instance.ai?.ToString()} " +
           $"{__instance.ai?.Current?.ToString()} {tacticsId}\n" +
           $"{(__instance).uid}{(__instance).IsMinion}/{(__instance).c_uidMaster}/{__instance.master?.ToString()}";
  }

  public static string Show_Weight(Chara __instance) {
    float childrenWeight = (float)(__instance).ChildrenWeight / 1000.0f;
    float weightLimit = (float)(__instance).WeightLimit / 1000.0f;
    string name = ((SourceData.BaseRow)Element.Get(11)).GetName();
    string weightInfo = string.Format("{0}s/{1}s", childrenWeight.ToString("F0"), weightLimit.ToString("F0"));
    Color weightColor = childrenWeight < weightLimit * 0.8f ? new Color(0.86f, 1f, 0.89f) : new Color(1f, 0.8f, 0.8f);

    return $" {name}:{ClassExtension.TagColor(weightInfo, weightColor)}";
  }

  public static string Show_EXP(Chara __instance) {
    int currentEXP = (__instance).exp;
    int expToNext = (__instance).ExpToNext;
    return $" EXP:{currentEXP}/{expToNext}";
  }

  public static string Show_Speed(Chara __instance) {
    return string.Format(" {0}:{1}", ((ElementContainer)(__instance).elements).GetElement(79).Name,
                         ((ElementContainer)(__instance).elements).GetElement(79).Value);
  }

  public static string Show_Resist(Chara __instance) {
    var resistElements = ((ElementContainer)(__instance).elements).ListElements(
        x => x.source.category == "resist" && x.Value != 0, null);
    return ResistCS.ShortOut(resistElements.Select(x => string.Format("{0}:{1}", ResistCS.GetName(x.Name, x.id), x.Value)).ToList());
  }
}
