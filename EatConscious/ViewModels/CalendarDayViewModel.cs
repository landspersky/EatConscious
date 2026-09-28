using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

    public bool HasEntries => Entries.Count > 0;

    /// <summary>
    /// How many entries fit in the cell; the rest is reachable through the day detail.
    /// </summary>
    public const int MaxVisibleEntries = 3;

    // The grid is rebuilt on every meal plan change, so the properties below don't need to react to Entries changes.
    public List<MealPlanEntry> VisibleEntries => Entries.Take(MaxVisibleEntries).ToList();

    public bool HasMoreEntries => Entries.Count > MaxVisibleEntries;

    /// <summary>
    /// Nutrients of all recipes planned for this day added up.
    /// </summary>
    public Nutrients TotalNutrients => MealPlanEntry.TotalNutrients(Entries);

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

    /// <summary>
    /// Opens the day detail window listing every recipe of this day.
    /// </summary>
    public ReactiveCommand<Unit, Unit> OpenDetailCommand { get; }

    public CalendarDayViewModel(DateOnly date, bool isCurrentMonth, IEnumerable<MealPlanEntry> entries,
        Action<DateOnly, Recipe> onAddRecipe, Action<DateOnly> onOpenDetail)
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
        OpenDetailCommand = ReactiveCommand.Create(() => onOpenDetail(Date));
    }
}
