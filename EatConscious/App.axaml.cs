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
    public static string IngredientsPath = "ingredients.json";
    private MainWindowViewModel _mainModel;
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        _mainModel = new MainWindowViewModel();
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
    }
}