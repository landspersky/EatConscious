using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using ReactiveUI;
using EatConscious.Models;

namespace EatConscious.ViewModels;

/// <summary>
/// Every recipe planned for a single day, with the option to remove them from that day.
/// </summary>
public class DayDetailViewModel : ViewModelBase
{
    private readonly CalendarViewModel _calendar;

    public DateOnly Date { get; }

    public ObservableCollection<MealPlanEntry> Entries { get; }

    public bool HasEntries => Entries.Count > 0;

    public Nutrients TotalNutrients => MealPlanEntry.TotalNutrients(Entries);

    public double TotalPrice => Math.Round(Entries.Sum(x => x.Recipe.Price), 2);

    /// <summary>
    /// Removes a single entry from this day (other days planning the same recipe are untouched).
    /// </summary>
    public ReactiveCommand<MealPlanEntry, Unit> RemoveEntryCommand { get; }

    public DayDetailViewModel(DateOnly date, CalendarViewModel calendar)
    {
        _calendar = calendar;
        Date = date;
        Entries = new ObservableCollection<MealPlanEntry>(calendar.MealPlan.Where(x => x.Date == date));

        RemoveEntryCommand = ReactiveCommand.Create<MealPlanEntry>(entry =>
        {
            _calendar.RemoveMealPlanEntry(entry);
            Entries.Remove(entry);
            this.RaisePropertyChanged(nameof(HasEntries));
            this.RaisePropertyChanged(nameof(TotalNutrients));
            this.RaisePropertyChanged(nameof(TotalPrice));
        });
    }
}
