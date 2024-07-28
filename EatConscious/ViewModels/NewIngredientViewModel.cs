using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using EatConscious.Models;
using ReactiveUI;

namespace EatConscious.ViewModels;

public class NewIngredientViewModel : ViewModelBase
{
    public string Name { get; set; } = "Onion";

    private readonly MainWindowViewModel _mainModel;

    public double Kcal { get; set; }
    public double Protein { get; set; } 
    public double Carbs { get; set; }
    public double Fats { get; set; }
    public double NutrientBase { get; set; } = 1;
    public double Price { get; set; }
    public double PriceBase { get; set; } = 1;
    
    public List<Measure> Units { get; set; } = Measure.All().ToList();

    public Measure SelectedUnit { get; set; } = Measure.Gram;
    
    public ObservableCollection<string> Tags { get; }

    public IReadOnlyList<string> SelectedTags { get; set; } = new List<string>();
    
    /// <returns>Ingredient created from the input fields data</returns>
    private Ingredient CreateIngredient()
    {
        Nutrients nutrients = new()
        {
            Kcal = Kcal,
            Protein = Protein,
            Carbs = Carbs,
            Fats = Fats
        };
        return Ingredient.Convert(
            Name, nutrients, NutrientBase, Price, PriceBase, SelectedUnit, SelectedTags.ToList());
    }

    public void AddClick()
    {
        _mainModel.Ingredients.Add(CreateIngredient());
        _mainModel.RaisePropertyChanged(nameof(_mainModel.Ingredients));
    }

    public NewIngredientViewModel(MainWindowViewModel mainModel)
    {
        _mainModel = mainModel;
        Tags = mainModel.Tags;
    }
}