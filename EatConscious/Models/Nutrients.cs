using System;

namespace EatConscious.Models;

public record struct Nutrients
{
    public double Kcal { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fats { get; init; }

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
}