using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    
    public List<Measure> Units => Measure.All().ToList();

    private Measure _selectedUnit = Measure.Gram;
    public Measure SelectedUnit
    {
        get => _selectedUnit;
        set => this.RaiseAndSetIfChanged(ref _selectedUnit, value);
    }
    
    public ObservableCollection<string> Tags { get; }

    public ObservableCollection<string> SelectedTags { get; set; } = new();

    /// <summary>
    /// Ingredient being edited; otherwise null
    /// </summary>
    private readonly Ingredient? _editing;

    /// <summary>
    /// What is displayed on the button on the bottom
    /// </summary>
    public string ButtonText => _editing is null ? "Add" : "Edit";
    
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
        
        int id;
        if (_editing is null)
        {
            State.IncrementIngredientId();
            id = State.TopIngredientId;
        }
        else
        {
            id = _editing.Id;
        }

        return Ingredient.Create(
            id, Name, nutrients, NutrientBase, Price, PriceBase, SelectedUnit, SelectedTags.ToList());
    }

    public void ButtonClick() => _mainModel.AddOrUpdate(CreateIngredient());

    /// <summary>
    /// View model for creating a new ingredient
    /// </summary>
    /// <param name="mainModel">Parent view model</param>
    public NewIngredientViewModel(MainWindowViewModel mainModel)
    {
        _mainModel = mainModel;
        Tags = mainModel.IngredientTags;
    }

    /// <summary>
    /// View model for editing an ingredient
    /// </summary>
    /// <param name="mainModel">Parent view model</param>
    /// <param name="openedFrom">Ingredient to edit</param>
    public NewIngredientViewModel(MainWindowViewModel mainModel, Ingredient openedFrom) 
        : this(mainModel)
    {
        Name = openedFrom.Name;
        SelectedUnit = openedFrom.Unit;
        _editing = openedFrom;
    }
}