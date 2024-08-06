using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewRecipeWindow : Window
{
    public NewRecipeWindow(MainWindowViewModel model)
    {
        InitializeComponent();
        var recipeModel = new NewRecipeViewModel(model);
        DataContext = recipeModel;
    }
}