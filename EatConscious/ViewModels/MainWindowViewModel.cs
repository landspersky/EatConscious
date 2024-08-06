using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            BindingErrorType.Error);
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
        _sourceCache.AddOrUpdate(State.OnLoad.Ingredients);
        
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
    public ObservableCollection<string> Tags { get; } = new(State.OnLoad.IngredientTags);
    
    /// <summary>
    /// Kategorie, podle kterých lze třídit
    /// </summary>
    public ObservableCollection<string> SortOptions { get; } = new(Enum.GetValues<Sorting>().Select(x => x.ToString()));
    
    private ObservableCollection<string> _selectedSorting = new();

    public ObservableCollection<string> SelectedSorting
    {
        get => _selectedSorting;
        set
        {
            _selectedSorting.CollectionChanged -= OnFilterTagsChanged;
            this.RaiseAndSetIfChanged(ref _selectedSorting, value);
            _selectedSorting.CollectionChanged += OnFilterTagsChanged;
        }
    }
    
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
        Sorting? sorting = SelectedSorting.FirstOrDefault()?.ToSorting();

        var changeSet = _sourceCache.Connect()
                                                           .Filter(x => !FilteredTags.Except(x.Tags).Any());

        if (sorting is { } s)
        {
            changeSet = changeSet.Sort(s.GetComparer());
        }
        changeSet.Bind(out _ingredients)
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
            Tags = Tags.ToList(),
            Ingredients = ingredientGroups.ToList()
        };
    }
}

public enum Sorting
{
    Name,
    Price,
    Kcal,
    Protein,
    Carbs,
    Fats
}

public static class SortingExtensions
{
    private static readonly Dictionary<Sorting, Comparison<Ingredient>> CompareFunctions = new()
    {
        { Sorting.Name, (x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal) },
        { Sorting.Price, (x, y) => x.Price.CompareTo(y.Price) },
        { Sorting.Kcal, (x, y) => x.Nutrients.Kcal.CompareTo(y.Nutrients.Kcal) },
        { Sorting.Protein, (x, y) => x.Nutrients.Protein.CompareTo(y.Nutrients.Protein) },
        { Sorting.Carbs, (x, y) => x.Nutrients.Carbs.CompareTo(y.Nutrients.Carbs) },
        { Sorting.Fats, (x, y) => x.Nutrients.Fats.CompareTo(y.Nutrients.Fats) },
    };

    public static IComparer<Ingredient> GetComparer(this Sorting s)
    {
        return Comparer<Ingredient>.Create(CompareFunctions[s]);
    }

    private static readonly Dictionary<string, Sorting> SortingByName =
        Enum.GetValues<Sorting>().ToDictionary(k => k.ToString(), v => v);

    public static Sorting ToSorting(this string s) => SortingByName[s];
}