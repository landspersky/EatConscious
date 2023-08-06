using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Avalonia.Controls.Selection;
using EatConscious.Models;
using EatConscious.Views;

namespace EatConscious.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public SelectionModel<Ingredient> IngredientSelection { get; } = new();
    public void Click()
    {
        var window = new NewIngredientWindow(this);
        window.Show();
    }

    public List<Ingredient> Ingredients { get; init; } = IngredientsWrapper.StateOnLoad.Ingredients;
    
    public List<string> Tags { get; } = IngredientsWrapper.StateOnLoad.Tags;

    public IngredientsWrapper WrapIngredients() => new IngredientsWrapper()
    {
        Tags = this.Tags,
        Ingredients = this.Ingredients
    };

    public MainWindowViewModel()
    {
        IngredientSelection.Select(4);
        
    }
}