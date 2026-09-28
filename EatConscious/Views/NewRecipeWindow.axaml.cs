using Avalonia.Controls;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewRecipeWindow : Window
{
    /// <summary>
    /// Opened for creating or editing a recipe, based on the view model
    /// </summary>
    public NewRecipeWindow(NewRecipeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
