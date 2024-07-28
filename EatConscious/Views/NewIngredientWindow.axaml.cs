using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewIngredientWindow : Window
{
    public NewIngredientWindow(MainWindowViewModel model)
    {
        InitializeComponent();
        var ingredientModel = new NewIngredientViewModel(model);
        DataContext = ingredientModel;
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}