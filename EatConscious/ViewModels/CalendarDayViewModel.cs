using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using EatConscious.Models;

namespace EatConscious.ViewModels;

/// <summary>
/// One rendered cell in the calendar month grid.
/// </summary>
public class CalendarDayViewModel : ViewModelBase
{
    public DateOnly Date { get; }

    /// <summary>
    /// False for leading/trailing days that belong to the previous/next month, rendered dimmed.
    /// </summary>
    public bool IsCurrentMonth { get; }

    public bool IsToday { get; }

    public ObservableCollection<MealPlanEntry> Entries { get; }

    private bool _isPickerOpen;
    /// <summary>
    /// Drives the visibility of the add-recipe popup anchored to this cell's "+" button.
    /// </summary>
    public bool IsPickerOpen
    {
        get => _isPickerOpen;
        set => this.RaiseAndSetIfChanged(ref _isPickerOpen, value);
    }

    public ReactiveCommand<Unit, Unit> TogglePickerCommand { get; }

    /// <summary>
    /// Adds the given recipe to this day, then closes the picker.
    /// </summary>
    public ReactiveCommand<Recipe, Unit> AddRecipeCommand { get; }

    public CalendarDayViewModel(DateOnly date, bool isCurrentMonth, IEnumerable<MealPlanEntry> entries,
        Action<DateOnly, Recipe> onAddRecipe)
    {
        Date = date;
        IsCurrentMonth = isCurrentMonth;
        IsToday = date == DateOnly.FromDateTime(DateTime.Now);
        Entries = new ObservableCollection<MealPlanEntry>(entries);

        TogglePickerCommand = ReactiveCommand.Create(() =>
        {
            IsPickerOpen = !IsPickerOpen;
        });
        AddRecipeCommand = ReactiveCommand.Create<Recipe>(recipe =>
        {
            onAddRecipe(Date, recipe);
            IsPickerOpen = false;
        });
    }
}
