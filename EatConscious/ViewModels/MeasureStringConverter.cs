using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using EatConscious.Models;

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
