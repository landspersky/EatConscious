using System;
using System.Collections.Generic;
using EatConscious.ViewModels;

namespace EatConscious.Models;

public class Ingredient : ISortable
{
    /// <summary>
    /// Unique identifier for each ingredient
    /// </summary>
    public int Id { get; init; }
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
    /// This property is specific for the ingredient form for filtering recipes
    /// </summary>
    public bool IsChecked { get; set; }

    /// <summary>
    /// The go-to method of creating new ingredients and editing existing ones
    /// </summary>
    public static Ingredient Create(int id, string name, 
        Nutrients nutrientsPerX, double x, 
        double pricePerY, double y,
        Measure measure,
        List<string> tags)
    {
        var nutrients = nutrientsPerX.Map(n => n * (measure.BaseValue / x));
        var price = pricePerY * (measure.BaseValue / y);
        return new Ingredient(id, name, nutrients.Map(n => Math.Round(n, 2)),
            Math.Round(price, 2), tags, measure);
    }
    
    private Ingredient(int id, string name, Nutrients nutrients, double price, List<string> tags, Measure unit)
    {
        Id = id;
        Name = name;
        Nutrients = nutrients;
        Price = price;
        Tags = tags;
        Unit = unit;
    }

#pragma warning disable CS8618
    /// <summary>
    /// The default constructor used by the <see cref="State.UnwrapIngredients"/>, please use <see cref="Create"/>
    /// </summary>
    public Ingredient()
    {
    }
#pragma warning restore CS8618
}