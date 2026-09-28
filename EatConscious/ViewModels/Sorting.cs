using System;
using System.Collections.Generic;
using System.Linq;
using EatConscious.Models;

namespace EatConscious.ViewModels;

/// <summary>
/// Possible values to sort on
/// </summary>
public enum Sorting
{
    Name,
    Price,
    Kcal,
    Protein,
    Carbs,
    Fats
}

/// <summary>
/// Exposes all fields from <see cref="Sorting"/> for common comparing functions
/// </summary>
public interface ISortable
{
    string Name { get; }
    double Price { get; }
    Nutrients Nutrients { get; }
}

public static class SortingExtensions
{
    private static readonly Dictionary<Sorting, Comparison<ISortable>> CompareFunctions = new()
    {
        { Sorting.Name, (x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase) },
        { Sorting.Price, (x, y) => x.Price.CompareTo(y.Price) },
        { Sorting.Kcal, (x, y) => x.Nutrients.Kcal.CompareTo(y.Nutrients.Kcal) },
        { Sorting.Protein, (x, y) => x.Nutrients.Protein.CompareTo(y.Nutrients.Protein) },
        { Sorting.Carbs, (x, y) => x.Nutrients.Carbs.CompareTo(y.Nutrients.Carbs) },
        { Sorting.Fats, (x, y) => x.Nutrients.Fats.CompareTo(y.Nutrients.Fats) },
    };

    /// <summary>
    /// Gets comparer based on the value passed in the parameter
    /// </summary>
    public static IComparer<ISortable> GetComparer(this Sorting s)
    {
        return Comparer<ISortable>.Create(CompareFunctions[s]);
    }

    private static readonly Dictionary<string, Sorting> SortingByName =
        Enum.GetValues<Sorting>().ToDictionary(k => k.ToString(), v => v);

    /// <summary>
    /// Inverse function to ToString
    /// </summary>
    public static Sorting ToSorting(this string s) => SortingByName[s];
}