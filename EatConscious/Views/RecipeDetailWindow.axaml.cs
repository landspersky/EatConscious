using Avalonia.Controls;
using EatConscious.Models;

namespace EatConscious.Views;

public partial class RecipeDetailWindow : Window
{
    public RecipeDetailWindow(Recipe recipe)
    {
        InitializeComponent();
        DataContext = recipe;
    }
}
