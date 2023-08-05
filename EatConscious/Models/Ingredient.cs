using System;
using System.Collections.Generic;

namespace EatConscious.Models;

public record Ingredient
{
    public string Name { get; init; }
    
    // these data are relative to the base measure in BaseMeasures
    public Nutrients Nutrients { get; set; }
    public double Price { get; init; }

    public List<string> Tags { get; init; } = new();

    // TODO: x and y could be of different types
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

    // the constructor has to be public, because JsonSerializer uses it
    public Ingredient(string name, Nutrients nutrients, double price)
    {
        Name = name;
        Nutrients = nutrients;
        Price = price;
    }
}