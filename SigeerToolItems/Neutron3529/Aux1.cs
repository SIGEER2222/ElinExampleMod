using Neutron3529.Utils;
namespace Neutron3529.Aux
{
    [BepInPlugin("Neutron3529.Aux", "Neutron3529.Aux", "0.1.0")]
    public class Aux : ModEntry
    {
        public Aux()
          : base("Neutron3529.Aux")
        {
        }

        public override void Awake()
        {
            base.Awake();
            ModEntry.logger("叮~Mod启动成功，请安心游戏");
        }

        private static string JoinList<T>(IEnumerable<T> content, ModEntry.Dump dump = null)
        {
            return (dump ?? ModEntry.Dump.instance).JoinList<T>(content);
        }

        private static string Detailed(object content, ModEntry.Dump dump = null)
        {
            return (dump ?? ModEntry.Dump.instance).Detailed(content);
        }

        private static void Dumping<T>(
          IEnumerable<T> items,
          Type[] as_type,
          string[] delayed,
          params (string, Func<T, object>)[] special)
        {
            Neutron3529.Aux.Aux.Dumping<T>(items, as_type, delayed, special);
        }

        private static void Dumping<T>(
          IEnumerable<T> items,
          Type[] as_type = null,
          string[] delayed = null,
          (string, Func<T, object>)[] special = null,
          Func<object, string> to_string = null,
          (char, string) sep = default((char, string)),
          (string, string) nl = default((string, string)))
        {
            ModEntry.Dump dump = new ModEntry.Dump()
            {
                toString = to_string
            };
            (char ch, string str) = sep;
            if (ch != char.MinValue || str != (string)null)
                dump.cellSep = sep;
            (string, string) tuple = nl;
            if (tuple.Item1 != (string)null || tuple.Item2 != (string)null)
                dump.nl = nl;
            dump.Dumping<T>(items, as_type, delayed, special);
        }

        private static string eval_fields(object item, params string[] par)
        {
            return Neutron3529.Aux.Aux.eval_fields(item, true, '\t', par);
        }

        private static string eval_fields(object item, bool include_field_name = true, params string[] par)
        {
            return Neutron3529.Aux.Aux.eval_fields(item, true, '\t', par);
        }

        private static string eval_fields(object item, char wsep = '\t', params string[] par)
        {
            return Neutron3529.Aux.Aux.eval_fields(item, true, wsep, par);
        }

        private static string eval_fields(
          object item,
          bool include_field_name = true,
          char wsep = '\t',
          params string[] par)
        {
            return string.Join<string>(Environment.NewLine, par.Select(x => !include_field_name ? Detailed(item.GetType().GetField(x).GetValue(item)) ?? "" : x + ":" + Detailed(item.GetType().GetField(x).GetValue(item))));
        }

        [Desc("信息计算-种族表（输出到log）", -1.0)]
        private class Info_Race : ModEntry.Entry
        {
            public override void InitDelayed()
            {
                base.InitDelayed();
                ModEntry.logger("种族表:");
                Neutron3529.Aux.Aux.Dumping<SourceRace.Row>((IEnumerable<SourceRace.Row>)((SourceData<SourceRace.Row, string>)EClass.sources.races).rows, (Type[])null, (string[])null, ("id", (Func<SourceRace.Row, object>)(x => (object)x.id)), ("name", (Func<SourceRace.Row, object>)(x => (object)((SourceData.BaseRow)x).GetName())), ("detail", (Func<SourceRace.Row, object>)(x => (object)((SourceData.BaseRow)x).GetDetail())));
            }

            public override void Enable()
            {
            }
        }

        [Desc("信息计算-材料等级表（输出到log）", -1.0)]
        private class Info_MaterialTier : ModEntry.Entry
        {
            public override void InitDelayed()
            {
                base.InitDelayed();
                ModEntry.logger("材料等级表:");
                foreach (KeyValuePair<string, SourceMaterial.TierList> tier1 in SourceMaterial.tierMap)
                {
                    ModEntry.logger("Group " + tier1.Key + ":");
                    int num = 0;
                    foreach (SourceMaterial.Tier tier2 in tier1.Value.tiers)
                        ModEntry.logger(string.Format("    {0}: sum = {1}, list = {2}", (object)num++, (object)tier2.sum, (object)Neutron3529.Aux.Aux.JoinList<string>(tier2.list.Select<SourceMaterial.Row, string>((Func<SourceMaterial.Row, string>)(x => x.name_L)))));
                }
            }

            public override void Enable()
            {
            }
        }

        [Desc("信息计算-材料表（输出到log）", -1.0)]
        private class Info_Material : ModEntry.Entry
        {
            public override void InitDelayed()
            {
                base.InitDelayed();
                ModEntry.logger("材料表:");
                Neutron3529.Aux.Aux.Dumping<SourceMaterial.Row>((IEnumerable<SourceMaterial.Row>)((SourceData<SourceMaterial.Row, int>)EClass.sources.materials).rows, (Type[])null, (string[])null, ("id", (Func<SourceMaterial.Row, object>)(x => (object)x.id.ToString())), ("name", (Func<SourceMaterial.Row, object>)(x => (object)((SourceData.BaseRow)x).GetName())), ("detail", (Func<SourceMaterial.Row, object>)(x => (object)((SourceData.BaseRow)x).GetDetail())));
            }

            public override void Enable()
            {
            }
        }

        [Desc("信息计算-配方表与锤炼法（输出到log）", -1.0)]
        private class Info_Recipe : ModEntry.Entry
        {
            public static string IngName(Recipe.Ingredient ing)
            {
                string[] strArray = new string[5]
                {
          ing.GetName(),
          "(",
          ing.id,
          null,
          null
                };
                string str;
                if (ing?.idOther?.Count <= 0)
                    str = "";
                else
                    str = "," + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)ing.idOther, new ModEntry.Dump()
                    {
                        left = "(可用",
                        right = "代替)"
                    });
                strArray[3] = str;
                strArray[4] = ")";
                return string.Concat(strArray);
            }

            public static string IngNameExact(string id, Recipe.Ingredient ing)
            {
                return (ing.useCat ? ClassExtension.lang("ingCat", ((SourceData.BaseRow)((SourceData<SourceCategory.Row, string>)EClass.sources.categories).map[id]).GetName(), (string)null, (string)null, (string)null, (string)null) : ClassExtension.lang(ClassExtension.IsEmpty(((SourceData.BaseRow)EClass.sources.cards.map[id]).GetName(), "card_" + id)) + (ClassExtension.IsEmpty(ing.tag) ? "" : "(" + ClassExtension.lang("tag_" + ing.tag) + ")")) ?? "";
            }

            public override void InitDelayed()
            {
                base.InitDelayed();
                ModEntry.logger("配方表:");
                StringBuilder stringBuilder = new StringBuilder();
                RecipeManager.BuildList();
                foreach (KeyValuePair<string, RecipeSource> keyValuePair in RecipeManager.dict)
                {
                    List<Recipe.Ingredient> ingredients = keyValuePair.Value.GetIngredients();
                    ModEntry.logger(string.Format("{0}({1})\tNeedFactory:{2}\tFactory:{3}\t{4}\tGetIngredients:{5}", (object)keyValuePair.Value.Name, (object)keyValuePair.Key, (object)keyValuePair.Value.NeedFactory, (object)Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)keyValuePair.Value.row.factory), (object)Neutron3529.Aux.Aux.eval_fields((object)keyValuePair.Value, "type", "id", "colorIng", "isBridge", "isBridgePillar", "isChara", "noListing", "isRandom", "alwaysKnown"), (object)string.Join(",\t", ingredients.Select<Recipe.Ingredient, string>((Func<Recipe.Ingredient, string>)(x => Neutron3529.Aux.Aux.Info_Recipe.IngName(x) + "\t" + Neutron3529.Aux.Aux.eval_fields((object)x, "uid", "req", "mat", "refVal", "id", "tag", "optional", "dye", "useCat"))))));
                    // ISSUE: explicit non-virtual call
                    if ((ingredients != null ? __nonvirtual(ingredients.Count) : 0) > 0)
                    {
                        List<string> idOther = ingredients[0].idOther;
                        // ISSUE: explicit non-virtual call
                        if ((idOther != null ? __nonvirtual(idOther.Count) : 0) > 0)
                        {
                            foreach (string id in ingredients[0].idOther)
                                stringBuilder.AppendLine(string.Format("from:({0})x{1} {2} to:{3} with:{4}", (object)Neutron3529.Aux.Aux.Info_Recipe.IngNameExact(id, ingredients[0]), (object)ingredients[0].req, (object)(Neutron3529.Aux.Aux.JoinList<string>(ingredients.Skip<Recipe.Ingredient>(1).Where<Recipe.Ingredient>((Func<Recipe.Ingredient, bool>)(x => !x.optional)).Select<Recipe.Ingredient, string>((Func<Recipe.Ingredient, string>)(x => string.Format("+ {0}x{1} ", (object)Neutron3529.Aux.Aux.Info_Recipe.IngName(x), (object)x.req))), new ModEntry.Dump()
                                {
                                    sep = ""
                                }) ?? ""), (object)Neutron3529.Aux.Aux.Info_Recipe.IngNameExact(ingredients[0].id, ingredients[0]), (object)keyValuePair.Value.Name));
                        }
                    }
                }
                ModEntry.logger("配方表（另表）:");
                foreach (SourceRecipe.Row row in ((SourceData<SourceRecipe.Row, int>)EClass.sources.recipes).rows)
                {
                    CardRow cardRow;
                    if (EClass.sources.cards.map.TryGetValue(row.thing, out cardRow))
                        ModEntry.logger(ClassExtension.lang(ClassExtension.IsEmpty(((SourceData.BaseRow)cardRow).GetName(), "card_" + row.thing)) + "(" + row.thing + "):\t" + Neutron3529.Aux.Aux.eval_fields((object)row, "id", "factory", "type", "num", "sp", "time") + "\t" + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)row.ing1) + "\t" + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)row.ing2) + "\t" + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)row.ing3) + "\t" + row.detail_L);
                    else
                        ModEntry.logger(ClassExtension.lang("card_" + row.thing) + "(" + row.thing + "):\t" + Neutron3529.Aux.Aux.eval_fields((object)row, "id", "factory", "type", "num", "sp", "time") + "\t" + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)row.ing1) + "\t" + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)row.ing2) + "\t" + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)row.ing3) + "\t" + row.detail_L);
                }
                ModEntry.logger(string.Format("锤炼法:\n{0}", (object)stringBuilder));
            }

            public override void Enable()
            {
            }
        }

        [Desc("信息计算-SourceCard(things/charas)", -1.0)]
        private class Info_SourceCard : ModEntry.Entry
        {
            public override void InitDelayed()
            {
                base.InitDelayed();
                ModEntry.logger("category表:");
                Neutron3529.Aux.Aux.Dumping<SourceCategory.Row>((IEnumerable<SourceCategory.Row>)((SourceData<SourceCategory.Row, string>)EClass.sources.categories).rows, (Type[])null, (string[])null, ("id", (Func<SourceCategory.Row, object>)(x => (object)x.id)), ("name", (Func<SourceCategory.Row, object>)(x => (object)((SourceData.BaseRow)x)?.GetName() ?? (object)"")), ("detail", (Func<SourceCategory.Row, object>)(x => (object)((SourceData.BaseRow)x)?.GetDetail() ?? (object)"")));
                ModEntry.logger("SourceCard表(things):");
                Neutron3529.Aux.Aux.Dumping<CardRow>((IEnumerable<CardRow>)((SourceData<SourceThing.Row, string>)EClass.sources.things).rows, (Type[])null, new string[3]
                {
          "name_JP",
          "detail_JP",
          "name2_JP"
                }, ("id", (Func<CardRow, object>)(x => (object)x.id)), ("name", (Func<CardRow, object>)(x => (object)((SourceData.BaseRow)x)?.GetName() ?? (object)"")), ("detail", (Func<CardRow, object>)(x => (object)((SourceData.BaseRow)x)?.GetDetail() ?? (object)"")), ("Name2", (Func<CardRow, object>)(x => (object)Neutron3529.Aux.Aux.Detailed((object)((IEnumerable<string>)x.name2).Select<string, string>((Func<string, string>)(x => ClassExtension.lang(x)))))));
                ModEntry.logger("SourceCard表(charas):");
                Neutron3529.Aux.Aux.Dumping<CardRow>((IEnumerable<CardRow>)((SourceData<SourceChara.Row, string>)EClass.sources.charas).rows, (Type[])null, new string[3]
                {
          "name_JP",
          "detail_JP",
          "name2_JP"
                }, ("id", (Func<CardRow, object>)(x => (object)x.id)), ("name", (Func<CardRow, object>)(x => (object)((SourceData.BaseRow)x)?.GetName() ?? (object)"")), ("detail", (Func<CardRow, object>)(x => (object)((SourceData.BaseRow)x)?.GetDetail() ?? (object)"")), ("Name2", (Func<CardRow, object>)(x => (object)Neutron3529.Aux.Aux.Detailed((object)((IEnumerable<string>)x.name2).Select<string, string>((Func<string, string>)(x => ClassExtension.lang(x)))))));
            }

            public override void Enable()
            {
            }
        }

        [Desc("信息计算-特典代码（输出到log）", -1.0)]
        private class Info_ElinEncoder : ModEntry.Entry
        {
            public override void Enable()
            {
            }

            public override void InitDelayed()
            {
                ModEntry.logger("特典代码: " + ElinEncoder.AesEncrypt("2504360").TrimEnd('='));
                ModEntry.logger("特典解码: " + ElinEncoder.AesDecrypt("/RR9CuHv6JQ="));
            }
        }

        [Desc("光速钓鱼", Neutron3529Enable.gt, 0.0)]
        [HarmonyPatch(typeof(AI_Fish.ProgressFish), "OnProgress")]
        private class FastFish : ModEntry.Entry
        {
            public static void Prefix(AI_Fish.ProgressFish __instance)
            {
                if (!((Card)((AIAct)__instance).owner).IsPC)
                    return;
                __instance.hit = 100;
            }
        }

        [HarmonyPatch(typeof(AI_Steal), "Run")]
        private class FastSteal : ModEntry.Entry
        {
            [Desc("侠盗：偷窃增加善恶值", Neutron3529Enable.gt, 0.0)]
            private static bool good = true;
            [Desc("侠盗：光速偷窃", Neutron3529Enable.gt, 0.0)]
            private static bool fast = true;

            public static IEnumerable<AIAct.Status> Postfix(
              IEnumerable<AIAct.Status> res,
              AI_Steal __instance)
            {
                IEnumerator<AIAct.Status> result = res?.GetEnumerator();
                result.MoveNext();
                if (Neutron3529.Aux.Aux.FastSteal.fast && ((Card)((AIAct)__instance)?.owner)?.IsPC.GetValueOrDefault() && ((AIAct)__instance)?.child is Progress_Custom child)
                {
                    child.maxProgress = 1;
                    if (result.Current != 1)
                        ((AIProgress)child).progress = 1;
                    child.interval = 1;
                }
                yield return result.Current;
                if (Neutron3529.Aux.Aux.FastSteal.good)
                    EClass.player.ModKarma(2);
                while (result.MoveNext())
                    yield return result.Current;
            }
        }

        [HarmonyPatch(typeof(FactionBranch), "OnClaimZone")]
        private class FactionBranchOnClaimZone_FEAT : ModEntry.Entry
        {
            [Desc("地区第一特性id", "ge", 0.0)]
            private static int zone1 = 3603;
            [Desc("地区第二特性id", "ge", 0.0)]
            private static int zone2 = 3709;
            [Desc("地区第三特性id", "ge", 0.0)]
            private static int zone3 = 3900;

            public static void Prefix(FactionBranch __instance)
            {
                Zone owner = __instance.owner;
                if (owner.landFeats == null)
                    owner.ListLandFeats();
                int[] numArray = new int[3]
                {
          Neutron3529.Aux.Aux.FactionBranchOnClaimZone_FEAT.zone1,
          Neutron3529.Aux.Aux.FactionBranchOnClaimZone_FEAT.zone2,
          Neutron3529.Aux.Aux.FactionBranchOnClaimZone_FEAT.zone3
                };
                for (int index = 0; index < 3; ++index)
                {
                    if (numArray[index] >= 0)
                    {
                        if (owner.landFeats.Count > index)
                            owner.landFeats[index] = numArray[index];
                        else
                            owner.landFeats.Add(numArray[index]);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(FactionBranch), "OnClaimZone")]
        private class FactionBranchOnClaimZone : ModEntry.Entry
        {
            [Desc("在开局第一天获取土地产权时，同时获取如下物品，语法：id[\\[elem=>val[ ...]\\]][d|c|n|b|doomed|cursed|normal|BLESSED][:材质id[x数量[+等级]]][,...]", Neutron3529Enable.gt, 0.0)]
            private static string obtained = "waterPot:0,wateringCan:0,sickle:0,shovel:0,pickaxe:0,hammer:0,shears:0,hoe:0,fishingRod:0,axe:0,1085:75x100,tent1:42,tent2:42,wrench_extend_v:42x100,wrench_extend_h:42x100,1139:75,596:75,pillow_ehekatl:75,mic[241=>100]:29x1+9,1069[45=>2 77=>1000 78=>1000 79=>10000 402>=100 407>=10000 412>=100 416=>100 420=>100 421=>100 422>=100 423>=100 424>=100 425>=100 426>=100 427>=100 481=>10 483=>10 600=>10 601=>10 602=>10 603=>10 604=>10 605=>10 606=>10 607=>10 660=>10 661=>10 666=>10]:75x1+9,chest_tax:75,wagon_big5:29x2,wagon_big:29x2,container_compost:29,container_unburnable:75,container_burnable:75,generator:42x10,boat1[751=>99999 752=>99999]n+0:42x100,plat[]n+0:39x1000";
            private static Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item[] items = new Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item[0];
            [Desc("在开局第一天获取土地产权时，习得如下专长的特性（冒号指定等级，默认满级）", "never", 0.0)]
            private static string learnfeat = "1421,1510,1512,1514,1516,1518,1520,1522,1524,1610,1611,1612,1620,1621,1622,1623,1624,1625,1626,1627,1628,1629,1630,1631,1632,1633,1634,1635,1636,1640,1641,1642,1643,1645,1646,1647,1648,1649,1650,1651,1652,1653,1654,1655,1656,1657";
            [Desc("在开局第一天获取土地产权时，令如下专长的技能提升指定等级等级（冒号指定等级，默认可能是1级）", "never", 0.0)]
            private static string learnskill = "200:10000,207:10000,210:1000,220:1000,225:1000,226:1000,227:1000,230:1000,235:1000,237:1000,240:10000,241:1000,242:1000,245:1000,250:1000,255:1000,256:1000,257:1000,258:1000,259:1000,260:1000,261:1000,280:1000,281:1000,285:1000,286:1000,287:1000,288:1000,289:1000,290:1000,291:1000,292:1000,293:1000,300:1000,301:1000,302:1000,303:1000,304:1000,305:1000,306:1000,307:1000,6003:1000,6011:1000,6012:1000,6018:1000,6700:1000,6720:1000,6018:1000,6019:1000,6020:1000,6050:1000";
            private static ((int, int)[], (int, int)[]) learnfeats = (((int, int)[])null, ((int, int)[])null);

            public static (int, int) learn_id(string x)
            {
                try
                {
                    int result1;
                    if (int.TryParse(x, out result1))
                        return (result1, Math.Max(Element.Get(result1).textExtra.Split('\n', StringSplitOptions.None).Length, Element.Get(result1).textPhase.Split('\n', StringSplitOptions.None).Length));
                    int result2;
                    int result3;
                    if (x.Split(':', StringSplitOptions.None).Length > 1 && int.TryParse(x.Split(':', StringSplitOptions.None)[0], out result2) && int.TryParse(x.Split(':', StringSplitOptions.None)[1], out result3))
                        return (result2, result3);
                    int result4;
                    ModEntry.LogWarn(string.Format("learn_id = `{0}` 出错: x.Split(':').Length: {1} && int.TryParse(x.Split(':')[0], out var _): {2} && int.TryParse(x.Split(':')[1], out var _): {3}", (object)x, (object)(x.Split(':', StringSplitOptions.None).Length > 1), (object)int.TryParse(x.Split(':', StringSplitOptions.None)[0], out result4), (object)int.TryParse(x.Split(':', StringSplitOptions.None)[1], out result4)));
                }
                catch (Exception ex)
                {
                    ModEntry.LogWarn("learn_id = " + x + " 出错", ex);
                }
                return (-1, -1);
            }

            public override void Enable()
            {
                Neutron3529.Aux.Aux.FactionBranchOnClaimZone.items = ((IEnumerable<string>)Neutron3529.Aux.Aux.FactionBranchOnClaimZone.obtained.Split(',', StringSplitOptions.None)).Select<string, Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item>((Func<string, Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item>)(x1 =>
                {
                    try
                    {
                        string[] strArray1 = x1.Split(':', StringSplitOptions.None);
                        (int, int)[] valueTupleArray = new (int, int)[0];
                        BlessedState blessedState = (BlessedState)1;
                        Rarity rarity = (Rarity)3;
                        if (strArray1.Length != 0)
                        {
                            string[] strArray2 = strArray1[0].Split('[', StringSplitOptions.None);
                            if (strArray2.Length > 1)
                            {
                                strArray1[0] = strArray2[0];
                                string[] strArray3 = strArray2[1].Split(']', StringSplitOptions.None);
                                int result1;
                                int result2;
                                valueTupleArray = ((IEnumerable<string>)strArray3[0].Split(' ', StringSplitOptions.None)).Select<string, string[]>((Func<string, string[]>)(x2 => x2.Split("=>", StringSplitOptions.None))).Where<string[]>((Func<string[], bool>)(x3 => x3.Length > 1)).Select<string[], (int, int)>((Func<string[], (int, int)>)(x4 => !int.TryParse(x4[0], out result1) || !int.TryParse(x4[1], out result2) ? (-1, -1) : (result1, result2))).Where<(int, int)>((Func<(int, int), bool>)(x5 => x5.Item1 != -1)).ToArray<(int, int)>();
                                if (strArray3.Length != 0)
                                {
                                    string[] strArray4 = strArray3[1].ToLower().Split('+', StringSplitOptions.None);
                                    string str1 = strArray4[0].Trim();
                                    if (str1 != null)
                                    {
                                        switch (str1.Length)
                                        {
                                            case 0:
                                                goto label_15;
                                            case 1:
                                                switch (str1[0])
                                                {
                                                    case 'b':
                                                        goto label_15;
                                                    case 'c':
                                                        goto label_12;
                                                    case 'd':
                                                        break;
                                                    case 'n':
                                                        goto label_13;
                                                    default:
                                                        goto label_14;
                                                }
                                                break;
                                            case 6:
                                                switch (str1[0])
                                                {
                                                    case 'c':
                                                        if (str1 == "cursed")
                                                            goto label_12;
                                                        else
                                                            goto label_14;
                                                    case 'd':
                                                        if (str1 == "doomed")
                                                            break;
                                                        goto label_14;
                                                    case 'n':
                                                        if (str1 == "normal")
                                                            goto label_13;
                                                        else
                                                            goto label_14;
                                                    default:
                                                        goto label_14;
                                                }
                                                break;
                                            case 7:
                                                if (str1 == "blessed")
                                                    goto label_15;
                                                else
                                                    goto label_14;
                                            default:
                                                goto label_14;
                                        }
                                        blessedState = (BlessedState) - 2;
                                        goto label_15;
                                    label_12:
                                        blessedState = (BlessedState) - 1;
                                        goto label_15;
                                    label_13:
                                        blessedState = (BlessedState)0;
                                        goto label_15;
                                    }
                                label_14:
                                    ModEntry.LogWarn("暂时不支持设置物品祝福状态为`" + strArray4[0].Trim() + "`(错误来自语句" + x1 + ")，默认此物品状态为`BlessedState.Blessed`");
                                label_15:
                                    if (strArray4.Length > 1)
                                    {
                                        string str2 = strArray4[1].Trim();
                                        if (str2 != null)
                                        {
                                            switch (str2.Length)
                                            {
                                                case 1:
                                                    switch (str2[0])
                                                    {
                                                        case '0':
                                                            goto label_32;
                                                        case '1':
                                                            goto label_33;
                                                        case '2':
                                                            goto label_34;
                                                        case '3':
                                                            goto label_35;
                                                        case '4':
                                                            goto label_36;
                                                        default:
                                                            goto label_37;
                                                    }
                                                case 2:
                                                    if (str2 == "-1")
                                                        goto label_31;
                                                    else
                                                        goto label_37;
                                                case 4:
                                                    if (str2 == "-999")
                                                        break;
                                                    goto label_37;
                                                case 5:
                                                    if (str2 == "crude")
                                                        goto label_31;
                                                    else
                                                        goto label_37;
                                                case 6:
                                                    switch (str2[0])
                                                    {
                                                        case 'n':
                                                            if (str2 == "normal")
                                                                goto label_32;
                                                            else
                                                                goto label_37;
                                                        case 'r':
                                                            if (str2 == "random")
                                                                break;
                                                            goto label_37;
                                                        default:
                                                            goto label_37;
                                                    }
                                                    break;
                                                case 8:
                                                    switch (str2[0])
                                                    {
                                                        case 'A':
                                                            if (str2 == "Artifact")
                                                                goto label_36;
                                                            else
                                                                goto label_37;
                                                        case 'M':
                                                            if (str2 == "Mythical")
                                                                goto label_35;
                                                            else
                                                                goto label_37;
                                                        case 's':
                                                            if (str2 == "superior")
                                                                goto label_33;
                                                            else
                                                                goto label_37;
                                                        default:
                                                            goto label_37;
                                                    }
                                                case 9:
                                                    if (str2 == "Legendary")
                                                        goto label_34;
                                                    else
                                                        goto label_37;
                                                default:
                                                    goto label_37;
                                            }
                                            rarity = (Rarity) - 999;
                                            goto label_38;
                                        label_31:
                                            rarity = (Rarity) - 1;
                                            goto label_38;
                                        label_32:
                                            rarity = (Rarity)0;
                                            goto label_38;
                                        label_33:
                                            rarity = (Rarity)1;
                                            goto label_38;
                                        label_34:
                                            rarity = (Rarity)2;
                                            goto label_38;
                                        label_35:
                                            rarity = (Rarity)3;
                                            goto label_38;
                                        label_36:
                                            rarity = (Rarity)4;
                                            goto label_38;
                                        }
                                    label_37:
                                        ModEntry.LogWarn("暂时不支持设置物品稀有度为`" + strArray4[1].Trim() + "`(错误来自语句" + x1 + ")，默认此物品状态为`Rarity.Mythical`");
                                    }
                                }
                            }
                        }
                    label_38:
                        string[] strArray5;
                        if (strArray1.Length <= 1)
                            strArray5 = new string[2] { "0", "" };
                        else
                            strArray5 = strArray1[1].ToLower().Split(nameof(x), StringSplitOptions.None);
                        string[] strArray6 = strArray5;
                        int result3;
                        if (strArray6.Length > 1 && strArray6[1].Split('+', StringSplitOptions.None).Length > 1 && int.TryParse(strArray6[1].Split('+', StringSplitOptions.None)[1], out result3))
                        {
                            strArray6[1] = strArray6[1].Split('+', StringSplitOptions.None)[0];
                            int result4;
                            int result5;
                            return new Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item()
                            {
                                id = strArray1[0],
                                mat = int.TryParse(strArray6[0], out result4) ? result4 : 0,
                                count = strArray6.Length <= 1 || !int.TryParse(strArray6[1], out result5) ? 0 : result5 - 1,
                                enc = result3,
                                blessed = blessedState,
                                rarity = rarity,
                                elemap = valueTupleArray
                            };
                        }
                        int result6;
                        int result7;
                        return new Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item()
                        {
                            id = strArray1[0],
                            mat = int.TryParse(strArray6[0], out result6) ? result6 : 0,
                            count = strArray6.Length <= 1 || !int.TryParse(strArray6[1], out result7) ? 0 : result7 - 1,
                            enc = 0,
                            blessed = blessedState,
                            rarity = rarity,
                            elemap = valueTupleArray
                        };
                    }
                    catch (Exception ex)
                    {
                        ModEntry.LogWarn("忽视数据" + x1 + "处理错误，具体错因：", ex);
                        return (Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item)null;
                    }
                })).Where<Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item>((Func<Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item, bool>)(x => !string.IsNullOrEmpty(x.id))).ToArray<Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item>();
                base.Enable();
            }

            public override void InitDelayed()
            {
                // ISSUE: reference to a compiler-generated field
                Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learnfeats = (((IEnumerable<string>)Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learnfeat.Split(",", StringSplitOptions.None)).Select<string, (int, int)>(Neutron3529.Aux.Aux.FactionBranchOnClaimZone.\u003C\u003EO.\u003C0\u003E__learn_id ?? (Neutron3529.Aux.Aux.FactionBranchOnClaimZone.\u003C\u003EO.\u003C0\u003E__learn_id = new Func<string, (int, int)>(Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learn_id))).Where<(int, int)>((Func<(int, int), bool>)(x => x.Item2 != -1)).ToArray<(int, int)>(), ((IEnumerable<string>)Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learnskill.Split(",", StringSplitOptions.None)).Select<string, (int, int)>(Neutron3529.Aux.Aux.FactionBranchOnClaimZone.\u003C\u003EO.\u003C0\u003E__learn_id ?? (Neutron3529.Aux.Aux.FactionBranchOnClaimZone.\u003C\u003EO.\u003C0\u003E__learn_id = new Func<string, (int, int)>(Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learn_id))).Where<(int, int)>((Func<(int, int), bool>)(x => x.Item2 != -1)).ToArray<(int, int)>());
            }

            public static Thing Create(Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item x)
            {
                CardBlueprint.Set(new CardBlueprint()
                {
                    lv = 999,
                    rarity = x.rarity,
                    qualityBonus = 999,
                    blesstedState = new BlessedState?(x.blessed)
                });
                Thing thing = ThingGen.Create(x.id, x.mat, 999);
                if (x.enc > 0)
                    ((Card)thing).ModEncLv(x.enc);
                if (((Card)thing).c_IDTState != 0)
                    ((Card)thing).c_IDTState = 0;
                ElementContainerCard elements = ((Card)thing).elements;
                foreach ((int feat_id, int level) in x.elemap)
                {
                    if (feat_id == 45 && ((Card)thing).trait is TraitLightSource trait && ((Trait)trait).Params.Length > 1 && ((Trait)trait).GetParamInt(1, 0) > 0)
                        ((Trait)trait).Params[1] = string.Format("{0}", (object)(((Trait)trait).GetParamInt(1, 0) + level));
                    Neutron3529.Aux.Aux.FactionBranchOnClaimZone.EleModBase((ElementContainer)elements, feat_id, level, "obtain");
                }
                return thing;
            }

            public static void Postfix(FactionBranch __instance)
            {
                if (EClass.player.stats.days >= 2)
                    return;
                foreach (Neutron3529.Aux.Aux.FactionBranchOnClaimZone.item x in Neutron3529.Aux.Aux.FactionBranchOnClaimZone.items)
                {
                    Thing thing = Neutron3529.Aux.Aux.FactionBranchOnClaimZone.Create(x);
                    if (((Card)thing).trait.CanStack)
                    {
                        if (x.count > 0)
                            ((Card)thing).ModNum(x.count, true);
                        __instance.PutInMailBox(thing, true, false);
                    }
                    else
                    {
                        __instance.PutInMailBox(thing, true, false);
                        for (int index = 0; index < x.count; ++index)
                            __instance.PutInMailBox(Neutron3529.Aux.Aux.FactionBranchOnClaimZone.Create(x), true, false);
                    }
                    WidgetPopText.Say(ClassExtension.lang("popDeliver", ((Card)thing).Name, (string)null, (string)null, (string)null, (string)null), (FontColor)1, (Sprite)null);
                }
                Chara chara = EClass.player.chara;
                ElementContainerCard elements = ((Card)chara).elements;
                foreach ((int num1, int num2) in Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learnfeats.Item1)
                {
                    Element element = ((ElementContainer)elements).GetElement(num1);
                    for (int index = (element != null ? element.vBase : 0) + 1; index <= num2; ++index)
                        chara.SetFeat(num1, index, index != num2);
                }
                foreach ((int feat_id, int level) in Neutron3529.Aux.Aux.FactionBranchOnClaimZone.learnfeats.Item2)
                    Neutron3529.Aux.Aux.FactionBranchOnClaimZone.EleModBase((ElementContainer)elements, feat_id, level, "learnskill");
                chara.HealAll();
            }

            private static void EleModBase(ElementContainer ele, int feat_id, int level, string desc = "EleModBase")
            {
                if (ele.GetOrCreateElement(feat_id) != null)
                    ele.ModBase(feat_id, level);
                else
                    ModEntry.LogWarn(string.Format("{0} 项 {1}:{2} 无效", (object)desc, (object)feat_id, (object)level));
            }

            public class item
            {
                public string id;
                public int mat;
                public int count = 1;
                public int enc;
                public BlessedState blessed = (BlessedState)1;
                public Rarity rarity = (Rarity)3;
                public (int, int)[] elemap = new (int, int)[0];
            }
        }

        [Desc("干渴之壶汲水速度最大化", Neutron3529Enable.gt, 0.0)]
        [HarmonyPatch(typeof(TaskDrawWater), "OnCreateProgress")]
        private class TaskDrawWaterOnCreateProgress : ModEntry.Entry
        {
            public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
        }

        [Desc("干渴之壶倒水速度最大化", Neutron3529Enable.gt, 0.0)]
        [HarmonyPatch(typeof(TaskPourWater), "OnCreateProgress")]
        private class TaskPourWaterOnCreateProgress : ModEntry.Entry
        {
            public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
        }

        [Desc("采集速度最大化", Neutron3529Enable.gt, 0.0)]
        [HarmonyPatch(typeof(TaskHarvest), "OnCreateProgress")]
        private class TaskHarvestOnCreateProgress : ModEntry.Entry
        {
            public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
        }

        [Desc("挖掘速度最大化", Neutron3529Enable.gt, 0.0)]
        [HarmonyPatch(typeof(TaskDig), "OnCreateProgress")]
        private class TaskDigOnCreateProgress : ModEntry.Entry
        {
            public static void Postfix(Progress_Custom p) => p.interval = p.maxProgress = 1;
        }
    }
}
