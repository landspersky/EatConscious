using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using DynamicData;
using ReactiveUI;
using EatConscious.Models;
using EatConscious.Wrappers;

namespace EatConscious.ViewModels;

/// <summary>
/// The ingredients tab; a colleague of <see cref="IMainMediator"/>
/// </summary>
public class IngredientsTabViewModel : ViewModelBase
{
    private readonly IMainMediator _mediator;

    public IngredientsTabViewModel(IMainMediator mediator, IEnumerable<Ingredient> ingredients, IEnumerable<string> tags)
    {
        _mediator = mediator;
        Tags = new(tags);

        _ingredientCache.AddOrUpdate(ingredients);

        _ingredientCache.Connect()
                        .Bind(out _ingredients)
                        .Subscribe();

        _ingredientCache.Connect()
                        .Bind(out _allIngredients)
                        .Subscribe();

        AddCommand = ReactiveCommand.Create(() => _mediator.OpenIngredientEditor(null));
        EditCommand = ReactiveCommand.Create<Ingredient>(ingredient => _mediator.OpenIngredientEditor(ingredient));
        DeleteCommand = ReactiveCommand.Create<Ingredient>(Delete);
        OnCheckCommand = ReactiveCommand.Create<Ingredient>(ingredient =>
            _mediator.IngredientCheckChanged(ingredient, ingredient.IsChecked));
    }

    /// <summary>
    /// Keeps the ingredients cached, helpful for sorting and filtering
    /// </summary>
    private readonly SourceCache<Ingredient, int> _ingredientCache = new (x => x.Id);

    private ReadOnlyObservableCollection<Ingredient> _ingredients;
    /// <summary>
    /// Source collection for the ingredient form
    /// </summary>
    public ReadOnlyObservableCollection<Ingredient> Ingredients => _ingredients;

    private readonly ReadOnlyObservableCollection<Ingredient> _allIngredients;
    /// <summary>
    /// Every ingredient, unaffected by filters
    /// </summary>
    public ReadOnlyObservableCollection<Ingredient> AllIngredients => _allIngredients;

    /// <summary>
    /// Available tags for assigning to ingredients
    /// </summary>
    public ObservableCollection<string> Tags { get; }

    /// <summary>
    /// Categories to sort on
    /// </summary>
    public ObservableCollection<string> SortOptions { get; } = new(Enum.GetValues<Sorting>().Select(x => x.ToString()));

    private ObservableCollection<string> _filters = new();
    /// <summary>
    /// Tags for filtering ingredients
    /// </summary>
    public ObservableCollection<string> Filters
    {
        get => _filters;
        set
        {
            _filters.CollectionChanged -= UpdateSelection;
            this.RaiseAndSetIfChanged(ref _filters, value);
            _filters.CollectionChanged += UpdateSelection;
        }
    }

    private ObservableCollection<string> _sorting = new();
    /// <summary>
    /// Selected parameters for sorting
    /// </summary>
    /// <remarks>Should be just one value but bound to SelectedItems for ease and future extension</remarks>
    public ObservableCollection<string> SortBy
    {
        get => _sorting;
        set
        {
            _sorting.CollectionChanged -= UpdateSelection;
            this.RaiseAndSetIfChanged(ref _sorting, value);
            _sorting.CollectionChanged += UpdateSelection;
        }
    }

    /// <summary>
    /// Reflects changes made in filters and sorts
    /// </summary>
    private void UpdateSelection(object? sender, EventArgs e)
    {
        Sorting? sorting = SortBy.FirstOrDefault()?.ToSorting();

        var changeSet = _ingredientCache.Connect()
            .Filter(x => !Filters.Except(x.Tags).Any());

        if (sorting is { } s)
        {
            changeSet = changeSet.Sort(s.GetComparer());
        }
        changeSet.Bind(out _ingredients)
                 .Subscribe();

        this.RaisePropertyChanged(nameof(Ingredients));
    }

    /// <summary>
    /// Opens the editor for a new ingredient
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddCommand { get; }

    /// <summary>
    /// Command for editing existing Ingredient
    /// </summary>
    public ReactiveCommand<Ingredient, Unit> EditCommand { get; }

    /// <summary>
    /// Command for deleting from the ingredients collection
    /// </summary>
    public ReactiveCommand<Ingredient, Unit> DeleteCommand { get; }

    /// <summary>
    /// The ingredient's check box was toggled; used for filtering recipes
    /// </summary>
    public ReactiveCommand<Ingredient, Unit> OnCheckCommand { get; }

    /// <summary>
    /// Adds or updates (based on id) the ingredients collection
    /// </summary>
    /// <remarks>An edited ingredient is a new object, so it takes over the check state of the old one</remarks>
    public void AddOrUpdate(Ingredient ingredient)
    {
        var existing = _ingredientCache.Lookup(ingredient.Id);
        if (existing.HasValue)
        {
            ingredient.IsChecked = existing.Value.IsChecked;
        }
        _ingredientCache.AddOrUpdate(ingredient);
    }

    private void Delete(Ingredient ingredient)
    {
        _ingredientCache.Remove(ingredient);
        _mediator.IngredientDeleted(ingredient);
    }

    /// <summary>
    /// Serializes tags and ingredients grouped by unit
    /// </summary>
    public IngredientsWrapper WrapIngredients()
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
            Tags = Tags.ToList(),
            Ingredients = ingredientGroups.ToList()
        };
    }
}
