using System.Collections.Generic;
using System.Linq;
using Avalonia.Media.Fonts;

namespace EatConscious.Models;

public class State
{
    public List<Ingredient> Ingredients { get; private init; }
    public List<string> IngredientTags { get; private init; }
    
    public static State OnLoad { get; } = Load();

    private static State Load()
    {
        var ingredients = UnwrapIngredients(IngredientsWrapper.StateOnLoad);
        var ingredientTags = IngredientsWrapper.StateOnLoad.Tags;
        return new State()
        {
            Ingredients = ingredients,
            IngredientTags = ingredientTags,
        };
    }

    private State()
    {
        
    }
    
    /// <summary>
    /// Collects all the ingredients and joins them with units
    /// </summary>
    /// <returns>All ingredients in their <see cref="Ingredient"/> representation</returns>
    private static List<Ingredient> UnwrapIngredients(IngredientsWrapper wrapper)
    {
        return wrapper.Ingredients.SelectMany(x => x.List, (measure, ingredient) => new Ingredient()
        {
            Name = ingredient.Name,
            Nutrients = ingredient.Nutrients,
            Price = ingredient.Price,
            Tags = ingredient.Tags,
            Unit = Measure.ById(measure.Unit),
        }).ToList();
    }
}