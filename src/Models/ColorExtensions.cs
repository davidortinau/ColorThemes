using System;

namespace ColorThemes.Models;

/// <summary>
/// Extension methods for Color objects.
/// </summary>
public static class ColorExtensions
{
    /// <summary>
    /// Converts a Color to its hex representation.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A string representing the hex color code.</returns>
    public static string ToHex(this Color color)
    {
        return $"#{(byte)(color.Red * 255):X2}{(byte)(color.Green * 255):X2}{(byte)(color.Blue * 255):X2}";
    }
    
    /// <summary>
    /// Converts a Color to its hex representation including alpha.
    /// </summary>
    /// <param name="color">The color to convert.</param>
    /// <returns>A string representing the hex color code with alpha.</returns>
    public static string ToHexWithAlpha(this Color color)
    {
        return $"#{(byte)(color.Alpha * 255):X2}{(byte)(color.Red * 255):X2}{(byte)(color.Green * 255):X2}{(byte)(color.Blue * 255):X2}";
    }
}
