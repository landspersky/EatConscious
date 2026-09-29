using System.Collections.ObjectModel;
using EatConscious.Models;
using EatConscious.Views;

namespace EatConscious.ViewModels;

/// <summary>
/// Concrete mediator: owns the tabs and routes what happens in one of them to the others.
/// </summary>
/// <remarks>Only routing belongs here; the behavior itself stays in the tabs.</remarks>
public class MainWindowViewModel : ViewModelBase, IMainMediator
{
    public MainWindowViewModel()
    {
        IngredientsTab = new IngredientsTabViewModel(this, State.OnLoad.Ingredients, State.OnLoad.IngredientTags);
        RecipesTab = new RecipesTabViewModel(this, State.OnLoad.Recipes, State.OnLoad.RecipeTags);
        Calendar = new CalendarViewModel(this, State.OnLoad.MealPlan);
    }

    public IngredientsTabViewModel IngredientsTab { get; }

    public RecipesTabViewModel RecipesTab { get; }

    public CalendarViewModel Calendar { get; }

    #region SHARED DATA
    public ReadOnlyObservableCollection<Ingredient> AllIngredients => IngredientsTab.AllIngredients;

    public ReadOnlyObservableCollection<Recipe> AllRecipes => RecipesTab.AllRecipes;

    public ObservableCollection<string> IngredientTags => IngredientsTab.Tags;

    public ObservableCollection<string> RecipeTags => RecipesTab.Tags;
    #endregion

    #region NOTIFICATIONS
    public void IngredientCheckChanged(Ingredient ingredient, bool isChecked) =>
        RecipesTab.SetIngredientChecked(ingredient, isChecked);

    public void IngredientSaved(Ingredient ingredient)
    {
        IngredientsTab.AddOrUpdate(ingredient);
        RecipesTab.ReplaceIngredient(ingredient);
    }

    public void IngredientDeleted(Ingredient ingredient) => RecipesTab.RemoveIngredient(ingredient);

    public void RecipeSaved(Recipe recipe)
    {
        RecipesTab.AddOrUpdate(recipe);
        Calendar.ReplaceRecipe(recipe);
    }

    public void RecipeDeleted(Recipe recipe) => Calendar.RemoveRecipe(recipe);
    #endregion

    #region WINDOWS
    public void OpenIngredientEditor(Ingredient? toEdit)
    {
        var viewModel = toEdit is null ? new NewIngredientViewModel(this) : new NewIngredientViewModel(this, toEdit);
        new NewIngredientWindow(viewModel).Show();
    }

    public void OpenRecipeEditor(Recipe? toEdit)
    {
        var viewModel = toEdit is null ? new NewRecipeViewModel(this) : new NewRecipeViewModel(this, toEdit);
        new NewRecipeWindow(viewModel).Show();
    }
    #endregion
}
