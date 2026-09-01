using System.ComponentModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class CalendarView : UserControl
{
    public CalendarView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Flyout.IsOpen is a read-only direct property in Avalonia 11, so the recipe-picker Flyout
    /// can't bind to <see cref="CalendarDayViewModel.IsPickerOpen"/> declaratively. The Button already
    /// opens its Flyout automatically on click; this closes it again once AddRecipeCommand (run when a
    /// recipe is picked) flips IsPickerOpen back to false.
    /// </summary>
    private void AddRecipeButton_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Flyout: { } flyout } button)
        {
            return;
        }

        CalendarDayViewModel? subscribedVm = null;

        void OnDayViewModelPropertyChanged(object? s, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CalendarDayViewModel.IsPickerOpen)
                && s is CalendarDayViewModel { IsPickerOpen: false })
            {
                flyout.Hide();
            }
        }

        void Subscribe(CalendarDayViewModel? vm)
        {
            if (subscribedVm is not null)
            {
                subscribedVm.PropertyChanged -= OnDayViewModelPropertyChanged;
            }

            subscribedVm = vm;

            if (subscribedVm is not null)
            {
                subscribedVm.PropertyChanged += OnDayViewModelPropertyChanged;
            }
        }

        void OnDataContextChanged(object? s, System.EventArgs args)
        {
            Subscribe(button.DataContext as CalendarDayViewModel);
        }

        button.DataContextChanged += OnDataContextChanged;
        Subscribe(button.DataContext as CalendarDayViewModel);

        button.Unloaded += OnButtonUnloaded;

        void OnButtonUnloaded(object? s, RoutedEventArgs args)
        {
            button.DataContextChanged -= OnDataContextChanged;
            Subscribe(null);
            button.Unloaded -= OnButtonUnloaded;
        }
    }
}
