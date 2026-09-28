using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using EatConscious.Models;
using EatConscious.Views;
using EatConscious.Wrappers;

namespace EatConscious.ViewModels;

/// <summary>
/// The calendar tab; a colleague of <see cref="IMainMediator"/>
/// </summary>
public class CalendarViewModel : ViewModelBase
{
    private readonly IMainMediator _mediator;

    public CalendarViewModel(IMainMediator mediator, IEnumerable<MealPlanEntry> mealPlan)
    {
        _mediator = mediator;
        MealPlan = new(mealPlan);

        PrevMonthCommand = ReactiveCommand.Create(() => { CurrentMonth = CurrentMonth.AddMonths(-1); });
        NextMonthCommand = ReactiveCommand.Create(() => { CurrentMonth = CurrentMonth.AddMonths(1); });
        TodayCommand = ReactiveCommand.Create(() => { CurrentMonth = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1); });
        RemoveMealPlanEntryCommand = ReactiveCommand.Create<MealPlanEntry>(RemoveMealPlanEntry);

        _calendarDays = BuildCalendarDays();
    }

    /// <summary>
    /// Every recipe planned for every day; the source of truth for <see cref="CalendarDays"/>.
    /// </summary>
    public ObservableCollection<MealPlanEntry> MealPlan { get; }

    /// <summary>
    /// Recipes offered in the add-recipe picker of every day, regardless of the recipes tab filters.
    /// </summary>
    public ReadOnlyObservableCollection<Recipe> Recipes => _mediator.AllRecipes;

    private DateOnly _currentMonth = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
    /// <summary>
    /// The first day of the month currently displayed in the calendar grid.
    /// </summary>
    public DateOnly CurrentMonth
    {
        get => _currentMonth;
        set
        {
            this.RaiseAndSetIfChanged(ref _currentMonth, value);
            RebuildCalendarDays();
        }
    }

    private bool _showDaySummaries = true;
    /// <summary>
    /// Toggles the per-day nutrient totals in every calendar cell.
    /// </summary>
    public bool ShowDaySummaries
    {
        get => _showDaySummaries;
        set => this.RaiseAndSetIfChanged(ref _showDaySummaries, value);
    }

    public ReactiveCommand<Unit, Unit> PrevMonthCommand { get; }
    public ReactiveCommand<Unit, Unit> NextMonthCommand { get; }
    public ReactiveCommand<Unit, Unit> TodayCommand { get; }

    private ReadOnlyObservableCollection<CalendarDayViewModel> _calendarDays;
    /// <summary>
    /// One entry per rendered grid cell (includes leading/trailing days of adjacent months).
    /// </summary>
    public ReadOnlyObservableCollection<CalendarDayViewModel> CalendarDays => _calendarDays;

    private ReadOnlyObservableCollection<CalendarDayViewModel> BuildCalendarDays()
    {
        var days = CalendarMonthBuilder.BuildDays(CurrentMonth);
        var dayViewModels = days.Select(date => new CalendarDayViewModel(
            date,
            isCurrentMonth: date.Month == CurrentMonth.Month && date.Year == CurrentMonth.Year,
            entries: MealPlan.Where(x => x.Date == date),
            onAddRecipe: AddMealPlanEntry,
            onOpenDetail: OpenDayDetail)).ToList();

        return new ReadOnlyObservableCollection<CalendarDayViewModel>(new ObservableCollection<CalendarDayViewModel>(dayViewModels));
    }

    /// <summary>
    /// Rebuilds <see cref="CalendarDays"/> for the current <see cref="CurrentMonth"/> from <see cref="MealPlan"/>.
    /// </summary>
    private void RebuildCalendarDays()
    {
        _calendarDays = BuildCalendarDays();
        this.RaisePropertyChanged(nameof(CalendarDays));
    }

    /// <summary>
    /// Adds a recipe to the given day and refreshes the grid.
    /// </summary>
    public void AddMealPlanEntry(DateOnly date, Recipe recipe)
    {
        MealPlan.Add(new MealPlanEntry { Date = date, Recipe = recipe });
        RebuildCalendarDays();
    }

    /// <summary>
    /// Command for removing a single recipe from a single day.
    /// </summary>
    public ReactiveCommand<MealPlanEntry, Unit> RemoveMealPlanEntryCommand { get; }

    public void RemoveMealPlanEntry(MealPlanEntry entry)
    {
        MealPlan.Remove(entry);
        RebuildCalendarDays();
    }

    /// <summary>
    /// Points every entry of the recipe (matched by id) to its new version.
    /// </summary>
    public void ReplaceRecipe(Recipe recipe)
    {
        for (int i = 0; i < MealPlan.Count; i++)
        {
            if (MealPlan[i].Recipe.Id == recipe.Id)
            {
                MealPlan[i] = new MealPlanEntry { Date = MealPlan[i].Date, Recipe = recipe };
            }
        }
        RebuildCalendarDays();
    }

    /// <summary>
    /// Removes every entry of the recipe (matched by id) from every day.
    /// </summary>
    public void RemoveRecipe(Recipe recipe)
    {
        foreach (var entry in MealPlan.Where(x => x.Recipe.Id == recipe.Id).ToList())
        {
            MealPlan.Remove(entry);
        }
        RebuildCalendarDays();
    }

    /// <summary>
    /// Opens the detail of a single day. It's modal, so the meal plan can't change behind its back.
    /// </summary>
    private void OpenDayDetail(DateOnly date)
    {
        var window = new DayDetailWindow(new DayDetailViewModel(date, this));
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } owner })
        {
            window.ShowDialog(owner);
        }
    }

    public MealPlanWrapper WrapMealPlan()
    {
        return new MealPlanWrapper()
        {
            Entries = MealPlan.Select(x => x.Strip()).ToList(),
        };
    }
}
