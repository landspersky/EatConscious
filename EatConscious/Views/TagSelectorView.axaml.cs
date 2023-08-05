using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace EatConscious.Views;

public partial class TagSelectorView : UserControl
{
    public TagSelectorView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}