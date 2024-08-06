using System;
using System.Collections.Generic;

namespace EatConscious.Models;

public class Ingredient
{
    public string Name { get; init; }
    
    /// <summary>
    /// <see cref="Nutrients"/> for respective base <see cref="Measure.BaseValue"/>
    /// </summary>
    public Nutrients Nutrients { get; set; }
    
    /// <summary>
    /// Price for respective base <see cref="Measure.BaseValue"/>
    /// </summary>
    public double Price { get; init; }

    public List<string> Tags { get; init; } = new();
    
    public Measure Unit { get; init; }

    /// <summary>
    /// The go-to method of creating new ingredient
    /// </summary>
    public static Ingredient Convert(string name, 
        Nutrients nutrientsPerX, double x, 
        double pricePerY, double y,
        Measure measure,
        List<string> tags)
    {
        var nutrients = nutrientsPerX.Map(n => n * (measure.BaseValue / x));
        var price = pricePerY * (measure.BaseValue / y);
        return new Ingredient(name, nutrients.Map(n => Math.Round(n, 2)), Math.Round(price, 2), tags, measure);
    }
    
    private Ingredient(string name, Nutrients nutrients, double price, List<string> tags, Measure unit)
    {
        Name = name;
        Nutrients = nutrients;
        Price = price;
        Tags = tags;
        Unit = unit;
    }

#pragma warning disable CS8618
    /// <summary>
    /// The default constructor used by the <see cref="IngredientsWrapper.UnwrapIngredients"/>, please use <see cref="Convert"/>
    /// </summary>
    public Ingredient()
    {
    }
#pragma warning restore CS8618
}