using EatConscious.Models;
using EatConscious.ViewModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace EatConscious.Tests;

public class CalendarDayViewModelTests
{
    private static readonly DateOnly Day = new(2026, 9, 15);

    private static Recipe RecipeWith(double kcal, double protein, double carbs, double fats, double grams) => new()
    {
        Name = "R",
        Ingredients = new List<IngredientPortion>
        {
            new()
            {
                Ingredient = new Ingredient
                {
                    Name = "I",
                    Unit = Measure.Gram,
                    Nutrients = new Nutrients { Kcal = kcal, Protein = protein, Carbs = carbs, Fats = fats },
                },
                Value = grams,
            },
        },
    };

    private static CalendarDayViewModel DayWith(params Recipe[] recipes)
    {
        var entries = new List<MealPlanEntry>();
        foreach (var recipe in recipes)
        {
            entries.Add(new MealPlanEntry { Date = Day, Recipe = recipe });
        }

        return new CalendarDayViewModel(Day, true, entries, (_, _) => { }, _ => { });
    }

    [Fact]
    public void TotalNutrients_SumsAllEntriesOfTheDay()
    {
        var day = DayWith(RecipeWith(100, 10, 20, 5, 200), RecipeWith(400, 2, 4, 30, 50));

        Assert.Equal(400, day.TotalNutrients.Kcal);
        Assert.Equal(21, day.TotalNutrients.Protein);
        Assert.Equal(42, day.TotalNutrients.Carbs);
        Assert.Equal(25, day.TotalNutrients.Fats);
        Assert.True(day.HasEntries);
    }

    [Fact]
    public void TotalNutrients_IsZeroForEmptyDay()
    {
        var day = DayWith();

        Assert.Equal(new Nutrients(), day.TotalNutrients);
        Assert.False(day.HasEntries);
    }

    [Fact]
    public void VisibleEntries_ShowsAllWhenThreeOrFewer()
    {
        var day = DayWith(RecipeWith(1, 1, 1, 1, 1), RecipeWith(1, 1, 1, 1, 1), RecipeWith(1, 1, 1, 1, 1));

        Assert.Equal(3, day.VisibleEntries.Count);
        Assert.False(day.HasMoreEntries);
    }

    [Fact]
    public void VisibleEntries_CapsAtThreeWhenMore()
    {
        var day = DayWith(RecipeWith(1, 1, 1, 1, 1), RecipeWith(1, 1, 1, 1, 1),
            RecipeWith(1, 1, 1, 1, 1), RecipeWith(1, 1, 1, 1, 1));

        Assert.Equal(3, day.VisibleEntries.Count);
        Assert.True(day.HasMoreEntries);
    }
}
