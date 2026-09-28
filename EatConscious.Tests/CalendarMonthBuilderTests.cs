using System;
using System.Linq;
using EatConscious.ViewModels;
using Xunit;

namespace EatConscious.Tests;

public class CalendarMonthBuilderTests
{
    [Fact]
    public void BuildDays_September2026_ReturnsFiveFullWeeksStartingPreviousMonday()
    {
        // September 1, 2026 is a Tuesday; September 30, 2026 is a Wednesday.
        var days = CalendarMonthBuilder.BuildDays(new DateOnly(2026, 9, 1));

        Assert.Equal(35, days.Count);
        Assert.Equal(new DateOnly(2026, 8, 31), days.First()); // preceding Monday
        Assert.Equal(new DateOnly(2026, 10, 4), days.Last());  // following Sunday
    }

    [Theory]
    [InlineData(2026, 2)]
    [InlineData(2026, 9)]
    [InlineData(2027, 1)]
    public void BuildDays_AnyMonth_StartsMondayEndsSundayAndCoversWholeMonth(int year, int month)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var days = CalendarMonthBuilder.BuildDays(monthStart);

        Assert.Equal(0, days.Count % 7);
        Assert.Equal(DayOfWeek.Monday, days.First().DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, days.Last().DayOfWeek);
        Assert.True(days.First() <= monthStart);
        Assert.True(days.Last() >= monthEnd);
        Assert.Contains(monthStart, days);
        Assert.Contains(monthEnd, days);
        // no gaps
        for (int i = 1; i < days.Count; i++)
        {
            Assert.Equal(days[i - 1].AddDays(1), days[i]);
        }
    }
}
