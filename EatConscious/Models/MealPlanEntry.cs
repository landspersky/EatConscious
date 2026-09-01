using System;

namespace EatConscious.Models;

/// <summary>
/// One recipe planned for one day. Multiple entries may share the same <see cref="Date"/>.
/// </summary>
public class MealPlanEntry
{
    public DateOnly Date { get; init; }
    public Recipe Recipe { get; init; }

#pragma warning disable CS8618
    public MealPlanEntry()
    {
    }
#pragma warning restore CS8618
}
