#nullable disable
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System.Text;

namespace Theming
{
    /// <summary>
    /// Represents a comprehensive set of theme colors for your app.
    /// </summary>
    public class ColorTheme
    {
        // Default constructor that initializes all properties
        public ColorTheme()
        {
            Background = Colors.White;
            Surface0 = Colors.White;
            Surface1 = Colors.White;
            Surface2 = Colors.White;
            Surface3 = Colors.White;

            Primary = Colors.Blue;
            PrimaryHover = Colors.Blue;
            PrimaryPressed = Colors.Blue;
            PrimaryDisabled = Colors.Gray;
            OnPrimary = Colors.White;

            Secondary = Colors.Gray;
            SecondaryHover = Colors.DarkGray;
            SecondaryPressed = Colors.DimGray;
            SecondaryDisabled = Colors.LightGray;
            OnSecondary = Colors.Black;

            Error = Colors.Red;
            Success = Colors.Green;
            Info = Colors.Blue;
            OnBackground = Colors.Black;
            OnSurface = Colors.Black;

            Border = Colors.Gray;
            FocusOutline = Colors.Blue;
        }

        public Color Background { get; set; }
        public Color Surface0 { get; set; }
        public Color Surface1 { get; set; }
        public Color Surface2 { get; set; }
        public Color Surface3 { get; set; }

        public Color Primary { get; set; }
        public Color PrimaryHover { get; set; }
        public Color PrimaryPressed { get; set; }
        public Color PrimaryDisabled { get; set; }
        public Color OnPrimary { get; set; }

        public Color Secondary { get; set; }
        public Color SecondaryHover { get; set; }
        public Color SecondaryPressed { get; set; }
        public Color SecondaryDisabled { get; set; }
        public Color OnSecondary { get; set; }

        public Color Error { get; set; }
        public Color Success { get; set; }
        public Color Info { get; set; }
        public Color OnBackground { get; set; }
        public Color OnSurface { get; set; }

        public Color Border { get; set; }
        public Color FocusOutline { get; set; }
    }

    /// <summary>
    /// Generates <see cref="ColorTheme"/> instances with sensible defaults and semantic roles.
    /// </summary>
    public static class ColorThemeGenerator
    {
        /// <summary>
        /// Creates a ColorTheme based on primary, secondary, and background colors.
        /// </summary>
        /// <param name="primary">Your app's primary accent color.</param>
        /// <param name="secondary">Your app's secondary accent color.</param>
        /// <param name="background">Base background (e.g. white or black).</param>
        /// <param name="isDarkTheme">True for dark mode, false for light.</param>
        public static ColorTheme Generate(
            Color primary,
            Color secondary,
            Color background,
            bool isDarkTheme)
        {
            // Default feedback colors
            var error = Color.FromArgb("#B00020");
            var success = Color.FromArgb("#2E7D32");
            var info = Color.FromArgb("#0288D1");

            // Helper lambdas for light/dark variants
            static Color Lighten(Color c, float amount) => c.WithLuminosity(MathF.Min(c.GetLuminosity() + amount, 1f));
            static Color Darken(Color c, float amount) => c.WithLuminosity(MathF.Max(c.GetLuminosity() - amount, 0f));

            var theme = new ColorTheme
            {
                // Primary variants
                Primary = primary,
                PrimaryHover = isDarkTheme ? Lighten(primary, 0.15f) : Darken(primary, 0.10f),
                PrimaryPressed = isDarkTheme ? Lighten(primary, 0.30f) : Darken(primary, 0.25f),
                PrimaryDisabled = primary.WithSaturation(0.2f).WithAlpha(0.5f),
                OnPrimary = primary.IsDark() ? Colors.White : Colors.Black,

                // Secondary variants
                Secondary = secondary,
                SecondaryHover = isDarkTheme ? Lighten(secondary, 0.15f) : Darken(secondary, 0.10f),
                SecondaryPressed = isDarkTheme ? Lighten(secondary, 0.30f) : Darken(secondary, 0.25f),
                SecondaryDisabled = secondary.WithSaturation(0.2f).WithAlpha(0.5f),
                OnSecondary = secondary.IsDark() ? Colors.White : Colors.Black,

                // Background & surfaces
                Background = background,
                Surface0 = isDarkTheme ? Lighten(background, 0.05f) : Darken(background, 0.05f),
                Surface1 = isDarkTheme ? Lighten(background, 0.10f) : Darken(background, 0.10f),
                Surface2 = isDarkTheme ? Lighten(background, 0.15f) : Darken(background, 0.15f),
                Surface3 = isDarkTheme ? Lighten(background, 0.20f) : Darken(background, 0.20f),
                
                // On colors
                OnBackground = background.IsDark() ? Colors.White : Colors.Black,
                
                // Feedback & accents
                Error = error,
                Success = success,
                Info = info,

                // Borders & focus
                Border = isDarkTheme ? Lighten(background, 0.20f) : Darken(background, 0.20f),
                FocusOutline = info.WithAlpha(0.8f)
            };
            
            // OnSurface needs to be set after Surface1 is initialized
            theme.OnSurface = theme.Surface1.IsDark() ? Colors.White : Colors.Black;

            return theme;
        }

        /// <summary>
        /// Applies a generated theme into a ResourceDictionary for runtime theming in .NET MAUI.
        /// </summary>
        public static void ApplyTheme(this ResourceDictionary resources, ColorTheme theme)
        {
            resources[nameof(theme.Primary)] = theme.Primary;
            resources[nameof(theme.PrimaryHover)] = theme.PrimaryHover;
            resources[nameof(theme.PrimaryPressed)] = theme.PrimaryPressed;
            resources[nameof(theme.PrimaryDisabled)] = theme.PrimaryDisabled;
            resources[nameof(theme.OnPrimary)] = theme.OnPrimary;

            resources[nameof(theme.Secondary)] = theme.Secondary;
            resources[nameof(theme.SecondaryHover)] = theme.SecondaryHover;
            resources[nameof(theme.SecondaryPressed)] = theme.SecondaryPressed;
            resources[nameof(theme.SecondaryDisabled)] = theme.SecondaryDisabled;
            resources[nameof(theme.OnSecondary)] = theme.OnSecondary;

            resources[nameof(theme.Background)] = theme.Background;
            resources[nameof(theme.Surface0)] = theme.Surface0;
            resources[nameof(theme.Surface1)] = theme.Surface1;
            resources[nameof(theme.Surface2)] = theme.Surface2;
            resources[nameof(theme.Surface3)] = theme.Surface3;
            resources[nameof(theme.OnBackground)] = theme.OnBackground;
            resources[nameof(theme.OnSurface)] = theme.OnSurface;

            resources[nameof(theme.Error)] = theme.Error;
            resources[nameof(theme.Success)] = theme.Success;
            resources[nameof(theme.Info)] = theme.Info;

            resources[nameof(theme.Border)] = theme.Border;
            resources[nameof(theme.FocusOutline)] = theme.FocusOutline;
        }

        /// <summary>
        /// Generates a XAML representation of the ColorTheme that can be displayed in an Editor or saved to a file.
        /// </summary>
        /// <param name="theme">The ColorTheme to convert to XAML</param>
        /// <param name="resourceDictionaryName">Optional name for the ResourceDictionary</param>
        /// <returns>A string containing the XAML representation of the theme</returns>
        // public static string ToXaml(this ColorTheme theme, string resourceDictionaryName = "ThemeColors")
        // {
        //     var sb = new StringBuilder();
            
        //     sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        //     sb.AppendLine("<ResourceDictionary ");
        //     sb.AppendLine("    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
        //     sb.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"");
        //     sb.AppendLine($"    x:Name=\"{resourceDictionaryName}\">");
        //     sb.AppendLine();

        //     // Primary colors
        //     AppendColorResource(sb, nameof(theme.Primary), theme.Primary);
        //     AppendColorResource(sb, nameof(theme.PrimaryHover), theme.PrimaryHover);
        //     AppendColorResource(sb, nameof(theme.PrimaryPressed), theme.PrimaryPressed);
        //     AppendColorResource(sb, nameof(theme.PrimaryDisabled), theme.PrimaryDisabled);
        //     AppendColorResource(sb, nameof(theme.OnPrimary), theme.OnPrimary);
        //     sb.AppendLine();

        //     // Secondary colors
        //     AppendColorResource(sb, nameof(theme.Secondary), theme.Secondary);
        //     AppendColorResource(sb, nameof(theme.SecondaryHover), theme.SecondaryHover);
        //     AppendColorResource(sb, nameof(theme.SecondaryPressed), theme.SecondaryPressed);
        //     AppendColorResource(sb, nameof(theme.SecondaryDisabled), theme.SecondaryDisabled);
        //     AppendColorResource(sb, nameof(theme.OnSecondary), theme.OnSecondary);
        //     sb.AppendLine();

        //     // Background and Surfaces
        //     AppendColorResource(sb, nameof(theme.Background), theme.Background);
        //     AppendColorResource(sb, nameof(theme.Surface0), theme.Surface0);
        //     AppendColorResource(sb, nameof(theme.Surface1), theme.Surface1);
        //     AppendColorResource(sb, nameof(theme.Surface2), theme.Surface2);
        //     AppendColorResource(sb, nameof(theme.Surface3), theme.Surface3);
        //     AppendColorResource(sb, nameof(theme.OnBackground), theme.OnBackground);
        //     AppendColorResource(sb, nameof(theme.OnSurface), theme.OnSurface);
        //     sb.AppendLine();

        //     // Feedback colors
        //     AppendColorResource(sb, nameof(theme.Error), theme.Error);
        //     AppendColorResource(sb, nameof(theme.Success), theme.Success);
        //     AppendColorResource(sb, nameof(theme.Info), theme.Info);
        //     sb.AppendLine();

        //     // Borders and Focus
        //     AppendColorResource(sb, nameof(theme.Border), theme.Border);
        //     AppendColorResource(sb, nameof(theme.FocusOutline), theme.FocusOutline);
            
        //     sb.AppendLine("</ResourceDictionary>");
            
        //     return sb.ToString();
        // }

        private static void AppendColorResource(StringBuilder sb, string name, Color color)
        {
            if (color != Colors.Transparent)
            {
                string colorHex = ToHex(color);
                sb.AppendLine($"    <Color x:Key=\"{name}\">{colorHex}</Color>");
            }
        }

        /// <summary>
        /// Extension method to determine if a color is dark based on luminance.
        /// </summary>
        private static bool IsDark(this Color color)
        {
            // Approximate relative luminance
            var lum = 0.299f * color.Red + 0.587f * color.Green + 0.114f * color.Blue;
            return lum < 0.5f;
        }

        /// <summary>
        /// Converts a Color to its hex representation (#RRGGBB or #AARRGGBB)
        /// </summary>
        private static string ToHex(this Color color)
        {
            if (color.Alpha < 1.0)
            {
                return $"#{(int)(color.Alpha * 255):X2}{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
            }
            return $"#{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
        }
        

    }
}
