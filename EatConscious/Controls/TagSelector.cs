using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
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
public class TagSelector : TemplatedControl
{
    public static readonly DirectProperty<TagSelector, ObservableCollection<string>> TagsProperty =
        AvaloniaProperty.RegisterDirect<TagSelector, ObservableCollection<string>>(
            nameof(Tags),
            o => o.Tags,
            (o, v) => o.Tags = v,
            defaultBindingMode: BindingMode.TwoWay);

    // the default should not be seen in the app, only through IDE designer
    private ObservableCollection<string> _tags = new() {"default", "tags"};

    public ObservableCollection<string> Tags
    {
        get => _tags;
        private set => SetAndRaise(TagsProperty, ref _tags, value);
    }
    
    public static readonly DirectProperty<TagSelector, SelectionModel<string>> TagSelectionProperty =
        AvaloniaProperty.RegisterDirect<TagSelector, SelectionModel<string>>(
            nameof(Tags),
            o => o.TagSelection,
            (o, v) => o.TagSelection = v,
            defaultBindingMode: BindingMode.OneTime); // this is for the template, use SelectedTags
    
    private SelectionModel<string> _tagSelection = new () {SingleSelect = false};
    
    public SelectionModel<string> TagSelection
    {
        get => _tagSelection;
        private set => SetAndRaise(TagSelectionProperty, ref _tagSelection, value);
    }

    public static readonly DirectProperty<TagSelector, IReadOnlyList<string?>> SelectedTagsProperty =
        AvaloniaProperty.RegisterDirect<TagSelector, IReadOnlyList<string?>>(
            nameof(SelectedTags),
            o => o.SelectedTags,
            defaultBindingMode: BindingMode.OneWayToSource); // set through UI, sent to model
    
    public IReadOnlyList<string?> SelectedTags => _tagSelection.SelectedItems;

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
        if (NewTagName is not null && !Tags.Contains(NewTagName))
        {
            Tags.Add(NewTagName);
        }
        NewTagName = "";
    }
}