using System;
using System.Collections.Generic;

namespace EatConscious.ViewModels;

/// <summary>
/// Pure date-arithmetic for laying out a month as full Monday-start weeks.
/// </summary>
public static class CalendarMonthBuilder
{
    /// <summary>
    /// Returns every day needed to render <paramref name="monthStart"/>'s month as full
    /// Monday-to-Sunday weeks, including leading/trailing days from adjacent months.
    /// </summary>
    /// <param name="monthStart">The first day of the month to render.</param>
    public static List<DateOnly> BuildDays(DateOnly monthStart)
    {
        int leadingOffset = ((int)monthStart.DayOfWeek + 6) % 7; // Monday = 0 ... Sunday = 6
        var firstDay = monthStart.AddDays(-leadingOffset);

        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        int trailingOffset = (7 - (((int)monthEnd.DayOfWeek + 6) % 7 + 1)) % 7;
        var lastDay = monthEnd.AddDays(trailingOffset);

        var days = new List<DateOnly>();
        for (var day = firstDay; day <= lastDay; day = day.AddDays(1))
        {
            days.Add(day);
        }

        return days;
    }
}
