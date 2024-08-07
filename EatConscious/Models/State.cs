using System.Collections.Generic;
using System.Linq;
using EatConscious.Wrappers;

namespace EatConscious.Models;

public class State
{
    public static State OnLoad { get; } = Load();
    
    public List<Ingredient> Ingredients { get; private init; }
    public List<string> IngredientTags { get; private init; }
    
    /// <summary>
    /// Highest <see cref="Ingredient"/> Id for assigning to new objects
    /// </summary>
    public static int TopIngredientId { get; private set; }
    public static void IncrementIngredientId() => TopIngredientId++;
    
    public List<Recipe> Recipes { get; private init; }
    public List<string> RecipeTags { get; private init; }
    
    /// <summary>
    /// Highest <see cref="Recipe"/> Id for assigning to new objects
    /// </summary>
    public static int TopRecipeId { get; private set; }
    public static void IncrementRecipeId() => TopRecipeId++;

    private static State Load()
    {
        var ingredients = UnwrapIngredients(IngredientsWrapper.StateOnLoad);
        var ingredientTags = IngredientsWrapper.StateOnLoad.Tags;
        TopIngredientId = ingredients.Count == 0 ? 0 : ingredients.Max(x => x.Id);

        var recipes = UnwrapRecipes(RecipeWrapper.StateOnLoad, ingredients.ToDictionary(x => x.Id, x => x));
        var recipeTags = RecipeWrapper.StateOnLoad.Tags;
        TopRecipeId = recipes.Count == 0 ? 0 : recipes.Max(x => x.Id);
        
        return new State()
        {
            Ingredients = ingredients,
            IngredientTags = ingredientTags,
            Recipes = recipes,
            RecipeTags = recipeTags,
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
            Id = ingredient.Id,
            Name = ingredient.Name,
            Nutrients = ingredient.Nutrients,
            Price = ingredient.Price,
            Tags = ingredient.Tags,
            Unit = Measure.ById(measure.Unit),
        }).ToList();
    }

    private static List<Recipe> UnwrapRecipes(RecipeWrapper wrapper, Dictionary<int, Ingredient> ingredients)
    {
        IngredientPortion Convert(RecipeWrapper.IngredientPortion ingredientPortion) => new()
        {
            Ingredient = ingredients[ingredientPortion.Id],
            Value = ingredientPortion.Value,
        };
        
        return wrapper.Recipes.Select(recipe => new Recipe()
        {
            Id = recipe.Id,
            Name = recipe.Name,
            Ingredients = recipe.Ingredients.Select(Convert).ToList(),
            Tags = recipe.Tags,
        }).ToList();
    }
}