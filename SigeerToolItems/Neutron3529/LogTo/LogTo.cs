using Neutron3529.Utils;
namespace Neutron3529.Aux;

[Desc("信息计算-种族表（输出到log）", -1.0)]
public class Info_Race : ModEntry.Entry {
    public override void InitDelayed() {
        base.InitDelayed();
        ModEntry.logger("种族表:");
        Neutron3529.Aux.Aux.Dumping<SourceRace.Row>((IEnumerable<SourceRace.Row>)((SourceData<SourceRace.Row, string>)EClass.sources.races).rows, (Type[])null, (string[])null, ("id", (Func<SourceRace.Row, object>)(x => (object)x.id)), ("name", (Func<SourceRace.Row, object>)(x => (object)((SourceData.BaseRow)x).GetName())), ("detail", (Func<SourceRace.Row, object>)(x => (object)((SourceData.BaseRow)x).GetDetail())));
    }

    public override void Enable() {
    }
}

[Desc("信息计算-材料等级表（输出到log）", -1.0)]
public class Info_MaterialTier : ModEntry.Entry {
    public override void InitDelayed() {
        base.InitDelayed();
        ModEntry.logger("材料等级表:");
        foreach (KeyValuePair<string, SourceMaterial.TierList> tier1 in SourceMaterial.tierMap) {
            ModEntry.logger("Group " + tier1.Key + ":");
            int num = 0;
            foreach (SourceMaterial.Tier tier2 in tier1.Value.tiers)
                ModEntry.logger(string.Format("    {0}: sum = {1}, list = {2}", (object)num++, (object)tier2.sum, (object)Neutron3529.Aux.Aux.JoinList<string>(tier2.list.Select<SourceMaterial.Row, string>((Func<SourceMaterial.Row, string>)(x => x.name_L)))));
        }
    }

    public override void Enable() {
    }
}

[Desc("信息计算-材料表（输出到log）", -1.0)]
public class Info_Material : ModEntry.Entry {
    public override void InitDelayed() {
        base.InitDelayed();
        ModEntry.logger("材料表:");
        Neutron3529.Aux.Aux.Dumping<SourceMaterial.Row>((IEnumerable<SourceMaterial.Row>)((SourceData<SourceMaterial.Row, int>)EClass.sources.materials).rows, (Type[])null, (string[])null, ("id", (Func<SourceMaterial.Row, object>)(x => (object)x.id.ToString())), ("name", (Func<SourceMaterial.Row, object>)(x => (object)((SourceData.BaseRow)x).GetName())), ("detail", (Func<SourceMaterial.Row, object>)(x => (object)((SourceData.BaseRow)x).GetDetail())));
    }

    public override void Enable() {
    }
}

[Desc("信息计算-配方表与锤炼法（输出到log）", -1.0)]
public class Info_Recipe : ModEntry.Entry {
    public static string IngName(Recipe.Ingredient ing) {
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
            str = "," + Neutron3529.Aux.Aux.JoinList<string>((IEnumerable<string>)ing.idOther, new ModEntry.Dump() {
                left = "(可用",
                right = "代替)"
            });
        strArray[3] = str;
        strArray[4] = ")";
        return string.Concat(strArray);
    }

    public static string IngNameExact(string id, Recipe.Ingredient ing) {
        return (ing.useCat ? ClassExtension.lang("ingCat", ((SourceData.BaseRow)((SourceData<SourceCategory.Row, string>)EClass.sources.categories).map[id]).GetName(), (string)null, (string)null, (string)null, (string)null) : ClassExtension.lang(ClassExtension.IsEmpty(((SourceData.BaseRow)EClass.sources.cards.map[id]).GetName(), "card_" + id)) + (ClassExtension.IsEmpty(ing.tag) ? "" : "(" + ClassExtension.lang("tag_" + ing.tag) + ")")) ?? "";
    }


    public override void InitDelayed() {
        base.InitDelayed();
        LogRecipeData();
        LogAlternativeRecipeTable();
    }

    public void LogRecipeData() {
        const string header = "配方表:";
        ModEntry.logger(header);

        var recipeDetails = new StringBuilder();
        RecipeManager.BuildList();

        foreach (var recipeEntry in RecipeManager.dict) {
            var recipe = recipeEntry.Value;
            var ingredients = recipe.GetIngredients();

            if (ingredients == null || ingredients.Count == 0) continue;

            // 记录主配方信息
            LogMainRecipe(recipeEntry.Key, recipe, ingredients);

            // 处理替代材料
            ProcessAlternativeMaterials(ingredients, recipe, recipeDetails);
        }
        LogForgingMethods(recipeDetails);
    }

    public void LogMainRecipe(string recipeKey, RecipeSource recipe, List<Recipe.Ingredient> ingredients) {
        var factoryList = JoinList(recipe.row.factory);
        var recipeFields = EvalFields(recipe, "type", "id", "colorIng", "isBridge", "isBridgePillar", "isChara", "noListing", "isRandom", "alwaysKnown");
        var ingredientDetails = ingredients.Select(FormatIngredient);

        ModEntry.logger($"{recipe.Name}({recipeKey})\t" +
                        $"NeedFactory:{recipe.NeedFactory}\t" +
                        $"Factory:{factoryList}\t" +
                        $"{recipeFields}\t" +
                        $"GetIngredients:{string.Join(",\t", ingredientDetails)}");
    }

    public string FormatIngredient(Recipe.Ingredient ing) {
        var ingFields = EvalFields(ing, "uid", "req", "mat", "refVal", "id", "tag", "optional", "dye", "useCat");
        return $"{Info_Recipe.IngName(ing)}\t{ingFields}";
    }

    public void ProcessAlternativeMaterials(List<Recipe.Ingredient> ingredients, RecipeSource recipe, StringBuilder detailsBuilder) {
        var primaryIngredient = ingredients.First();
        if (primaryIngredient.idOther?.Count > 0) {
            foreach (var altId in primaryIngredient.idOther) {
                var optionalIngredients = ingredients.Skip(1)
                    .Where(x => !x.optional)
                    .Select(x => $"+ {Info_Recipe.IngName(x)}x{x.req} ");

                detailsBuilder.AppendLine(
                    $"from:({Info_Recipe.IngNameExact(altId, primaryIngredient)})x{primaryIngredient.req} " +
                    $"{JoinList(optionalIngredients, new ModEntry.Dump { sep = "" }) ?? ""} " +
                    $"to:{Info_Recipe.IngNameExact(primaryIngredient.id, primaryIngredient)} " +
                    $"with:{recipe.Name}"
                );
            }
        }
    }

    public void LogAlternativeRecipeTable() {
        ModEntry.logger("配方表（另表）:");

        foreach (var row in EClass.sources.recipes.rows.Cast<SourceRecipe.Row>()) {
            var cardName = EClass.sources.cards.map.TryGetValue(row.thing, out var cardRow)
                ? ClassExtension.lang(ClassExtension.IsEmpty(cardRow.GetName(), $"card_{row.thing}"))
                : ClassExtension.lang($"card_{row.thing}");

            var rowFields = EvalFields(row, "id", "factory", "type", "num", "sp", "time");
            var ings = string.Join("\t",
                JoinList(row.ing1),
                JoinList(row.ing2),
                JoinList(row.ing3)
            );

            ModEntry.logger($"{cardName}({row.thing}):\t{rowFields}\t{ings}\t{row.detail_L}");
        }
    }

    public void LogForgingMethods(StringBuilder detailsBuilder) {
        ModEntry.logger($"锤炼法:\n{detailsBuilder}");
    }

    // Helper methods
    public static string JoinList<T>(IEnumerable<T> list, ModEntry.Dump dump = null) =>
        Neutron3529.Aux.Aux.JoinList(list, dump ?? ModEntry.Dump.instance);

    public static string EvalFields(object obj, params string[] fields) =>
        Neutron3529.Aux.Aux.EvalFields(obj, true, '\t', fields);

    public override void Enable() {
    }
}

[Desc("信息计算-SourceCard(things/charas)", -1.0)]
public class Info_SourceCard : ModEntry.Entry {
    public override void InitDelayed() {
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

    public override void Enable() {
    }
}

[Desc("信息计算-特典代码（输出到log）", -1.0)]
public class Info_ElinEncoder : ModEntry.Entry {
    public override void Enable() {
    }

    public override void InitDelayed() {
        ModEntry.logger("特典代码: " + ElinEncoder.AesEncrypt("2504360").TrimEnd('='));
        ModEntry.logger("特典解码: " + ElinEncoder.AesDecrypt("/RR9CuHv6JQ="));
    }
}

