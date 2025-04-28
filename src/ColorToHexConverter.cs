using System.Globalization;
using Microsoft.Maui.Graphics;

namespace dotnet_colorthemes;

/// <summary>
/// Converts a Color object to its hexadecimal string representation.
/// </summary>
public class ColorToHexConverter : IValueConverter
{
    /// <summary>
    /// Converts a Color object to its hexadecimal string representation.
    /// </summary>
    /// <param name="value">The Color to convert.</param>
    /// <param name="targetType">The type to convert to (ignored).</param>
    /// <param name="parameter">Optional parameter (ignored).</param>
    /// <param name="culture">The culture to use for the conversion (ignored).</param>
    /// <returns>A string representing the color in hexadecimal format (#RRGGBB).</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return $"#{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
        }

        return "#FFFFFF"; // Default white if conversion fails
    }

    /// <summary>
    /// Not implemented as this is a one-way converter.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("ColorToHexConverter is a one-way converter.");
    }
}
