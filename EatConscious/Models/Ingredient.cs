namespace EatConscious.Models;

public record Ingredient
{
    public string Name { get; init; }
    
    // these data are relative to the base measure in BaseMeasures
    public Nutrients Nutrients { get; init; }
    public double Price { get; init; }

    // TODO: x and y could be of different types
    public static Ingredient Convert(string name, Nutrients nutrientsPerX, Measure x, double pricePerY, Measure y)
    {
        var baseMeasure = x;
        var nutrients = nutrientsPerX * (baseMeasure / x);
        var price = pricePerY * (baseMeasure / y);
        return new Ingredient(name, nutrients, price);
    }

    // the constructor has to be public, because JsonSerializer uses it
    public Ingredient(string name, Nutrients nutrients, double price)
    {
        Name = name;
        Nutrients = nutrients;
        Price = price;
    }
}