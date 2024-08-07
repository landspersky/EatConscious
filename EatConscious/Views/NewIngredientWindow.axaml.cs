using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using EatConscious.Models;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewIngredientWindow : Window
{
    /// <summary>
    /// Opened for creating a new ingredient
    /// </summary>
    public NewIngredientWindow(MainWindowViewModel model)
    {
        InitializeComponent();
        var ingredientModel = new NewIngredientViewModel(model);
        DataContext = ingredientModel;
    }

    /// <summary>
    /// Opened for editing an ingredient
    /// </summary>
    public NewIngredientWindow(MainWindowViewModel model, Ingredient ingredient)
    {
        InitializeComponent();
        var ingredientModel = new NewIngredientViewModel(model, ingredient);
        DataContext = ingredientModel;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}