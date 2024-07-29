using Avalonia;
using Avalonia.Data;

namespace EatConscious.Controls;

public class PopupSelector : Selector
{
    public static readonly DirectProperty<PopupSelector, string?> ContentProperty =
        AvaloniaProperty.RegisterDirect<PopupSelector, string?>(
            nameof(Content),
            o => o.Content,
            (o, v) => o.Content = v,
            defaultBindingMode: BindingMode.TwoWay);
    
    public static readonly DirectProperty<PopupSelector, bool> DropdownOpenProperty =
        AvaloniaProperty.RegisterDirect<PopupSelector, bool>(
            nameof(DropdownOpen),
            o => o.DropdownOpen,
            (o, v) => o.DropdownOpen = v,
            defaultBindingMode: BindingMode.TwoWay);
    
    private string? _content = "drop down";
    public string? Content
    {
        get => _content;
        set => SetAndRaise(ContentProperty, ref _content, value);
    }
    
    private bool _dropdownOpen;
    public bool DropdownOpen
    {
        get => _dropdownOpen;
        set => SetAndRaise(DropdownOpenProperty, ref _dropdownOpen, value);
    }

    public void Click() => DropdownOpen = !DropdownOpen;
}