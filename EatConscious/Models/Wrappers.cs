using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EatConscious.Models;

/// <summary>
/// The data are (de)serialized in this wrapper so as to separate the tags of the ingredients from
/// the tags displayed when creating ingredient. Tag manipulation can be done by editing the json.
/// </summary>
public class IngredientsWrapper
{
    public List<string> Tags { get; init; } = new();
    public List<IngredientsWithMeasure> Ingredients { get; init; } = new();

    /// <summary>
    /// Data deserialized on the app start
    /// </summary>
    public static IngredientsWrapper StateOnLoad = DeserializeIngredients();
    
    /// <summary>
    /// Reads the input ingredients + tags file and deserializes it into wrapper
    /// </summary>
    private static IngredientsWrapper DeserializeIngredients()
    {
        IngredientsWrapper output;
        try
        {
            string ingredientsJson = File.ReadAllText(App.IngredientsPath);
            output = JsonSerializer.Deserialize<IngredientsWrapper>(ingredientsJson) ?? new();
        }
        catch (Exception ex)
        {
            output = new();
        }

        return output;
    }
    
#pragma warning disable CS8618
    /// <summary>
    /// Wrapper class for group of ingredients with common unit for <see cref="JsonSerializer"/>
    /// </summary>
    public class IngredientsWithMeasure
    {
        public string Unit { get; init; }
        public List<IngredientStripped> List { get; init; } = new();
    }

    /// <summary>
    /// Ingredients json representation for <see cref="JsonSerializer"/>
    /// </summary>
    public class IngredientStripped
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public Nutrients Nutrients { get; init; }
        public double Price { get; init; }
        public List<string> Tags { get; init; }
    }
#pragma warning restore CS8618
}

public static class IngredientExtensions
{
    /// <summary>
    /// Strips away the unit to create json representation
    /// </summary>
    public static IngredientsWrapper.IngredientStripped Strip(this Ingredient i) => new()
    {
        Id = i.Id,
        Name = i.Name,
        Nutrients = i.Nutrients,
        Price = i.Price,
        Tags = i.Tags,
    };
}