namespace EatConscious.Models;

public record struct Nutrients
{
    public double Kcal { get; init; }
    public double Protein { get; init; }
    public double Carbs { get; init; }
    public double Fats { get; init; }

    public static Nutrients operator *(Nutrients nutrients, double val)
    {
        return new Nutrients()
        {
            Kcal = nutrients.Kcal * val,
            Protein = nutrients.Protein * val,
            Carbs = nutrients.Carbs * val,
            Fats = nutrients.Fats * val
        };
    }

    public static Nutrients operator *(double val, Nutrients nutrients) => nutrients * val;
}