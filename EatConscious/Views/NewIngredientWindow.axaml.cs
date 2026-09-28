using Avalonia.Controls;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewIngredientWindow : Window
{
    /// <summary>
    /// Opened for creating or editing an ingredient, based on the view model
    /// </summary>
    public NewIngredientWindow(NewIngredientViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
