using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using EatConscious.ViewModels;

namespace EatConscious.Views;

public partial class NewIngredientWindow : Window
{
    public NewIngredientWindow()
    {
        InitializeComponent();
        DataContext = new NewIngredientViewModel(this.Find<ComboBox>("SelectUnit"));
#if DEBUG
        this.AttachDevTools();
#endif
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}