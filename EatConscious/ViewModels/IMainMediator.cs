using EatConscious.Models;
using System.Collections.ObjectModel;

namespace EatConscious.ViewModels;

/// <summary>
/// Abstract mediator between the tabs of the main window (<see cref="IngredientsTabViewModel"/>,
/// <see cref="RecipesTabViewModel"/>, <see cref="CalendarViewModel"/>) and the editor windows.
/// </summary>
public interface IMainMediator
{
    #region SHARED DATA
    /// <summary>
    /// Every ingredient regardless of the filters in the ingredients tab
    /// </summary>
    ReadOnlyObservableCollection<Ingredient> AllIngredients { get; }

    /// <summary>
    /// Every recipe regardless of the filters in the recipes tab
    /// </summary>
    ReadOnlyObservableCollection<Recipe> AllRecipes { get; }

    /// <summary>
    /// Available tags for assigning to ingredients
    /// </summary>
    ObservableCollection<string> IngredientTags { get; }

    /// <summary>
    /// Available tags for assigning to recipes
    /// </summary>
    ObservableCollection<string> RecipeTags { get; }
    #endregion

    #region NOTIFICATIONS
    /// <summary>
    /// An ingredient was (un)checked for filtering recipes
    /// </summary>
    void IngredientCheckChanged(Ingredient ingredient, bool isChecked);

    /// <summary>
    /// An ingredient was created or edited (matched by id)
    /// </summary>
    void IngredientSaved(Ingredient ingredient);

    void IngredientDeleted(Ingredient ingredient);

    /// <summary>
    /// A recipe was created or edited (matched by id)
    /// </summary>
    void RecipeSaved(Recipe recipe);

    void RecipeDeleted(Recipe recipe);
    #endregion

    #region WINDOWS
    /// <summary>
    /// Opens the ingredient editor
    /// </summary>
    /// <param name="toEdit">Ingredient to edit; null for creating a new one</param>
    void OpenIngredientEditor(Ingredient? toEdit);

    /// <summary>
    /// Opens the recipe editor
    /// </summary>
    /// <param name="toEdit">Recipe to edit; null for creating a new one</param>
    void OpenRecipeEditor(Recipe? toEdit);
    #endregion
}
