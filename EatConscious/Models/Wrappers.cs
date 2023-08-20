using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace EatConscious.Models;

/// <summary>
/// The data are (de)serialized in this wrapper so as to separate the tags of the ingredients from
/// the tags displayed when creating ingredient. Tag manipulation can be done by editing the json.
/// </summary>
public class IngredientsWrapper
{
    public ObservableCollection<string> Tags { get; init; } = new();
    public ObservableCollection<Ingredient> Ingredients { get; init; } = new();

    /// <summary>
    /// Data deserialized on the app start
    /// </summary>
    public static IngredientsWrapper StateOnLoad = DeserializeIngredients();

    /// <summary>
    /// Reads the input ingredients + tags file and deserializes it into wrapper
    /// </summary>
    private static IngredientsWrapper DeserializeIngredients()
    {
        var ingredientsJson = File.ReadAllText(App.IngredientsPath);
        var ingredientsWrapper = JsonSerializer.Deserialize<IngredientsWrapper>(ingredientsJson);
        return ingredientsWrapper ?? new();
    }
}