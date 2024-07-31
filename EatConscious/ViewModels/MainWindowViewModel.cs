using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;
using DynamicData;
using EatConscious.Models;
using EatConscious.Views;
using ReactiveUI;

namespace EatConscious.ViewModels;

/// <summary>
/// Displays nicely the base value from <see cref="Measure"/>
/// </summary>
public class MeasureStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Measure unit)
        {
            return $"per {unit.BaseValue}{unit.Id}";
        }
        return new BindingNotification(new InvalidCastException(),
            BindingErrorType.Error);;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        _sourceCache.AddOrUpdate(IngredientsWrapper.StateOnLoad.UnwrapIngredients());
        
        _sourceCache.Connect()
                    .Bind(out _ingredients)
                    .Subscribe();
    }
  
    /// <summary>
    /// Keeps the ingredients cached, helpful for sorting and filtering
    /// </summary>
    private readonly SourceCache<Ingredient, string> _sourceCache = new (x => x.Name);
    
    
    private ReadOnlyObservableCollection<Ingredient> _ingredients;
    /// <summary>
    /// Source collection for the ingredient form
    /// </summary>
    public ReadOnlyObservableCollection<Ingredient> Ingredients => _ingredients;

    
    private ObservableCollection<string> _filteredTags = new();
    public ObservableCollection<string> FilteredTags
    {
        get => _filteredTags;
        set
        {
            _filteredTags.CollectionChanged -= OnFilterTagsChanged;
            this.RaiseAndSetIfChanged(ref _filteredTags, value);
            _filteredTags.CollectionChanged += OnFilterTagsChanged;
        }
    }
    
    /// <summary>
    /// These tags are shown when creating new ingredient, they do not limit already existing ones
    /// </summary>
    public ObservableCollection<string> Tags { get; } = IngredientsWrapper.StateOnLoad.Tags;

    public void AddIngredientClick()
    {
        var window = new NewIngredientWindow(this);
        window.Show();
    }
    
    /// <summary>
    /// Adds or updates (based on name) the ingredients collection
    /// </summary>
    public void AddOrUpdate(Ingredient ingredient) => _sourceCache.AddOrUpdate(ingredient);

    private void OnFilterTagsChanged(object? sender, EventArgs e)
    {
        _sourceCache.Connect()
                    .Filter(x => !FilteredTags.Except(x.Tags).Any())
                    .Bind(out _ingredients)
                    .Subscribe();
        
        this.RaisePropertyChanged(nameof(Ingredients));
    }

    /// <summary>
    /// Serializes tags and ingredients grouped by unit
    /// </summary>
    public IngredientsWrapper WrapIngredients()
    {
        var ingredientGroups = _sourceCache.Items
            .GroupBy(x => x.Unit.Id)
            .Select(g => new IngredientsWrapper.IngredientsWithMeasure()
            {
                Unit = g.Key,
                List = g.Select(x => x.Strip()).ToList()
            });
        
        return new IngredientsWrapper()
        {
            Tags = Tags,
            Ingredients = ingredientGroups.ToList()
        };
    }
}