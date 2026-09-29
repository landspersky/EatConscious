using System;
using System.Collections.Generic;
using System.Linq;
using EatConscious.Models;
using EatConscious.ViewModels;
using Xunit;

namespace EatConscious.Tests;

/// <summary>
/// Colleagues only talk to the mediator, so a fake one is enough to test them
/// </summary>
public class ColleagueTests
{
    private readonly FakeMediator _mediator = new();

    private static Ingredient IngredientWith(int id) => new() { Id = id, Name = $"I{id}", Unit = Measure.Gram };

    private static Recipe RecipeWith(int id, params Ingredient[] ingredients) => new()
    {
        Id = id,
        Name = $"R{id}",
        Ingredients = ingredients.Select(x => new IngredientPortion { Ingredient = x, Value = 100 }).ToList(),
    };

    [Fact]
    public void IngredientsTab_ReportsCheckChange()
    {
        var onion = IngredientWith(1);
        var tab = new IngredientsTabViewModel(_mediator, new[] { onion }, new List<string>());

        onion.IsChecked = true;
        tab.OnCheckCommand.Execute(onion).Subscribe();

        Assert.Equal(new[] { (onion, true) }, _mediator.CheckChanges);
    }

    [Fact]
    public void IngredientsTab_DeletesAndReports()
    {
        var onion = IngredientWith(1);
        var tab = new IngredientsTabViewModel(_mediator, new[] { onion }, new List<string>());

        tab.DeleteCommand.Execute(onion).Subscribe();

        Assert.Empty(tab.AllIngredients);
        Assert.Equal(new[] { onion }, _mediator.DeletedIngredients);
    }

    [Fact]
    public void IngredientsTab_AsksMediatorToOpenEditor()
    {
        var onion = IngredientWith(1);
        var tab = new IngredientsTabViewModel(_mediator, new[] { onion }, new List<string>());

        tab.AddCommand.Execute().Subscribe();
        tab.EditCommand.Execute(onion).Subscribe();

        Assert.Equal(new[] { null, onion }, _mediator.OpenedIngredientEditors);
    }

    [Fact]
    public void RecipesTab_DeletesAndReports()
    {
        var soup = RecipeWith(1);
        var tab = new RecipesTabViewModel(_mediator, new[] { soup }, new List<string>());

        tab.DeleteCommand.Execute(soup).Subscribe();

        Assert.Empty(tab.AllRecipes);
        Assert.Equal(new[] { soup }, _mediator.DeletedRecipes);
    }

    [Fact]
    public void RecipesTab_RemovingIngredient_AnnouncesOnlyAffectedRecipes()
    {
        var onion = IngredientWith(1);
        var carrot = IngredientWith(2);
        var tab = new RecipesTabViewModel(_mediator, new[] { RecipeWith(1, onion, carrot), RecipeWith(2, carrot) },
            new List<string>());

        tab.RemoveIngredient(onion);

        var saved = Assert.Single(_mediator.SavedRecipes);
        Assert.Equal(1, saved.Id);
        Assert.Equal(new[] { carrot }, saved.Ingredients.Select(x => x.Ingredient));
    }

    [Fact]
    public void Calendar_PickerUsesMediatorRecipes()
    {
        var soup = RecipeWith(1);
        _mediator.Recipes.Add(soup);
        var calendar = new CalendarViewModel(_mediator, new List<MealPlanEntry>());

        Assert.Equal(new[] { soup }, calendar.Recipes);
    }

    [Fact]
    public void IngredientEditor_ReportsSavedIngredientWithSameId()
    {
        var onion = IngredientWith(5);
        var editor = new NewIngredientViewModel(_mediator, onion) { Name = "Red onion" };

        editor.ButtonClick();

        var saved = Assert.Single(_mediator.SavedIngredients);
        Assert.Equal(5, saved.Id);
        Assert.Equal("Red onion", saved.Name);
    }

    [Fact]
    public void RecipeEditor_OffersAllIngredientsAndKeepsPortions()
    {
        var onion = IngredientWith(1);
        var carrot = IngredientWith(2);
        _mediator.Ingredients.Add(onion);
        _mediator.Ingredients.Add(carrot);
        var soup = RecipeWith(3, onion);
        var editor = new NewRecipeViewModel(_mediator, soup);

        Assert.Equal(2, editor.Ingredients.Count);
        editor.ButtonClick();

        var saved = Assert.Single(_mediator.SavedRecipes);
        Assert.Equal(3, saved.Id);
        var portion = Assert.Single(saved.Ingredients);
        Assert.Same(onion, portion.Ingredient);
        Assert.Equal(100, portion.Value);
    }
}
