using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Controls.Selection;
using Avalonia.Data;
using Avalonia.Data.Converters;
using EatConscious.Models;
using EatConscious.Views;

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
    /// <summary>
    /// TODO: Utilize in the future or redo
    /// cool methods like Select(index)...
    /// </summary>
    public SelectionModel<Ingredient> IngredientSelection { get; } = new();
    public void AddIngredientClick()
    {
        var window = new NewIngredientWindow(this);
        window.Show();
    }

    public ObservableCollection<Ingredient> Ingredients { get; init; } = new(IngredientsWrapper.StateOnLoad.UnwrapIngredients());
    
    /// <summary>
    /// These tags are shown when creating new ingredient, they do not limit already existing ones
    /// </summary>
    public ObservableCollection<string> Tags { get; } = IngredientsWrapper.StateOnLoad.Tags;

    /// <summary>
    /// Serializes tags and ingredients grouped by unit
    /// </summary>
    public IngredientsWrapper WrapIngredients()
    {
        var ingredientGroups = Ingredients
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