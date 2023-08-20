using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace EatConscious.Models;

public abstract class Measure
{
    public double Value { get; set; }

    public abstract string MeasureId { get; }
    
    /// <returns>Base measure based on the type of measure.
    /// Data in Ingredients are calculated per this measure.</returns>
    public static Measure GetBase(Measure m)
    {
        return m switch
        {
            Gram => 100.ToMeasure<Gram>(),
            Mililiter => 100.ToMeasure<Mililiter>(),
            Piece => 1.ToMeasure<Piece>(),
            _ => throw new ArgumentException($"The measure {m.GetType()} is not supported")
        };
    }

    /// <summary>
    /// 1 X for each class X derived from Measure ie. [1 g, 1 ml,...]
    /// </summary>
    public static readonly List<Measure> OneOfEach = GetOneOfEach();
    
    private static List<Measure> GetOneOfEach()
    {
        
        var assembly = Assembly.GetExecutingAssembly();
        // get all the types inheriting from Measure
        var measures = assembly.GetTypes()
            .Where(type => typeof(Measure).IsAssignableFrom(type) && !type.IsAbstract);

        // for each Type T, invoke the generic method of creating T on 1
        var oneOfEach = measures.Select(type =>
        {
            MethodInfo? method = typeof(IntExtensions).GetMethod(nameof(IntExtensions.ToMeasure));
            MethodInfo genericMethod = method.MakeGenericMethod(type);
            return (Measure)genericMethod.Invoke(null, new object[]{1});
        });
        
        return oneOfEach.ToList();
    }

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
    public override string MeasureId => "ml";
}

public class Gram : Measure
{
    public override string MeasureId => "g";
}

public class Piece : Measure
{
    public override string MeasureId => "pcs";
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