using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Input;
using Avalonia.Interactivity;
using EatConscious.Models;
using ReactiveUI;

namespace EatConscious.ViewModels;

public class NewIngredientViewModel : ViewModelBase
{
    public string Name { get; set; } = "Onion";
    // get the unit
    private readonly ComboBox _unitComboBox;

    private readonly MainWindowViewModel _mainModel;

    public double Kcal { get; set; }

    public double Protein { get; set; } 
    public double Carbs { get; set; }
    public double Fats { get; set; }
    public double NutrientBase { get; set; } = 1;
    public double Price { get; set; }
    public double PriceBase { get; set; } = 1;

    private List<string> Options { get; } = Measure.OneOfEach.Select(m => m.MeasureId).ToList();

    public string Unit => Options[_unitComboBox.SelectedIndex];

    public List<string> Tags { get; }

    public IReadOnlyList<string> SelectedTags { get; set; } = new List<string>();

    public Ingredient CreateIngredient()
    {
        Nutrients nutrients = new()
        {
            Kcal = this.Kcal,
            Protein = this.Protein,
            Carbs = this.Carbs,
            Fats = this.Fats
        };
        Measure nutrientBaseMeasure = Measure.OneOfEach[_unitComboBox.SelectedIndex];
        var priceBaseMeasure = nutrientBaseMeasure;
        nutrientBaseMeasure.Value = NutrientBase;
        priceBaseMeasure.Value = PriceBase;
        return Ingredient.Convert(Name, nutrients, nutrientBaseMeasure, Price, priceBaseMeasure, 
            SelectedTags.ToList());
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
        unitComboBox.SelectedIndex = 0;
        this.WhenAnyValue(m => m._unitComboBox.SelectedIndex)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Unit)));
        Tags = mainModel.Tags;
    }
}