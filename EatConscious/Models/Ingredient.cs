using System;
using System.Collections.Generic;

namespace EatConscious.Models;

public class Ingredient
{
    public string Name { get; init; }
    
    /// <summary>
    /// <see cref="Nutrients"/> for respective base <see cref="Measure"/> in <see cref="Measure.OneOfEach"/>
    /// </summary>
    public Nutrients Nutrients { get; set; }
    /// <summary>
    /// Price for respective base <see cref="Measure"/> in <see cref="Measure.OneOfEach"/>
    /// </summary>
    public double Price { get; init; }

    public List<string> Tags { get; init; } = new();

    // TODO: x and y could be of different types
    /// <summary>
    /// The go-to method of creating new ingredient
    /// </summary>
    public static Ingredient Convert(string name, 
        Nutrients nutrientsPerX, Measure x, 
        double pricePerY, Measure y,
        List<string> tags)
    {
        var baseMeasure = Measure.GetBase(x);
        var nutrients = nutrientsPerX.Map(n => n * (baseMeasure / x));
        var price = pricePerY * (baseMeasure / y);
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