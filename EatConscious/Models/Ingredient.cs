using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EatConscious.Models;

public static class BaseMeasures
{
    // all ingredients have info per these measures
    public static Measure Get(Measure m)
    {
        return m switch
        {
            Gram => 100.ToMeasure<Gram>(),
            Mililiter => 100.ToMeasure<Mililiter>(),
            Piece => 1.ToMeasure<Piece>(),
            _ => throw new ArgumentException($"The measure {m.GetType()} is not supported")
        };
    }
}

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
        var baseMeasure = BaseMeasures.Get(x);
        Nutrients = nutrientsPerX * (baseMeasure / x);
        Price = pricePerY * (baseMeasure / y);
    }
}