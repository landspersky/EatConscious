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
    /// Initialized to all ingredients, we pick non-zero elements from it
    /// </summary>
    public List<IngredientPortion> Ingredients { get; set; }

    private Recipe CreateRecipe()
    {
        State.IncrementRecipeId();
        return new Recipe()
        {
            Id = State.TopRecipeId,
            Name = Name,
            Ingredients = Ingredients.Where(x => x.Value > 0).ToList(),
            Note = Note,
            Tags = SelectedTags.ToList(),
        };
    }
    
    public void AddClick() => _mainModel.AddOrUpdate(CreateRecipe());
    
    private readonly MainWindowViewModel _mainModel;
    
    public NewRecipeViewModel(MainWindowViewModel mainModel)
    {
        _mainModel = mainModel;
        Tags = mainModel.RecipeTags;
        Ingredients = _mainModel.Ingredients.Select(x => new IngredientPortion()
        {
            Ingredient = x,
            Value = 0,
        }).ToList();
    }
}