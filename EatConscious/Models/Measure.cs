using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace EatConscious.Models;

public class Measure
{
    public double BaseValue { get; }

    public string Id { get; }
    public string Name { get; }

    private Measure(string name, string measureId, double baseValue)
    {
        Name = name;
        Id = measureId;
        BaseValue = baseValue;
    }

    public static readonly Measure Gram = new Measure("gram", "g", 100);
    public static readonly Measure Mililiter = new Measure("mililiter", "ml", 100);
    public static readonly Measure Piece = new Measure("piece", "pcs", 1);

    public static IEnumerable<Measure> All()
    {
        yield return Gram;
        yield return Mililiter;
        yield return Piece;
    }

    public override string ToString() => Name;
}