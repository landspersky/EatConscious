using System;
using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace EatConscious.Controls;

/// <summary>
/// Custom text control for numeric input
/// </summary>
/// <remarks>
/// The text value is converted to double on lost focus
/// </remarks>
public class NumericInput : TextBox
{
    private static readonly double CommonMaximum = 9_999;
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<NumericInput, double>(nameof(Value));

    protected override Type StyleKeyOverride { get; } = typeof(TextBox);

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        ValidateInput();
        Text = Value.ToString(CultureInfo.InvariantCulture);
    }

    private void ValidateInput()
    {
        Value = double.TryParse(Text, out double result) && result >= Min && result <= Max ? result : Value;
    }

    private double Min { get; }
    private double Max { get; }

    protected NumericInput(double min)
    {
        Value = min;
        Text = Value.ToString(CultureInfo.InvariantCulture);
        Min = min;
        Max = CommonMaximum;
    }
}

/// <summary>
/// Text control for inputting ingredients features (kcal, nutrients, price)
/// </summary>
public class FeatureInput : NumericInput
{
    public FeatureInput() : base(0)
    {
    }
}

/// <summary>
/// Text control for inputting ingredients base measures
/// </summary>
public class BaseInput : NumericInput
{
    // we divide it so it cannot be 0
    public BaseInput() : base(1)
    {
    }
}