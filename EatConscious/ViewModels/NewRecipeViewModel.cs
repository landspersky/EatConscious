using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using EatConscious.Models;

namespace EatConscious.ViewModels;

public class NewRecipeViewModel : ViewModelBase
{
    public string Name { get; set; }
    
    public string Note { get; set; }
    
    public ObservableCollection<string> Tags { get; }
    
    public ObservableCollection<string> SelectedTags { get; set; } = new();
    
    /// <summary>
    /// Ingredient being edited; otherwise null
    /// </summary>
    private readonly Recipe? _editing;

    /// <summary>
    /// What is displayed on the button on the bottom
    /// </summary>
    public string ButtonText => _editing is null ? "Add" : "Edit";
    
    /// <summary>
    /// Initialized to all ingredients, we pick non-zero elements from it
    /// </summary>
    public List<IngredientPortion> Ingredients { get; set; }

    private Recipe CreateRecipe()
    {
        int id;
        if (_editing is null)
        {
            State.IncrementRecipeId();
            id = State.TopRecipeId;
        }
        else
        {
            id = _editing.Id;
        }
        return new Recipe()
        {
            Id = id,
            Name = Name,
            Ingredients = Ingredients.Where(x => x.Value > 0).ToList(),
            Note = Note,
            Tags = SelectedTags.ToList(),
        };
    }
    
    public void ButtonClick() => _mainModel.AddOrUpdate(CreateRecipe());
    
    private readonly MainWindowViewModel _mainModel;

    /// <summary>
    /// Fills up <see cref="Ingredients"/>
    /// </summary>
    /// <remarks>Populates the date from the underlying recipe, if available/></remarks>
    private void InitIngredients()
    {
        Dictionary<int, double> editingPortions = new();
        if (_editing is not null)
        {
            editingPortions = _editing.Ingredients.ToDictionary(k => k.Ingredient.Id, v => v.Value);
        }
        
        Ingredients = _mainModel.Ingredients.Select(x => new IngredientPortion()
        {
            Ingredient = x,
            Value = editingPortions.GetValueOrDefault(x.Id, 0),
        }).ToList();
    }
    
    /// <summary>
    /// View model for creating a new recipe
    /// </summary>
    /// <param name="mainModel">Parent view model</param>
    public NewRecipeViewModel(MainWindowViewModel mainModel)
    {
        _mainModel = mainModel;
        Tags = mainModel.RecipeTags;
        InitIngredients();
    }

    /// <summary>
    /// View model for editing a recipe
    /// </summary>
    /// <param name="mainModel">Parent view model</param>
    /// <param name="openedFrom">Recipe to edit</param>
    public NewRecipeViewModel(MainWindowViewModel mainModel, Recipe openedFrom)
    {
        _mainModel = mainModel;
        Tags = mainModel.RecipeTags;
        Name = openedFrom.Name;
        Note = openedFrom.Note;
        _editing = openedFrom;
        InitIngredients();
    }
}