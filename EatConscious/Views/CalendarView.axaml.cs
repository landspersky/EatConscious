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

        button.DataContextChanged += (_, _) =>
        {
            if (button.DataContext is CalendarDayViewModel vm)
            {
                vm.PropertyChanged += OnDayViewModelPropertyChanged;
            }
        };

        if (button.DataContext is CalendarDayViewModel currentVm)
        {
            currentVm.PropertyChanged += OnDayViewModelPropertyChanged;
        }

        void OnDayViewModelPropertyChanged(object? s, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CalendarDayViewModel.IsPickerOpen)
                && s is CalendarDayViewModel { IsPickerOpen: false })
            {
                flyout.Hide();
            }
        }
    }
}
