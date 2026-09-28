using System;
using System.Linq;
using EatConscious.Models;
using EatConscious.ViewModels;
using Xunit;

namespace EatConscious.Tests;

/// <summary>
/// The concrete mediator routing between the real tabs
/// </summary>
/// <remarks>No data files exist in the test directory, so the tabs start empty</remarks>
public class MainMediatorTests
{
    private static readonly DateOnly Day = new(2026, 9, 15);

    private static Ingredient IngredientWith(int id, double kcal = 0) => new()
    {
        Id = id, Name = $"I{id}", Unit = Measure.Gram, Nutrients = new Nutrients { Kcal = kcal },
    };

    private static Recipe RecipeWith(int id, params Ingredient[] ingredients) => new()
    {
        Id = id,
        Name = $"R{id}",
        Ingredients = ingredients.Select(x => new IngredientPortion { Ingredient = x, Value = 100 }).ToList(),
    };

    private readonly MainWindowViewModel _main = new();
    private readonly Ingredient _onion = IngredientWith(1);
    private readonly Ingredient _carrot = IngredientWith(2);
    private readonly Recipe _onionSoup;
    private readonly Recipe _carrotSalad;

    public MainMediatorTests()
    {
        _onionSoup = RecipeWith(1, _onion);
        _carrotSalad = RecipeWith(2, _carrot);
        _main.IngredientsTab.AddOrUpdate(_onion);
        _main.IngredientsTab.AddOrUpdate(_carrot);
        _main.RecipesTab.AddOrUpdate(_onionSoup);
        _main.RecipesTab.AddOrUpdate(_carrotSalad);
    }

    private void Check(Ingredient ingredient, bool isChecked)
    {
        // the check box binding sets IsChecked before the command runs
        ingredient.IsChecked = isChecked;
        _main.IngredientsTab.OnCheckCommand.Execute(ingredient).Subscribe();
    }

    [Fact]
    public void CheckingIngredient_FiltersRecipesInRecipesTab()
    {
        Check(_onion, true);

        Assert.Equal(new[] { _onionSoup }, _main.RecipesTab.Recipes);

        Check(_onion, false);

        Assert.Equal(2, _main.RecipesTab.Recipes.Count);
    }

    [Fact]
    public void CalendarPicker_OffersAllRecipesDespiteFilter()
    {
        Check(_onion, true);

        Assert.Equal(2, _main.Calendar.Recipes.Count);
    }

    [Fact]
    public void DeletingRecipe_RemovesItsCalendarEntries()
    {
        _main.Calendar.AddMealPlanEntry(Day, _onionSoup);
        _main.Calendar.AddMealPlanEntry(Day.AddDays(1), _onionSoup);
        _main.Calendar.AddMealPlanEntry(Day, _carrotSalad);

        _main.RecipesTab.DeleteCommand.Execute(_onionSoup).Subscribe();

        Assert.Equal(new[] { _carrotSalad }, _main.Calendar.MealPlan.Select(x => x.Recipe));
        Assert.DoesNotContain(_onionSoup, _main.AllRecipes);
    }

    [Fact]
    public void SavingRecipe_UpdatesItsCalendarEntries()
    {
        _main.Calendar.AddMealPlanEntry(Day, _onionSoup);
        var edited = RecipeWith(_onionSoup.Id, _onion, _carrot);

        _main.RecipeSaved(edited);

        Assert.Same(edited, _main.Calendar.MealPlan.Single().Recipe);
        Assert.Same(edited, _main.AllRecipes.Single(x => x.Id == edited.Id));
    }

    [Fact]
    public void SavingIngredient_UpdatesRecipesAndCalendarUsingIt()
    {
        _main.Calendar.AddMealPlanEntry(Day, _onionSoup);
        var edited = IngredientWith(_onion.Id, kcal: 50);

        _main.IngredientSaved(edited);

        var soup = _main.AllRecipes.Single(x => x.Id == _onionSoup.Id);
        Assert.Same(edited, soup.Ingredients.Single().Ingredient);
        Assert.Equal(50, soup.Nutrients.Kcal);
        Assert.Same(soup, _main.Calendar.MealPlan.Single().Recipe);
        Assert.Same(_carrotSalad, _main.AllRecipes.Single(x => x.Id == _carrotSalad.Id));
    }

    [Fact]
    public void SavingCheckedIngredient_KeepsItChecked()
    {
        Check(_onion, true);

        var edited = IngredientWith(_onion.Id);
        _main.IngredientSaved(edited);

        Assert.True(edited.IsChecked);
        Assert.Equal(new[] { _onionSoup.Id }, _main.RecipesTab.Recipes.Select(x => x.Id));
    }

    [Fact]
    public void DeletingIngredient_RemovesItFromRecipesAndFilter()
    {
        Check(_onion, true);

        _main.IngredientsTab.DeleteCommand.Execute(_onion).Subscribe();

        Assert.DoesNotContain(_onion, _main.AllIngredients);
        Assert.Empty(_main.AllRecipes.Single(x => x.Id == _onionSoup.Id).Ingredients);
        Assert.Equal(2, _main.RecipesTab.Recipes.Count);
    }
}
