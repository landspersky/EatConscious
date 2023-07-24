using System.Collections.Generic;
using EatConscious.Models;
using ReactiveUI;

namespace EatConscious.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public List<Measure> Measures { get; set; } = new()
    {
        100.ToMeasure<Gram>(),
        40.ToMeasure<Mililiter>(),
        3.ToMeasure<Piece>()
    };

    public void Click()
    {
        Measures.Add(300.ToMeasure<Mililiter>());
        //this.RaisePropertyChanged(nameof(Measures));
    }
    
}