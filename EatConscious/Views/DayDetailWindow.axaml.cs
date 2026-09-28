using Avalonia.Controls;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class DayDetailWindow : Window
{
    public DayDetailWindow(DayDetailViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
