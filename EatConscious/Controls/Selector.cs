using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace EatConscious.Controls;

public class BoolSelectionModeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool singleSelect)
        {
            return singleSelect
                ? SelectionMode.Single | SelectionMode.Toggle
                : SelectionMode.Multiple | SelectionMode.Toggle;
        }
        return new BindingNotification(new InvalidCastException(),
            BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

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

    private ObservableCollection<string> _selectedItems = new();
    public ObservableCollection<string> SelectedItems
    {
        get => _selectedItems;
        set => SetAndRaise(SelectedItemsProperty, ref _selectedItems, value);
    }

    public static readonly DirectProperty<Selector, bool> SingleSelectProperty =
        AvaloniaProperty.RegisterDirect<Selector, bool>(
            nameof(SingleSelect),
            o => o.SingleSelect,
            (o, v) => o.SingleSelect = v,
            defaultBindingMode: BindingMode.OneWay);
    
    private bool _singleSelect;
    public bool SingleSelect
    {
        get => _singleSelect;
        set => SetAndRaise(SingleSelectProperty, ref _singleSelect, value);
    }
}