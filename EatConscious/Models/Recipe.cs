using System.Collections.Generic;
using System.Linq;

namespace EatConscious.Models;

public class Recipe
{
    public int Id { get; init; }
    public string Name { get; init; }
    public List<IngredientPortion> Ingredients { get; init; } = new();

    public List<string> Tags { get; init; } = new();
    
    public string Note { get; init; }

    public double Price => Ingredients.Sum(x => x.Ingredient.Price * (x.Ingredient.Unit.BaseValue / x.Value));
    
    public double Kcal => Ingredients.Sum(x => x.Ingredient.Nutrients.Kcal * (x.Ingredient.Unit.BaseValue / x.Value));
}

public class IngredientPortion
{
    public Ingredient Ingredient { get; init; }
    public double Value { get; init; }
}