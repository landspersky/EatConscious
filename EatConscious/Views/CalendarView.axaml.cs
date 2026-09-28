using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class CalendarView : UserControl
{
    public CalendarView()
    {
        InitializeComponent();
    }

    private void DayCell_OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        // Double-clicking the "+", trash or "..." buttons (or the recipe picker) shouldn't also open the detail
        if (e.Source is Visual source && source.FindAncestorOfType<Button>(includeSelf: true) != null)
        {
            return;
        }

        if (sender is Control { DataContext: CalendarDayViewModel day })
        {
            day.OpenDetailCommand.Execute().Subscribe();
        }
    }
}
