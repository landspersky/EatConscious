using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EatConscious.Models;

namespace EatConscious.Wrappers;

public class MealPlanWrapper : IWrapper<MealPlanWrapper>
{
    public List<MealPlanEntryStripped> Entries { get; init; } = new();

    public static MealPlanWrapper StateOnLoad { get; } = Deserialize();

    private static MealPlanWrapper Deserialize()
    {
        MealPlanWrapper output;
        try
        {
            string mealPlanJson = File.ReadAllText(App.MealPlanPath);
            output = JsonSerializer.Deserialize<MealPlanWrapper>(mealPlanJson) ?? new();
        }
        catch (Exception)
        {
            output = new();
        }

        return output;
    }

    public class MealPlanEntryStripped
    {
        public DateOnly Date { get; init; }
        public int RecipeId { get; init; }
    }
}

public static class MealPlanExtensions
{
    public static MealPlanWrapper.MealPlanEntryStripped Strip(this MealPlanEntry e) => new()
    {
        Date = e.Date,
        RecipeId = e.Recipe.Id,
    };
}
