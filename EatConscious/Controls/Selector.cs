using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
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
    
    /// <summary>
    /// This is for the template, use SelectedItems
    /// </summary>
    public static readonly DirectProperty<Selector, SelectionModel<string>> SelectionProperty =
        AvaloniaProperty.RegisterDirect<Selector, SelectionModel<string>>(
            nameof(ItemsSource),
            o => o.Selection,
            (o, v) => o.Selection = v,
            defaultBindingMode: BindingMode.OneTime);
    
    
    private SelectionModel<string> _selection = new () {SingleSelect = false};
    public SelectionModel<string> Selection
    {
        get => _selection;
        private set => SetAndRaise(SelectionProperty, ref _selection, value);
    }
    
    public static readonly DirectProperty<Selector, IReadOnlyList<string?>> SelectedItemsProperty =
        AvaloniaProperty.RegisterDirect<Selector, IReadOnlyList<string?>>(
            nameof(SelectedItems),
            o => o.SelectedItems,
            defaultBindingMode: BindingMode.OneWayToSource); // set through UI, sent to model
    
    public IReadOnlyList<string?> SelectedItems => _selection.SelectedItems;
}