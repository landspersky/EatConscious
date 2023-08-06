using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace EatConscious.Models;

// The data are (de)serialized in this wrapper so as to separate the tags of the ingredients from
// the tags displayed when creating ingredient. Tag manipulation can be done by editing the json.
public class IngredientsWrapper
{
    public List<string> Tags { get; init; } = new();
    public List<Ingredient> Ingredients { get; init; } = new();

    public static IngredientsWrapper StateOnLoad = DeserializeIngredients();

    private static IngredientsWrapper DeserializeIngredients()
    {
        var ingredientsJson = File.ReadAllText(App.IngredientsPath);
        var ingredientsWrapper = JsonSerializer.Deserialize<IngredientsWrapper>(ingredientsJson);
        return ingredientsWrapper ?? new();
    }
}