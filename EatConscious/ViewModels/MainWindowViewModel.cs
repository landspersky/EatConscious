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

    public List<Ingredient> Ingredients { get; init; } = DeserializeIngredients();
    private static List<Ingredient> DeserializeIngredients()
    {
        var ingredientsJson = File.ReadAllText(App.IngredientsPath);
        var ingredients = JsonSerializer.Deserialize<List<Ingredient>>(ingredientsJson);
        return ingredients ?? new List<Ingredient>();
    }

    public MainWindowViewModel()
    {
        IngredientSelection.Select(4);
        
    }
}