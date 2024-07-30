using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace EatConscious.Controls;

public class Selector : TemplatedControl
{
    public static readonly DirectProperty<Selector, ObservableCollection<string>> ItemsSourceProperty =
        AvaloniaProperty.RegisterDirect<Selector, ObservableCollection<string>>(
            nameof(ItemsSource),
            o => o.ItemsSource,
            (o, v) => o.ItemsSource = v,
            defaultBindingMode: BindingMode.TwoWay);
    
    private ObservableCollection<string> _itemsSource = ["one", "two"];
    public ObservableCollection<string> ItemsSource
    {
        get => _itemsSource;
        private set => SetAndRaise(ItemsSourceProperty, ref _itemsSource, value);
    }
    
    public static readonly DirectProperty<Selector, ObservableCollection<string>> SelectedItemsProperty =
        AvaloniaProperty.RegisterDirect<Selector, ObservableCollection<string>>(
            nameof(SelectedItems),
            o => o.SelectedItems,
            (o, v) => o.SelectedItems = v,
            defaultBindingMode: BindingMode.OneWayToSource);

    private ObservableCollection<string> _selectedItems = ["one"];
    public ObservableCollection<string> SelectedItems
    {
        get => _selectedItems;
        set => SetAndRaise(SelectedItemsProperty, ref _selectedItems, value);
    }
}