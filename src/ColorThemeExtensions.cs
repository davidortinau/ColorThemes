#nullable disable
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System.Text;
using ColorThemes.Models;

namespace Theming
{
    /// <summary>
    /// Extension methods for the ColorTheme class
    /// </summary>
    public static class ColorThemeExtensions
    {
        /// <summary>
        /// Generates a XAML representation of the ColorTheme that can be displayed in an Editor or saved to a file.
        /// </summary>
        /// <param name="theme">The ColorTheme to convert to XAML</param>
        /// <param name="resourceDictionaryName">Optional name for the ResourceDictionary</param>
        /// <returns>A string containing the XAML representation of the theme</returns>
        public static string ToExtXaml(this ColorTheme theme, string resourceDictionaryName = "ThemeColors")
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.AppendLine("<ResourceDictionary ");
            sb.AppendLine("    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
            sb.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"");
            sb.AppendLine($"    x:Name=\"{resourceDictionaryName}\">");
            sb.AppendLine();

            // Primary colors
            AppendColorResource(sb, nameof(theme.Primary), theme.Primary);
            AppendColorResource(sb, nameof(theme.PrimaryHover), theme.PrimaryHover);
            AppendColorResource(sb, nameof(theme.PrimaryPressed), theme.PrimaryPressed);
            AppendColorResource(sb, nameof(theme.PrimaryDisabled), theme.PrimaryDisabled);
            AppendColorResource(sb, nameof(theme.OnPrimary), theme.OnPrimary);
            sb.AppendLine();

            // Secondary colors
            AppendColorResource(sb, nameof(theme.Secondary), theme.Secondary);
            AppendColorResource(sb, nameof(theme.SecondaryHover), theme.SecondaryHover);
            AppendColorResource(sb, nameof(theme.SecondaryPressed), theme.SecondaryPressed);
            AppendColorResource(sb, nameof(theme.SecondaryDisabled), theme.SecondaryDisabled);
            AppendColorResource(sb, nameof(theme.OnSecondary), theme.OnSecondary);
            sb.AppendLine();

            // Background and Surfaces
            AppendColorResource(sb, nameof(theme.Background), theme.Background);
            AppendColorResource(sb, nameof(theme.Surface0), theme.Surface0);
            AppendColorResource(sb, nameof(theme.Surface1), theme.Surface1);
            AppendColorResource(sb, nameof(theme.Surface2), theme.Surface2);
            AppendColorResource(sb, nameof(theme.Surface3), theme.Surface3);
            AppendColorResource(sb, nameof(theme.OnBackground), theme.OnBackground);
            AppendColorResource(sb, nameof(theme.OnSurface), theme.OnSurface);
            sb.AppendLine();

            // Feedback colors
            AppendColorResource(sb, nameof(theme.Error), theme.Error);
            AppendColorResource(sb, nameof(theme.Success), theme.Success);
            AppendColorResource(sb, nameof(theme.Info), theme.Info);
            sb.AppendLine();

            // Borders and Focus
            AppendColorResource(sb, nameof(theme.Border), theme.Border);
            AppendColorResource(sb, nameof(theme.FocusOutline), theme.FocusOutline);
            
            sb.AppendLine("</ResourceDictionary>");
            
            return sb.ToString();
        }

        private static void AppendColorResource(StringBuilder sb, string name, Color color)
        {
            if (color != Colors.Transparent)
            {
                string colorHex = ToHex(color);
                sb.AppendLine($"    <Color x:Key=\"{name}\">{colorHex}</Color>");
            }
        }

        /// <summary>
        /// Converts a Color to its hex representation (#RRGGBB or #AARRGGBB)
        /// </summary>
        private static string ToHex(Color color)
        {
            if (color.Alpha < 1.0)
            {
                return $"#{(int)(color.Alpha * 255):X2}{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
            }
            return $"#{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
        }
    }
}
