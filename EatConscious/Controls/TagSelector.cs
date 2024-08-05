using Avalonia;
using Avalonia.Data;

namespace EatConscious.Controls;

/// <summary>
/// Control for selecting and creating tags
/// </summary>
/// <remarks>
/// The expected use case is
/// <code>&lt;TagSelector Tags="{...}" SelectedTags="{...}"/&gt;</code>
/// Other properties are for template binding in the style
/// </remarks>
public class TagSelector : Selector
{
    public static readonly DirectProperty<TagSelector, string?> NewTagNameProperty =
        AvaloniaProperty.RegisterDirect<TagSelector, string?>(
            nameof(NewTagName),
            o => o.NewTagName,
            (o, v) => o.NewTagName = v,
            defaultBindingMode: BindingMode.TwoWay);

    private string? _newTagName = "new tag";

    public string? NewTagName
    {
        get => _newTagName;
        set => SetAndRaise(NewTagNameProperty, ref _newTagName, value);
    }

    public void AddTag()
    {
        if (NewTagName is not null && !ItemsSource.Contains(NewTagName))
        {
            ItemsSource.Add(NewTagName);
        }

        NewTagName = "";
    }
}