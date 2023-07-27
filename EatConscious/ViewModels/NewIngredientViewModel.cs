using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Avalonia.Controls;
using EatConscious.Models;
using ReactiveUI;

namespace EatConscious.ViewModels;

public class NewIngredientViewModel : ViewModelBase
{
    public string Name { get; set; } = "Onion";
    // get the unit
    private readonly ComboBox _unitComboBox;

    private readonly MainWindowViewModel _mainModel;
    
    [Required]
    public double Kcal { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fats { get; set; }
    public double NutrientBase { get; set; }
    
    public double Price { get; set; }
    public double PriceBase { get; set; }

    private List<string> Options { get; } = Measure.OneOfEach.Select(m => m.MeasureId).ToList();

    public string Unit => Options[_unitComboBox.SelectedIndex];

    public Ingredient CreateIngredient()
    {
        Nutrients nutrients = new()
        {
            Kcal = this.Kcal,
            Protein = this.Protein,
            Carbs = this.Carbs,
            Fats = this.Fats
        };
        Measure NutrientBaseMeasure = Measure.OneOfEach[_unitComboBox.SelectedIndex];
        var PriceBaseMeasure = NutrientBaseMeasure;
        NutrientBaseMeasure.Value = NutrientBase;
        PriceBaseMeasure.Value = PriceBase;
        return new Ingredient(Name, nutrients, NutrientBaseMeasure, Price, PriceBaseMeasure);
    }

    public void AddClick()
    {
        _mainModel.Ingredients.Add(CreateIngredient());
        _mainModel.RaisePropertyChanged(nameof(_mainModel.Ingredients));
    }

    public NewIngredientViewModel(MainWindowViewModel mainModel, ComboBox unitComboBox)
    {
        _mainModel = mainModel;
        _unitComboBox = unitComboBox;
        unitComboBox.ItemsSource = Options;
        this.WhenAnyValue(m => m._unitComboBox.SelectedIndex)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Unit)));
    }
}