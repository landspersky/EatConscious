using System.Collections.Generic;
using System.Collections.ObjectModel;
using EatConscious.Models;
using EatConscious.ViewModels;

namespace EatConscious.Tests;

/// <summary>
/// Records what the colleagues tell the mediator, so they can be tested in isolation
/// </summary>
public class FakeMediator : IMainMediator
{
    public ObservableCollection<Ingredient> Ingredients { get; } = new();
    public ObservableCollection<Recipe> Recipes { get; } = new();

    public ReadOnlyObservableCollection<Ingredient> AllIngredients => new(Ingredients);
    public ReadOnlyObservableCollection<Recipe> AllRecipes => new(Recipes);
    public ObservableCollection<string> IngredientTags { get; } = new();
    public ObservableCollection<string> RecipeTags { get; } = new();

    public List<(Ingredient Ingredient, bool IsChecked)> CheckChanges { get; } = new();
    public List<Ingredient> SavedIngredients { get; } = new();
    public List<Ingredient> DeletedIngredients { get; } = new();
    public List<Recipe> SavedRecipes { get; } = new();
    public List<Recipe> DeletedRecipes { get; } = new();
    public List<Ingredient?> OpenedIngredientEditors { get; } = new();
    public List<Recipe?> OpenedRecipeEditors { get; } = new();

    public void IngredientCheckChanged(Ingredient ingredient, bool isChecked) => CheckChanges.Add((ingredient, isChecked));
    public void IngredientSaved(Ingredient ingredient) => SavedIngredients.Add(ingredient);
    public void IngredientDeleted(Ingredient ingredient) => DeletedIngredients.Add(ingredient);
    public void RecipeSaved(Recipe recipe) => SavedRecipes.Add(recipe);
    public void RecipeDeleted(Recipe recipe) => DeletedRecipes.Add(recipe);
    public void OpenIngredientEditor(Ingredient? toEdit) => OpenedIngredientEditors.Add(toEdit);
    public void OpenRecipeEditor(Recipe? toEdit) => OpenedRecipeEditors.Add(toEdit);
}
