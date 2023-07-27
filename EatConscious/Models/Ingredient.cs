using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EatConscious.Models;

public record Ingredient
{
    public string Name { get; set; }
    
    // these data are relative to the base measure in BaseMeasures
    public Nutrients Nutrients { get; set; }
    public double Price { get; set; }

    // TODO: x and y could be of different types
    public Ingredient(string name, Nutrients nutrientsPerX, Measure x, double pricePerY, Measure y)
    {
        Name = name;
        var baseMeasure = Measure.GetBase(x);
        Nutrients = nutrientsPerX * (baseMeasure / x);
        Price = pricePerY * (baseMeasure / y);
    }
}