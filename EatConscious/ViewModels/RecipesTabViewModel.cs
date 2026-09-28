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
/// The recipes tab; a colleague of <see cref="IMainMediator"/>
/// </summary>
public class RecipesTabViewModel : ViewModelBase
{
    private readonly IMainMediator _mediator;

    public RecipesTabViewModel(IMainMediator mediator, IEnumerable<Recipe> recipes, IEnumerable<string> tags)
    {
        _mediator = mediator;
        Tags = new(tags);

        _recipeCache.AddOrUpdate(recipes);

        _recipeCache.Connect()
                    .Bind(out _recipes)
                    .Subscribe();

        _recipeCache.Connect()
                    .Bind(out _allRecipes)
                    .Subscribe();

        AddCommand = ReactiveCommand.Create(() => _mediator.OpenRecipeEditor(null));
        EditCommand = ReactiveCommand.Create<Recipe>(recipe => _mediator.OpenRecipeEditor(recipe));
        DeleteCommand = ReactiveCommand.Create<Recipe>(Delete);
    }

    /// <summary>
    /// Keeps the recipes cached, helpful for sorting and filtering
    /// </summary>
    private readonly SourceCache<Recipe, int> _recipeCache = new (x => x.Id);

    private ReadOnlyObservableCollection<Recipe> _recipes;
    /// <summary>
    /// Source collection for the recipe form
    /// </summary>
    public ReadOnlyObservableCollection<Recipe> Recipes => _recipes;

    private readonly ReadOnlyObservableCollection<Recipe> _allRecipes;
    /// <summary>
    /// Every recipe, unaffected by filters
    /// </summary>
    public ReadOnlyObservableCollection<Recipe> AllRecipes => _allRecipes;

    /// <summary>
    /// Available tags for assigning to recipes
    /// </summary>
    public ObservableCollection<string> Tags { get; }

    /// <summary>
    /// Categories to sort on
    /// </summary>
    public ObservableCollection<string> SortOptions { get; } = new(Enum.GetValues<Sorting>().Select(x => x.ToString()));

    private ObservableCollection<string> _filters = new();
    /// <summary>
    /// Tags for filtering recipes
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

    private ObservableCollection<string> _sortBy = new();
    /// <summary>
    /// Selected parameters for sorting
    /// </summary>
    /// <remarks>Should be just one value but bound to SelectedItems for ease and future extension</remarks>
    public ObservableCollection<string> SortBy
    {
        get => _sortBy;
        set
        {
            _sortBy.CollectionChanged -= UpdateSelection;
            this.RaiseAndSetIfChanged(ref _sortBy, value);
            _sortBy.CollectionChanged += UpdateSelection;
        }
    }

    /// <summary>
    /// Ids of the ingredients checked in the ingredients tab; recipes with at least one of them are shown
    /// </summary>
    /// <remarks>Ids rather than objects, so the filter survives editing the ingredient</remarks>
    private readonly HashSet<int> _checkedIngredients = new();

    /// <summary>
    /// Includes or excludes the ingredient from the filter
    /// </summary>
    public void SetIngredientChecked(Ingredient ingredient, bool isChecked)
    {
        if (isChecked)
        {
            _checkedIngredients.Add(ingredient.Id);
        }
        else
        {
            _checkedIngredients.Remove(ingredient.Id);
        }

        UpdateSelection(null, EventArgs.Empty);
    }

    /// <summary>
    /// Reflects changes made in filters and sorts
    /// </summary>
    private void UpdateSelection(object? sender, EventArgs e)
    {
        Sorting? sorting = SortBy.FirstOrDefault()?.ToSorting();

        var changeSet = _recipeCache.Connect()
            .Filter(x => !Filters.Except(x.Tags).Any()
            // the checked ingredients are either none or they have common elements with recipe ingredient
            && (!_checkedIngredients.Any()
                || x.Ingredients.Any(ing => _checkedIngredients.Contains(ing.Ingredient.Id))));

        if (sorting is { } s)
        {
            changeSet = changeSet.Sort(s.GetComparer());
        }
        changeSet.Bind(out _recipes)
                 .Subscribe();

        this.RaisePropertyChanged(nameof(Recipes));
    }

    /// <summary>
    /// Opens the editor for a new recipe
    /// </summary>
    public ReactiveCommand<Unit, Unit> AddCommand { get; }

    /// <summary>
    /// Command for editing existing Recipe
    /// </summary>
    public ReactiveCommand<Recipe, Unit> EditCommand { get; }

    /// <summary>
    /// Command for deleting from the recipe collection
    /// </summary>
    public ReactiveCommand<Recipe, Unit> DeleteCommand { get; }

    /// <summary>
    /// Adds or updates (based on id) the recipe collection
    /// </summary>
    public void AddOrUpdate(Recipe recipe) => _recipeCache.AddOrUpdate(recipe);

    private void Delete(Recipe recipe)
    {
        _recipeCache.Remove(recipe);
        _mediator.RecipeDeleted(recipe);
    }

    /// <summary>
    /// Points the recipes using the ingredient to its new version, so they don't show stale values
    /// </summary>
    public void ReplaceIngredient(Ingredient ingredient)
    {
        ReplaceRecipesUsing(ingredient.Id, portions => portions.Select(p => p.Ingredient.Id == ingredient.Id
            ? new IngredientPortion { Ingredient = ingredient, Value = p.Value }
            : p));
    }

    /// <summary>
    /// Drops the ingredient from every recipe and from the filter
    /// </summary>
    /// <remarks>Otherwise the recipes would reference an ingredient that no longer exists after save + load</remarks>
    public void RemoveIngredient(Ingredient ingredient)
    {
        ReplaceRecipesUsing(ingredient.Id, portions => portions.Where(p => p.Ingredient.Id != ingredient.Id));
        SetIngredientChecked(ingredient, false);
    }

    /// <summary>
    /// Creates a new version of every recipe containing the ingredient and announces it as saved
    /// </summary>
    private void ReplaceRecipesUsing(int ingredientId, Func<IEnumerable<IngredientPortion>, IEnumerable<IngredientPortion>> change)
    {
        var affected = _recipeCache.Items
            .Where(r => r.Ingredients.Any(p => p.Ingredient.Id == ingredientId))
            .ToList();

        foreach (var recipe in affected)
        {
            _mediator.RecipeSaved(new Recipe()
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Ingredients = change(recipe.Ingredients).ToList(),
                Note = recipe.Note,
                Tags = recipe.Tags,
            });
        }
    }

    public RecipeWrapper WrapRecipes()
    {
        return new RecipeWrapper()
        {
            Tags = Tags.ToList(),
            Recipes = _recipeCache.Items.Select(x => x.Strip()).ToList(),
        };
    }
}
