using Neutron3529.Utils;
namespace Neutron3529.Aux;

[HarmonyPatch(typeof(FactionBranch), nameof(FactionBranch.OnClaimZone))]
public class FactionBranchOnClaimZone_FEAT : ModEntry.Entry {
    [Desc("地区第一特性id", "ge", 0.0)]
    public static int zone1 = 3603;
    [Desc("地区第二特性id", "ge", 0.0)]
    public static int zone2 = 3709;
    [Desc("地区第三特性id", "ge", 0.0)]
    public static int zone3 = 3900;

    public static void Prefix(FactionBranch __instance) {
        Zone owner = __instance.owner;
        if (owner.landFeats == null)
            owner.ListLandFeats();
        int[] numArray = new int[3]
        {
          zone1,
          zone2,
          zone3
        };
        for (int index = 0; index < 3; ++index) {
            if (numArray[index] >= 0) {
                if (owner.landFeats.Count > index)
                    owner.landFeats[index] = numArray[index];
                else
                    owner.landFeats.Add(numArray[index]);
            }
        }
    }
}

[HarmonyPatch(typeof(FactionBranch), nameof(FactionBranch.OnClaimZone))]
public class FactionBranchOnClaimZone : ModEntry.Entry {
    [Desc("在开局第一天获取土地产权时，同时获取如下物品，语法：id[\\[elem=>val[ ...]\\]][d|c|n|b|doomed|cursed|normal|BLESSED][:材质id[x数量[+等级]]][,...]", Neutron3529Enable.gt, 0.0)]
    public static string obtained = "waterPot:0,wateringCan:0,sickle:0,shovel:0,pickaxe:0,hammer:0,shears:0,hoe:0,fishingRod:0,axe:0,1085:75x100,tent1:42,tent2:42,wrench_extend_v:42x100,wrench_extend_h:42x100,1139:75,596:75,pillow_ehekatl:75,mic[241=>100]:29x1+9,1069[45=>2 77=>1000 78=>1000 79=>10000 402>=100 407>=10000 412>=100 416=>100 420=>100 421=>100 422>=100 423>=100 424>=100 425>=100 426>=100 427>=100 481=>10 483=>10 600=>10 601=>10 602=>10 603=>10 604=>10 605=>10 606=>10 607=>10 660=>10 661=>10 666=>10]:75x1+9,chest_tax:75,wagon_big5:29x2,wagon_big:29x2,container_compost:29,container_unburnable:75,container_burnable:75,generator:42x10,boat1[751=>99999 752=>99999]n+0:42x100,plat[]n+0:39x1000";
    public static item[] items = new item[0];
    [Desc("在开局第一天获取土地产权时，习得如下专长的特性（冒号指定等级，默认满级）", "never", 0.0)]
    public static string learnfeat = "1421,1510,1512,1514,1516,1518,1520,1522,1524,1610,1611,1612,1620,1621,1622,1623,1624,1625,1626,1627,1628,1629,1630,1631,1632,1633,1634,1635,1636,1640,1641,1642,1643,1645,1646,1647,1648,1649,1650,1651,1652,1653,1654,1655,1656,1657";
    [Desc("在开局第一天获取土地产权时，令如下专长的技能提升指定等级等级（冒号指定等级，默认可能是1级）", "never", 0.0)]
    public static string learnskill = "200:10000,207:10000,210:1000,220:1000,225:1000,226:1000,227:1000,230:1000,235:1000,237:1000,240:10000,241:1000,242:1000,245:1000,250:1000,255:1000,256:1000,257:1000,258:1000,259:1000,260:1000,261:1000,280:1000,281:1000,285:1000,286:1000,287:1000,288:1000,289:1000,290:1000,291:1000,292:1000,293:1000,300:1000,301:1000,302:1000,303:1000,304:1000,305:1000,306:1000,307:1000,6003:1000,6011:1000,6012:1000,6018:1000,6700:1000,6720:1000,6018:1000,6019:1000,6020:1000,6050:1000";
    public static ((int, int)[], (int, int)[]) learnfeats = (((int, int)[])null, ((int, int)[])null);

    public static (int, int) learn_id(string x) {
        try {
            int result1;
            if (int.TryParse(x, out result1))
                return (result1, Math.Max(Element.Get(result1).textExtra.Split('\n').Length, Element.Get(result1).textPhase.Split('\n').Length));
            int result2;
            int result3;
            if (x.Split(':').Length > 1 && int.TryParse(x.Split(':')[0], out result2) && int.TryParse(x.Split(':')[1], out result3))
                return (result2, result3);
            int result4;
            ModEntry.LogWarn(string.Format("learn_id = `{0}` 出错: x.Split(':').Length: {1} && int.TryParse(x.Split(':')[0], out var _): {2} && int.TryParse(x.Split(':')[1], out var _): {3}", (object)x, (object)(x.Split(':').Length > 1), (object)int.TryParse(x.Split(':')[0], out result4), (object)int.TryParse(x.Split(':')[1], out result4)));
        }
        catch (Exception ex) {
            ModEntry.LogWarn("learn_id = " + x + " 出错", ex);
        }
        return (-1, -1);
    }
    public override void Enable() {
        items = ParseItems();
        base.Enable();
    }

    public static item[] ParseItems() {
        var blessedStateMap = new Dictionary<string, BlessedState>(StringComparer.OrdinalIgnoreCase) {
            ["b"] = BlessedState.Blessed,
            ["blessed"] = BlessedState.Blessed,
            ["c"] = BlessedState.Cursed,
            ["cursed"] = BlessedState.Cursed,
            ["d"] = BlessedState.Doomed,
            ["doomed"] = BlessedState.Doomed,
            ["n"] = BlessedState.Normal,
            ["normal"] = BlessedState.Normal
        };

        var rarityMap = new Dictionary<string, Rarity>(StringComparer.OrdinalIgnoreCase) {
            ["-1"] = Rarity.Random,
            ["crude"] = Rarity.Crude,
            ["0"] = Rarity.Normal,
            ["1"] = Rarity.Superior,
            ["superior"] = Rarity.Superior,
            ["2"] = Rarity.Legendary,
            ["legendary"] = Rarity.Legendary,
            ["3"] = Rarity.Mythical,
            ["mythical"] = Rarity.Mythical,
            ["4"] = Rarity.Artifact,
            ["artifact"] = Rarity.Artifact
        };

        return obtained
            .Split(',')
            .Select(ParseSingleItem)
            .Where(item => item != null && !string.IsNullOrEmpty(item.id))
            .ToArray();

        item ParseSingleItem(string input) {
            try {
                var parts = input.Split(':');
                if (parts.Length == 0) return null;

                var mainSegment = parts[0];
                var bracketIndex = mainSegment.IndexOf('[');
                var (itemId, elementMappings, blessedState, rarity) = ParseMainSegment(mainSegment, bracketIndex);

                var (material, count, enc) = ParseParameters(parts.Length > 1 ? parts[1] : "0");

                return new item {
                    id = itemId,
                    mat = material,
                    count = Math.Max(0, count - 1),
                    enc = enc,
                    blessed = blessedState,
                    rarity = rarity,
                    elemap = elementMappings
                };
            }
            catch (Exception ex) {
                ModEntry.LogWarn($"处理数据失败: {input}", ex);
                return null;
            }
        }

        (string itemId, (int, int)[] elementMappings, BlessedState blessedState, Rarity rarity) ParseMainSegment(string segment, int bracketIndex) {
            var itemId = segment;
            var elementMappings = Array.Empty<(int, int)>();
            var blessedState = BlessedState.Blessed;
            var rarity = Rarity.Mythical;

            if (bracketIndex <= 0) return (itemId, elementMappings, blessedState, rarity);

            // 解析元素映射
            itemId = segment[..bracketIndex];

            var bracketContent = segment[(bracketIndex + 1)..].Split(']')[0];
            elementMappings = bracketContent.Split(' ')
                .Select(x => x.Split("=>"))
                .Where(pair => pair.Length == 2)
                .Select(pair => (
                    int.TryParse(pair[0], out var k) ? k : -1,
                    int.TryParse(pair[1], out var v) ? v : -1
                ))
                .Where(t => t.Item1 != -1 && t.Item2 != -1)
                .ToArray();

            // 解析状态和稀有度
            var modifiers = segment.Split(']').Last().Split('+');
            if (modifiers.Length == 0) return (itemId, elementMappings, blessedState, rarity);

            // 解析祝福状态
            if (modifiers.Length > 0 && !string.IsNullOrWhiteSpace(modifiers[0])) {
                var stateKey = modifiers[0].Trim().ToLower();
                if (!blessedStateMap.TryGetValue(stateKey, out blessedState)) {
                    ModEntry.LogWarn($"无效的祝福状态: {stateKey}，使用默认值");
                }
            }

            // 解析稀有度
            if (modifiers.Length > 1 && !string.IsNullOrWhiteSpace(modifiers[1])) {
                var rarityKey = modifiers[1].Trim().ToLower();
                if (!rarityMap.TryGetValue(rarityKey, out rarity)) {
                    ModEntry.LogWarn($"无效的稀有度: {rarityKey}，使用默认值");
                }
            }

            return (itemId, elementMappings, blessedState, rarity);
        }

        (int material, int count, int enc) ParseParameters(string paramStr) {
            var paramParts = paramStr.ToLower().Split('x');
            var material = 0;
            var count = 0;
            var enc = 0;

            if (paramParts.Length > 0) {
                int.TryParse(paramParts[0], out material);
            }

            if (paramParts.Length > 1) {
                var countParts = paramParts[1].Split('+');
                if (countParts.Length > 0) {
                    int.TryParse(countParts[0], out count);
                }
                if (countParts.Length > 1) {
                    int.TryParse(countParts[1], out enc);
                }
            }

            return (material, count, enc);
        }
    }

    public override void InitDelayed() {
        // 初始化静态委托（如果必须保持编译器生成的逻辑）
        var parser = learn_id;

        learnfeats = (
            ParseEntries(learnfeat, parser),
            ParseEntries(learnskill, parser)
        );
    }

    public static (int, int)[] ParseEntries(string input, Func<string, (int, int)> parser) {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<(int, int)>();

        return input.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(parser)
                   .Where(x => x.Item2 != -1)
                   .ToArray();
    }

    public static Thing Create(item x) {
        CardBlueprint.Set(new CardBlueprint() {
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
        foreach ((int feat_id, int level) in x.elemap) {
            if (feat_id == 45 && ((Card)thing).trait is TraitLightSource trait && ((Trait)trait).Params.Length > 1 && ((Trait)trait).GetParamInt(1, 0) > 0)
                ((Trait)trait).Params[1] = string.Format("{0}", (object)(((Trait)trait).GetParamInt(1, 0) + level));
            EleModBase((ElementContainer)elements, feat_id, level, "obtain");
        }
        return thing;
    }

    public static void Postfix(FactionBranch __instance) {
        if (EClass.player.stats.days >= 2)
            return;
        foreach (item x in items) {
            Thing thing = Create(x);
            if (((Card)thing).trait.CanStack) {
                if (x.count > 0)
                    ((Card)thing).ModNum(x.count, true);
                __instance.PutInMailBox(thing, true, false);
            }
            else {
                __instance.PutInMailBox(thing, true, false);
                for (int index = 0; index < x.count; ++index)
                    __instance.PutInMailBox(Create(x), true, false);
            }
            WidgetPopText.Say(ClassExtension.lang("popDeliver", ((Card)thing).Name, (string)null, (string)null, (string)null, (string)null), (FontColor)1, (Sprite)null);
        }
        Chara chara = EClass.player.chara;
        ElementContainerCard elements = ((Card)chara).elements;
        foreach ((int num1, int num2) in learnfeats.Item1) {
            Element element = ((ElementContainer)elements).GetElement(num1);
            for (int index = (element != null ? element.vBase : 0) + 1; index <= num2; ++index)
                chara.SetFeat(num1, index, index != num2);
        }
        foreach ((int feat_id, int level) in learnfeats.Item2)
            EleModBase((ElementContainer)elements, feat_id, level, "learnskill");
        chara.HealAll();
    }

    public static void EleModBase(ElementContainer ele, int feat_id, int level, string desc = "EleModBase") {
        if (ele.GetOrCreateElement(feat_id) != null)
            ele.ModBase(feat_id, level);
        else
            ModEntry.LogWarn(string.Format("{0} 项 {1}:{2} 无效", (object)desc, (object)feat_id, (object)level));
    }

    public class item {
        public string id;
        public int mat;
        public int count = 1;
        public int enc;
        public BlessedState blessed = (BlessedState)1;
        public Rarity rarity = (Rarity)3;
        public (int, int)[] elemap = new (int, int)[0];
    }
}
