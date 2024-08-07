using System;

namespace EatConscious.Models;

/// <summary>
/// Container for amount of kcal, protein, carbs, fats of one ingredient
/// </summary>
public struct Nutrients
{
    public double Kcal { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fats { get; init; }

    /// <param name="f">Function to apply to each value</param>
    /// <returns>Nutrients with values transformed by the function</returns>
    public Nutrients Map(Func<double, double> f)
    {
        return new Nutrients()
        {
            Kcal = f(this.Kcal),
            Protein = f(this.Protein),
            Carbs = f(this.Carbs),
            Fats = f(this.Fats)
        };
    }

    /// <param name="otherNutrients">Nutrients to combine each value with</param>
    /// <param name="f">Combining function</param>
    /// <returns>Nutrients with values combined by the function</returns>
    public Nutrients Combine(Nutrients otherNutrients, Func<double, double, double> f)
    {
        return new Nutrients()
        {
            Kcal = f(Kcal, otherNutrients.Kcal),
            Protein = f(Protein, otherNutrients.Protein),
            Carbs = f(Carbs, otherNutrients.Carbs),
            Fats = f(Fats, otherNutrients.Fats),
        };
    }
}