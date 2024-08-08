using Avalonia.Controls;
using EatConscious.Models;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewRecipeWindow : Window
{
    /// <summary>
    /// Opened for creating a new recipe
    /// </summary>
    public NewRecipeWindow(MainWindowViewModel model)
    {
        InitializeComponent();
        var recipeModel = new NewRecipeViewModel(model);
        DataContext = recipeModel;
    }

    /// <summary>
    /// Opened for editing a recipe
    /// </summary>
    public NewRecipeWindow(MainWindowViewModel model, Recipe recipe)
    {
        InitializeComponent();
        var recipeModel = new NewRecipeViewModel(model, recipe);
        DataContext = recipeModel;
    }
}