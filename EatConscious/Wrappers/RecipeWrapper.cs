using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using EatConscious.Models;

namespace EatConscious.Wrappers;


public class RecipeWrapper : IWrapper<RecipeWrapper>
{
    public List<string> Tags { get; init; } = new();
    public List<RecipeStripped> Recipes { get; init; } = new();

    public static RecipeWrapper StateOnLoad { get; } = DeserializeRecipes();

    private static RecipeWrapper DeserializeRecipes()
    {
        RecipeWrapper output;
        try
        {
            string recipesJson = File.ReadAllText(App.RecipePath);
            output = JsonSerializer.Deserialize<RecipeWrapper>(recipesJson) ?? new();
        }
        catch (Exception ex)
        {
            output = new();
        }

        return output;
    }
    
    public class IngredientPortion
    {
        public int Id { get; init; }
        public double Value { get; init; }
    }
    
#pragma warning disable CS8618
    public class RecipeStripped
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public List<IngredientPortion> Ingredients { get; init; }
        public List<string> Tags { get; init; }
        public string Note { get; init; }
    }
#pragma warning restore CS8618
}

public static class RecipeExtensions
{
    public static RecipeWrapper.RecipeStripped Strip(this Recipe r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Ingredients = r.Ingredients.Select(x => new RecipeWrapper.IngredientPortion()
        {
            Id = x.Ingredient.Id,
            Value = x.Value,
        }).ToList(),
        Tags = r.Tags,
        Note = r.Note,
    };
}