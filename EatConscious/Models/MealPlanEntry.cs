using System;
using System.Collections.Generic;
using System.Linq;

namespace EatConscious.Models;

/// <summary>
/// One recipe planned for one day. Multiple entries may share the same <see cref="Date"/>.
/// </summary>
public class MealPlanEntry
{
    public DateOnly Date { get; init; }
    public Recipe Recipe { get; init; }

    /// <summary>
    /// Nutrients of all the entries' recipes added up
    /// </summary>
    public static Nutrients TotalNutrients(IEnumerable<MealPlanEntry> entries) => entries.Aggregate(new Nutrients(),
        (total, entry) => total.Combine(entry.Recipe.Nutrients, (x, y) => x + y)).Map(n => Math.Round(n, 2));

#pragma warning disable CS8618
    public MealPlanEntry()
    {
    }
#pragma warning restore CS8618
}
