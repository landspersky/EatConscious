using System.Collections.ObjectModel;
using Avalonia.Controls.Selection;
using EatConscious.Models;
using EatConscious.Views;

namespace EatConscious.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// TODO: Utilize in the future or redo
    /// cool methods like Select(index)...
    /// </summary>
    public SelectionModel<Ingredient> IngredientSelection { get; } = new();
    public void AddIngredientClick()
    {
        var window = new NewIngredientWindow(this);
        window.Show();
    }

    public ObservableCollection<Ingredient> Ingredients { get; init; } = new(IngredientsWrapper.StateOnLoad.Ingredients);
    
    /// <summary>
    /// These tags are shown when creating new ingredient, they do not limit already existing ones
    /// </summary>
    public ObservableCollection<string> Tags { get; } = IngredientsWrapper.StateOnLoad.Tags;

    /// <summary>
    /// Serializes ingredients and tags from the model
    /// </summary>
    public IngredientsWrapper WrapIngredients() => new IngredientsWrapper()
    {
        Tags = this.Tags,
        Ingredients = this.Ingredients
    };
}