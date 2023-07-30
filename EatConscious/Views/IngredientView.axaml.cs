using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EatConscious.Views;

public partial class IngredientView : UserControl
{
    public IngredientView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}