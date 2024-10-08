using System;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using EatConscious.ViewModels;
using EatConscious.Views;

namespace EatConscious;

public partial class App : Application
{
    /// <summary>
    /// Path of file containing ingredients and their tags
    /// </summary>
    public const string IngredientsPath = "ingredients.json";

    /// <summary>
    /// Path of file containing recipes and their tags
    /// </summary>
    public const string RecipePath = "recipes.json";
    
    private readonly MainWindowViewModel _mainModel = new();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _mainModel,
            };
            desktop.Exit += SerializeData;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SerializeData(object? sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        string json = JsonSerializer.Serialize(_mainModel.WrapIngredients());
        using (var sw = new StreamWriter(IngredientsPath, false))
        {
            sw.Write(json);
        }

        json = JsonSerializer.Serialize(_mainModel.WrapRecipes());
        using (var sw = new StreamWriter(RecipePath, false))
        {
            sw.Write(json);
        }
    }
}