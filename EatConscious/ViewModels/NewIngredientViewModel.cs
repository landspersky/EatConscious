using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Avalonia.Controls;
using ReactiveUI;

namespace EatConscious.ViewModels;

public class NewIngredientViewModel : ViewModelBase
{
    public string? Name { get; set; } = "Onion";
    // get the unit
    private readonly ComboBox _unitComboBox;
    
    [Required]
    public int? Kcal { get; set; }
    public int? Protein { get; set; }
    public int? Carbs { get; set; }
    public int? Fats { get; set; }
    public int? NutrientBase { get; set; }
    
    public int? Price { get; set; }
    public int? PriceBase { get; set; }

    private List<string> Options { get; } = new() { "g", "ml", "pcs" };

    public string Unit => Options[_unitComboBox.SelectedIndex];

    public NewIngredientViewModel(ComboBox unitComboBox)
    {
        _unitComboBox = unitComboBox;
        unitComboBox.ItemsSource = Options;
        this.WhenAnyValue(m => m._unitComboBox.SelectedIndex)
            .Subscribe(_ => this.RaisePropertyChanged(nameof(Unit)));
    }
}