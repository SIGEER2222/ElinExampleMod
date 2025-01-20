namespace GanExtendDisplay {
  public static class ResistCS {
    public static Color flameResistance = new Color(1f, 0.5f, 0.2f);
    public static Color coldResistance = new Color(0.2f, 0.5f, 1f);
    public static Color electricResistance = new Color(1f, 1f, 0.2f);
    public static Color darkResistance = new Color(0.5f, 0.2f, 0.5f);
    public static Color mesmerizeResistance = new Color(0.5f, 0.2f, 1f);
    public static Color poisonResistance = new Color(0.2f, 1f, 0.2f);
    public static Color hellResistance = new Color(1f, 0.2f, 0.2f);
    public static Color soundResistance = new Color(1f, 0.5f, 0.5f);
    public static Color nerveResistance = new Color(0.2f, 1f, 0.6f);
    public static Color chaosResistance = new Color(0.6f, 0.3f, 0.2f);
    public static Color divineResistance = new Color(1f, 1f, 0.5f);
    public static Color magicResistance = new Color(0.2f, 0.2f, 1f);
    public static Color aetherResistance = new Color(0.2f, 0.2f, 1f);
    public static Color acidResistance = new Color(0.2f, 1f, 0.2f);
    public static Color bleedingResistance = new Color(1f, 0.2f, 0.2f);
    public static Color impactResistance = new Color(0.2f, 0.2f, 1f);
    public static Color corruptionResistance = new Color(0.6f, 0.3f, 0.2f);
    public static Color damageResistance = new Color(0.5f, 0.5f, 0.5f);
    public static Color curseResistance = new Color(0.2f, 0.2f, 0.2f);

    public static string GetName(string res, int id) {
      switch (id) {
        case 950:
          res = ClassExtension.TagColor(res, ResistCS.flameResistance);
          break;
        case 951:
          res = ClassExtension.TagColor(res, ResistCS.coldResistance);
          break;
        case 952:
          res = ClassExtension.TagColor(res, ResistCS.electricResistance);
          break;
        case 953:
          res = ClassExtension.TagColor(res, ResistCS.darkResistance);
          break;
        case 954:
          res = ClassExtension.TagColor(res, ResistCS.mesmerizeResistance);
          break;
        case 955:
          res = ClassExtension.TagColor(res, ResistCS.poisonResistance);
          break;
        case 956:
          res = ClassExtension.TagColor(res, ResistCS.hellResistance);
          break;
        case 957:
          res = ClassExtension.TagColor(res, ResistCS.soundResistance);
          break;
        case 958:
          res = ClassExtension.TagColor(res, ResistCS.nerveResistance);
          break;
        case 959:
          res = ClassExtension.TagColor(res, ResistCS.chaosResistance);
          break;
        case 960:
          res = ClassExtension.TagColor(res, ResistCS.divineResistance);
          break;
        case 961:
          res = ClassExtension.TagColor(res, ResistCS.magicResistance);
          break;
        case 962:
          res = ClassExtension.TagColor(res, ResistCS.aetherResistance);
          break;
        case 963:
          res = ClassExtension.TagColor(res, ResistCS.acidResistance);
          break;
        case 964:
          res = ClassExtension.TagColor(res, ResistCS.bleedingResistance);
          break;
        case 965:
          res = ClassExtension.TagColor(res, ResistCS.impactResistance);
          break;
        case 970:
          res = ClassExtension.TagColor(res, ResistCS.corruptionResistance);
          break;
        case 971:
          res = ClassExtension.TagColor(res, ResistCS.damageResistance);
          break;
        case 972:
          res = ClassExtension.TagColor(res, ResistCS.curseResistance);
          break;
      }
      return res;
    }

    public static string ShortOut(List<string> inList, int lineSize = 5) {
      if (inList.Count == 0)
        return "";
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      foreach (string str in inList) {
        ++num;
        if (num % lineSize == 1)
          stringBuilder.Append(Environment.NewLine).Append(str);
        else
          stringBuilder.Append(" ").Append(str);
      }
      return stringBuilder.ToString();
    }
  }
}
