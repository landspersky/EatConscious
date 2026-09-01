using System;
using EatConscious.Models;
using EatConscious.Wrappers;
using Xunit;

namespace EatConscious.Tests;

public class MealPlanWrapperTests
{
    [Fact]
    public void Strip_CopiesDateAndRecipeId()
    {
        var recipe = new Recipe { Id = 7, Name = "Soup" };
        var entry = new MealPlanEntry { Date = new DateOnly(2026, 9, 15), Recipe = recipe };

        var stripped = entry.Strip();

        Assert.Equal(new DateOnly(2026, 9, 15), stripped.Date);
        Assert.Equal(7, stripped.RecipeId);
    }
}
