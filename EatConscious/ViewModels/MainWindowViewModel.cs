using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reactive;
using Avalonia.Data;
using Avalonia.Data.Converters;
using DynamicData;
using ReactiveUI;
using EatConscious.Models;
using EatConscious.Views;
using EatConscious.Wrappers;

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
        _ingredientCache.AddOrUpdate(State.OnLoad.Ingredients);
        
        _ingredientCache.Connect()
                        .Bind(out _ingredients)
                        .Subscribe();
        
        _recipeCache.AddOrUpdate((State.OnLoad.Recipes));

        _recipeCache.Connect()
                    .Bind(out _recipes)
                    .Subscribe();

        EditIngredientCommand = ReactiveCommand.Create<Ingredient>(Edit);
        
        DeleteIngredientCommand = ReactiveCommand.Create<Ingredient>(Delete);
        DeleteRecipeCommand = ReactiveCommand.Create<Recipe>(Delete);
    }
    
    /// <summary>
    /// Categories to sort on, common to both ingredients and recipes
    /// </summary>
    public ObservableCollection<string> SortOptions { get; } = new(Enum.GetValues<Sorting>().Select(x => x.ToString()));

    #region INGREDIENTS
    /// <summary>
    /// Keeps the ingredients cached, helpful for sorting and filtering
    /// </summary>
    private readonly SourceCache<Ingredient, int> _ingredientCache = new (x => x.Id);
    
    private ReadOnlyObservableCollection<Ingredient> _ingredients;
    /// <summary>
    /// Source collection for the ingredient form
    /// </summary>
    public ReadOnlyObservableCollection<Ingredient> Ingredients => _ingredients;
    
    private ObservableCollection<string> _ingredientFilters = new();
    /// <summary>
    /// Tags for filtering ingredients
    /// </summary>
    public ObservableCollection<string> IngredientFilters
    {
        get => _ingredientFilters;
        set
        {
            _ingredientFilters.CollectionChanged -= UpdateIngredientSelection;
            this.RaiseAndSetIfChanged(ref _ingredientFilters, value);
            _ingredientFilters.CollectionChanged += UpdateIngredientSelection;
        }
    }
    
    /// <summary>
    /// Available tags for assigning to new ingredients
    /// </summary>
    public ObservableCollection<string> IngredientTags { get; } = new(State.OnLoad.IngredientTags);
    
    private ObservableCollection<string> _ingredientSorting = new();
    /// <summary>
    /// Selected parameters for sorting
    /// </summary>
    /// <remarks>Should be just one value but bound to SelectedItems for ease and future extension</remarks>
    public ObservableCollection<string> IngredientSorting
    {
        get => _ingredientSorting;
        set
        {
            _ingredientSorting.CollectionChanged -= UpdateIngredientSelection;
            this.RaiseAndSetIfChanged(ref _ingredientSorting, value);
            _ingredientSorting.CollectionChanged += UpdateIngredientSelection;
        }
    }

    /// <summary>
    /// Reflects changes made in filters and sorts
    /// </summary>
    private void UpdateIngredientSelection(object? sender, EventArgs e)
    {
        Sorting? sorting = IngredientSorting.FirstOrDefault()?.ToSorting();

        var changeSet = _ingredientCache.Connect()
            .Filter(x => !IngredientFilters.Except(x.Tags).Any());

        if (sorting is { } s)
        {
            changeSet = changeSet.Sort(s.GetComparer());
        }
        changeSet.Bind(out _ingredients)
                 .Subscribe();

        this.RaisePropertyChanged(nameof(Ingredients));
    }
    
    /// <summary>
    /// Opens <see cref="NewIngredientWindow"/>
    /// </summary>
    public void AddIngredientClick()
    {
        var window = new NewIngredientWindow(this);
        window.Show();
    }
    
    /// <summary>
    /// Adds or updates (based on id) the ingredients collection
    /// </summary>
    public void AddOrUpdate(Ingredient ingredient) => _ingredientCache.AddOrUpdate(ingredient);
    
    /// <summary>
    /// Command for editing existing Ingredient
    /// </summary>
    public ReactiveCommand<Ingredient, Unit> EditIngredientCommand { get; }

    private void Edit(Ingredient ingredient)
    {
        var window = new NewIngredientWindow(this, ingredient);
        window.Show();
        Console.WriteLine(ingredient);
    }

    /// <summary>
    /// Command for deleting from the ingredients collection
    /// </summary>
    public ReactiveCommand<Ingredient, Unit> DeleteIngredientCommand { get; }
    
    private void Delete(Ingredient ingredient) => _ingredientCache.Remove(ingredient);
    #endregion

    #region RECIPES
    /// <summary>
    /// Keeps the recipes cached, helpful for sorting and filtering
    /// </summary>
    private readonly SourceCache<Recipe, int> _recipeCache = new (x => x.Id);

    private ReadOnlyObservableCollection<Recipe> _recipes;
    /// <summary>
    /// Source collection for the recipe form
    /// </summary>
    public ReadOnlyObservableCollection<Recipe> Recipes => _recipes;

    private ObservableCollection<string> _recipeFilters = new();
    /// <summary>
    /// Tags for filtering recipes
    /// </summary>
    public ObservableCollection<string> RecipeFilters
    {
        get => _recipeFilters;
        set
        {
            _recipeFilters.CollectionChanged -= UpdateRecipeSelection;
            this.RaiseAndSetIfChanged(ref _recipeFilters, value);
            _recipeFilters.CollectionChanged += UpdateRecipeSelection;
        }
    }
    
    /// <summary>
    /// Available tags for assigning to new recipes
    /// </summary>
    public ObservableCollection<string> RecipeTags { get; } = new(State.OnLoad.RecipeTags);

    private ObservableCollection<string> _recipeSorting = new();
    /// <summary>
    /// Selected parameters for sorting
    /// </summary>
    /// <remarks>Should be just one value but bound to SelectedItems for ease and future extension</remarks>
    public ObservableCollection<string> RecipeSorting
    {
        get => _recipeSorting;
        set
        {
            _recipeSorting.CollectionChanged -= UpdateRecipeSelection;
            this.RaiseAndSetIfChanged(ref _recipeSorting, value);
            _recipeSorting.CollectionChanged += UpdateRecipeSelection;
        }
    }

    /// <summary>
    /// Reflects changes made in filters and sorts
    /// </summary>
    private void UpdateRecipeSelection(object? sender, EventArgs e)
    {
        Sorting? sorting = RecipeSorting.FirstOrDefault()?.ToSorting();

        var changeSet = _recipeCache.Connect()
            .Filter(x => !RecipeFilters.Except(x.Tags).Any());

        if (sorting is { } s)
        {
            changeSet = changeSet.Sort(s.GetComparer());
        }
        changeSet.Bind(out _recipes)
                 .Subscribe();

        this.RaisePropertyChanged(nameof(Recipes));
    }

    /// <summary>
    /// Opens <see cref="NewRecipeWindow"/>
    /// </summary>
    public void AddRecipeClick()
    {
        var window = new NewRecipeWindow(this);
        window.Show();
    }
    
    /// <summary>
    /// Adds or updates (based on id) the recipe collection
    /// </summary>
    public void AddOrUpdate(Recipe recipe) => _recipeCache.AddOrUpdate(recipe);
    
    /// <summary>
    /// Command for deleting from the recipe collection
    /// </summary>
    public ReactiveCommand<Recipe, Unit> DeleteRecipeCommand { get; }

    private void Delete(Recipe recipe) => _recipeCache.Remove(recipe);
    #endregion

    /// <summary>
    /// Serializes tags and ingredients grouped by unit
    /// </summary>
    public Wrappers.IngredientsWrapper WrapIngredients()
    {
        var ingredientGroups = _ingredientCache.Items
            .GroupBy(x => x.Unit.Id)
            .Select(g => new IngredientsWrapper.IngredientsWithMeasure()
            {
                Unit = g.Key,
                List = g.Select(x => x.Strip()).ToList()
            });
        
        return new IngredientsWrapper()
        {
            Tags = IngredientTags.ToList(),
            Ingredients = ingredientGroups.ToList()
        };
    }

    public Wrappers.RecipeWrapper WrapRecipes()
    {
        return new RecipeWrapper()
        {
            Tags = RecipeTags.ToList(),
            Recipes = Recipes.Select(x => x.Strip()).ToList(),
        };
    }
}

/// <summary>
/// Possible values to sort on
/// </summary>
public enum Sorting
{
    Name,
    Price,
    Kcal,
    Protein,
    Carbs,
    Fats
}

/// <summary>
/// Exposes all fields from <see cref="Sorting"/> for common comparing functions
/// </summary>
public interface ISortable
{
    string Name { get; }
    double Price { get; }
    Nutrients Nutrients { get; }
}

public static class SortingExtensions
{
    private static readonly Dictionary<Sorting, Comparison<ISortable>> CompareFunctions = new()
    {
        { Sorting.Name, (x, y) => string.Compare(x.Name, y.Name, StringComparison.InvariantCultureIgnoreCase) },
        { Sorting.Price, (x, y) => x.Price.CompareTo(y.Price) },
        { Sorting.Kcal, (x, y) => x.Nutrients.Kcal.CompareTo(y.Nutrients.Kcal) },
        { Sorting.Protein, (x, y) => x.Nutrients.Protein.CompareTo(y.Nutrients.Protein) },
        { Sorting.Carbs, (x, y) => x.Nutrients.Carbs.CompareTo(y.Nutrients.Carbs) },
        { Sorting.Fats, (x, y) => x.Nutrients.Fats.CompareTo(y.Nutrients.Fats) },
    };

    /// <summary>
    /// Gets comparer based on the value passed in the parameter
    /// </summary>
    public static IComparer<ISortable> GetComparer(this Sorting s)
    {
        return Comparer<ISortable>.Create(CompareFunctions[s]);
    }

    private static readonly Dictionary<string, Sorting> SortingByName =
        Enum.GetValues<Sorting>().ToDictionary(k => k.ToString(), v => v);

    /// <summary>
    /// Inverse function to ToString
    /// </summary>
    public static Sorting ToSorting(this string s) => SortingByName[s];
}