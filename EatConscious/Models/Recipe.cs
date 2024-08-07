using System;
using System.Collections.Generic;
using System.Linq;
using EatConscious.ViewModels;

namespace EatConscious.Models;

public class Recipe : ISortable
{
    public int Id { get; init; }
    public string Name { get; init; }
    public List<IngredientPortion> Ingredients { get; init; } = new();

    public List<string> Tags { get; init; } = new();
    
    public string Note { get; init; }

    public double Price => Ingredients.Sum(x => x.Ingredient.Price * (x.Value / x.Ingredient.Unit.BaseValue));

    /// <summary>
    /// Sum nutrient value of the ingredients weighted by the portion
    /// </summary>
    public Nutrients Nutrients
    {
        get
        {
            // When user changes ingredient/value, all categories will be changes, so it doesn't matter we're calculating
            // them together
            Nutrients output = new();
            foreach (var ingredientPortion in this.Ingredients)
            {
                var ingredient = ingredientPortion.Ingredient;
                var nutrients = ingredient.Nutrients;
                output = output.Combine(nutrients, 
                    (x, y) => x + y * (ingredientPortion.Value / ingredient.Unit.BaseValue));
            }

            return output.Map(n => Math.Round(n, 2));
        }
    }
    
}

public class IngredientPortion
{
    public Ingredient Ingredient { get; init; }
    public double Value { get; init; }
}