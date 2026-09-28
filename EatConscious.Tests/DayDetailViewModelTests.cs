using System;
using System.Linq;
using EatConscious.Models;
using EatConscious.ViewModels;
using Xunit;

namespace EatConscious.Tests;

public class DayDetailViewModelTests
{
    private static readonly DateOnly Day = new(2026, 9, 15);

    [Fact]
    public void Entries_ContainsOnlyThatDay()
    {
        var main = new MainWindowViewModel();
        var recipe = new Recipe { Id = 1, Name = "Soup" };
        main.AddMealPlanEntry(Day, recipe);
        main.AddMealPlanEntry(Day, recipe);
        main.AddMealPlanEntry(Day.AddDays(1), recipe);

        var detail = new DayDetailViewModel(Day, main);

        Assert.Equal(2, detail.Entries.Count);
        Assert.All(detail.Entries, e => Assert.Equal(Day, e.Date));
    }

    [Fact]
    public void RemoveEntry_RemovesOnlyThatEntryFromDetailAndMealPlan()
    {
        var main = new MainWindowViewModel();
        var recipe = new Recipe { Id = 1, Name = "Soup" };
        main.AddMealPlanEntry(Day, recipe);
        main.AddMealPlanEntry(Day, recipe);
        main.AddMealPlanEntry(Day.AddDays(1), recipe);
        var detail = new DayDetailViewModel(Day, main);

        detail.RemoveEntryCommand.Execute(detail.Entries[0]).Subscribe();

        Assert.Single(detail.Entries);
        Assert.Equal(1, main.MealPlan.Count(e => e.Date == Day));
        Assert.Equal(1, main.MealPlan.Count(e => e.Date == Day.AddDays(1)));
    }
}
