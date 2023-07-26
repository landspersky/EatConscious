using System;

namespace EatConscious.Models;

public abstract class Measure
{
    public double Value { get; init; }

    protected abstract string MeasureId { get; }

    public static double operator /(Measure a, Measure b)
    {
        Type aType = a.GetType();
        Type bType = b.GetType();
        if (aType != bType)
        {
            throw new ArgumentException($"{aType} is incompatible with {bType}");
        }
        return a.Value / b.Value;
    }

    public override string ToString()
    {
        return $"{Value} {MeasureId}";
    }
}

public class Mililiter : Measure
{
    protected override string MeasureId => "ml";
}

public class Gram : Measure
{
    protected override string MeasureId => "g";
}

public class Piece : Measure
{
    protected override string MeasureId => "pcs";
}

public static class IntExtensions
{
    public static T ToMeasure<T>(this int x) where T:Measure, new()
    {
        return new T() { Value = x };
    }

    public static Gram G(this int x) => x.ToMeasure<Gram>();
    public static Mililiter Ml(this int x) => x.ToMeasure<Mililiter>();
    public static Piece Pcs(this int x) => x.ToMeasure<Piece>();
}