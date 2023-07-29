using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EatConscious.Models;
using EatConscious.Views;

namespace EatConscious.ViewModels;

public class MainWindowViewModel : ViewModelBase
{

    public void Click()
    {
        var window = new NewIngredientWindow(this);
        window.Show();
    }

    public List<Ingredient> Ingredients { get; set; } = DeserializeIngredients();

    private static List<Ingredient> DeserializeIngredients()
    {
        var ingredientsJson = File.ReadAllText(App.IngredientsPath);
        var ingredients = JsonSerializer.Deserialize<List<Ingredient>>(ingredientsJson);
        return ingredients ?? new List<Ingredient>();
    }
}