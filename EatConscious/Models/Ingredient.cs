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

    /// <summary>
    /// The go-to method of creating new ingredient
    /// </summary>
    public static Ingredient Convert(string name, 
        Nutrients nutrientsPerX, double x, 
        double pricePerY, double y,
        Measure measure,
        List<string> tags)
    {
        var nutrients = nutrientsPerX.Map(n => n * (x / measure.BaseValue));
        var price = pricePerY * (y / measure.BaseValue);
        return new Ingredient(name, nutrients.Map(n => Math.Round(n, 2)), Math.Round(price, 2)) {Tags = tags};
    }
    
    /// <summary>
    /// The default constructor used by the JsonSeriarializer, please use <see cref="Convert"/>
    /// </summary>
    public Ingredient(string name, Nutrients nutrients, double price)
    {
        Name = name;
        Nutrients = nutrients;
        Price = price;
    }
}