#nullable disable
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System.Text;
using System.Reflection;

namespace Theming
{
    /// <summary>
    /// Represents a comprehensive set of theme colors for your app.
    /// 
    /// These colors can be used with AppThemeResource in XAML:
    /// <code>
    /// &lt;Button Background="{toolkit:AppThemeResource Primary}" /&gt;
    /// </code>
    /// 
    /// Make sure to include the toolkit namespace in your XAML:
    /// <code>
    /// xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
    /// </code>
    /// </summary>
    public class ColorTheme
    {
        // Default constructor that initializes all properties
        public ColorTheme()
        {
            Background = Colors.White;
            BackgroundDark = Colors.Black;
            
            Surface0 = Colors.White;
            Surface0Dark = Color.FromArgb("#121212");
            
            Surface1 = Colors.White;
            Surface1Dark = Color.FromArgb("#1E1E1E");
            
            Surface2 = Colors.White;
            Surface2Dark = Color.FromArgb("#222222");
            
            Surface3 = Colors.White;
            Surface3Dark = Color.FromArgb("#242424");

            Primary = Colors.Blue;
            PrimaryDark = Colors.LightBlue;
            
            PrimaryHover = Colors.Blue;
            PrimaryHoverDark = Colors.LightBlue;
            
            PrimaryPressed = Colors.Blue;
            PrimaryPressedDark = Colors.LightBlue;
            
            PrimaryDisabled = Colors.Gray;
            PrimaryDisabledDark = Colors.DimGray;
            
            OnPrimary = Colors.White;
            OnPrimaryDark = Colors.Black;

            Secondary = Colors.Gray;
            SecondaryDark = Colors.DarkGray;
            
            SecondaryHover = Colors.DarkGray;
            SecondaryHoverDark = Colors.Silver;
            
            SecondaryPressed = Colors.DimGray;
            SecondaryPressedDark = Colors.LightGray;
            
            SecondaryDisabled = Colors.LightGray;
            SecondaryDisabledDark = Colors.Gray;
            
            OnSecondary = Colors.Black;
            OnSecondaryDark = Colors.White;

            Error = Colors.Red;
            ErrorDark = Color.FromArgb("#FF5252");
            
            Success = Colors.Green;
            SuccessDark = Color.FromArgb("#69F0AE");
            
            Info = Colors.Blue;
            InfoDark = Color.FromArgb("#82B1FF");
            
            OnBackground = Colors.Black;
            OnBackgroundDark = Colors.White;
            
            OnSurface = Colors.Black;
            OnSurfaceDark = Colors.White;

            Border = Colors.Gray;
            BorderDark = Colors.DarkGray;
            
            FocusOutline = Colors.Blue;
            FocusOutlineDark = Colors.LightBlue;
        }

        // Light theme colors
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

        // Dark theme colors
        public Color BackgroundDark { get; set; }
        public Color Surface0Dark { get; set; }
        public Color Surface1Dark { get; set; }
        public Color Surface2Dark { get; set; }
        public Color Surface3Dark { get; set; }
        public Color PrimaryDark { get; set; }
        public Color PrimaryHoverDark { get; set; }
        public Color PrimaryPressedDark { get; set; }
        public Color PrimaryDisabledDark { get; set; }
        public Color OnPrimaryDark { get; set; }
        public Color SecondaryDark { get; set; }
        public Color SecondaryHoverDark { get; set; }
        public Color SecondaryPressedDark { get; set; }
        public Color SecondaryDisabledDark { get; set; }
        public Color OnSecondaryDark { get; set; }
        public Color ErrorDark { get; set; }
        public Color SuccessDark { get; set; }
        public Color InfoDark { get; set; }
        public Color OnBackgroundDark { get; set; }
        public Color OnSurfaceDark { get; set; }
        public Color BorderDark { get; set; }
        public Color FocusOutlineDark { get; set; }
    }

    /// <summary>
    /// Generates <see cref="ColorTheme"/> instances with sensible defaults and semantic roles.
    /// </summary>
    public static class ColorThemeGenerator
    {
        /// <summary>
        /// Creates a ColorTheme with both light and dark mode colors.
        /// </summary>
        /// <param name="lightPrimary">Your app's primary accent color for light mode.</param>
        /// <param name="darkPrimary">Your app's primary accent color for dark mode.</param>
        /// <param name="lightSecondary">Your app's secondary accent color for light mode.</param>
        /// <param name="darkSecondary">Your app's secondary accent color for dark mode.</param>
        /// <param name="lightBackground">Base background for light mode (e.g. white).</param>
        /// <param name="darkBackground">Base background for dark mode (e.g. black).</param>
        public static ColorTheme Generate(
            Color lightPrimary,
            Color darkPrimary,
            Color lightSecondary,
            Color darkSecondary,
            Color lightBackground = null,
            Color darkBackground = null)
        {
            lightBackground ??= Colors.White;
            darkBackground ??= Color.FromArgb("#121212"); // Material dark theme background

            // Default feedback colors
            var lightError = Color.FromArgb("#B00020");
            var darkError = Color.FromArgb("#CF6679");
            
            var lightSuccess = Color.FromArgb("#2E7D32");
            var darkSuccess = Color.FromArgb("#69F0AE");
            
            var lightInfo = Color.FromArgb("#0288D1");
            var darkInfo = Color.FromArgb("#82B1FF");

            // Helper lambdas for light/dark variants
            static Color Lighten(Color c, float amount) => c.WithLuminosity(MathF.Min(c.GetLuminosity() + amount, 1f));
            static Color Darken(Color c, float amount) => c.WithLuminosity(MathF.Max(c.GetLuminosity() - amount, 0f));

            var theme = new ColorTheme
            {
                // Light theme colors
                // Primary variants
                Primary = lightPrimary,
                PrimaryHover = Darken(lightPrimary, 0.10f),
                PrimaryPressed = Darken(lightPrimary, 0.25f),
                PrimaryDisabled = lightPrimary.WithSaturation(0.2f).WithAlpha(0.5f),
                OnPrimary = lightPrimary.IsDark() ? Colors.White : Colors.Black,

                // Secondary variants
                Secondary = lightSecondary,
                SecondaryHover = Darken(lightSecondary, 0.10f),
                SecondaryPressed = Darken(lightSecondary, 0.25f),
                SecondaryDisabled = lightSecondary.WithSaturation(0.2f).WithAlpha(0.5f),
                OnSecondary = lightSecondary.IsDark() ? Colors.White : Colors.Black,

                // Background & surfaces
                Background = lightBackground,
                Surface0 = Darken(lightBackground, 0.05f),
                Surface1 = Darken(lightBackground, 0.10f),
                Surface2 = Darken(lightBackground, 0.15f),
                Surface3 = Darken(lightBackground, 0.20f),
                
                // On colors
                OnBackground = lightBackground.IsDark() ? Colors.White : Colors.Black,
                
                // Feedback & accents
                Error = lightError,
                Success = lightSuccess,
                Info = lightInfo,

                // Borders & focus
                Border = Darken(lightBackground, 0.20f),
                FocusOutline = lightInfo.WithAlpha(0.8f),
                
                // Dark theme colors
                // Primary variants
                PrimaryDark = darkPrimary,
                PrimaryHoverDark = Lighten(darkPrimary, 0.15f),
                PrimaryPressedDark = Lighten(darkPrimary, 0.30f),
                PrimaryDisabledDark = darkPrimary.WithSaturation(0.2f).WithAlpha(0.5f),
                OnPrimaryDark = darkPrimary.IsDark() ? Colors.White : Colors.Black,

                // Secondary variants
                SecondaryDark = darkSecondary,
                SecondaryHoverDark = Lighten(darkSecondary, 0.15f),
                SecondaryPressedDark = Lighten(darkSecondary, 0.30f),
                SecondaryDisabledDark = darkSecondary.WithSaturation(0.2f).WithAlpha(0.5f),
                OnSecondaryDark = darkSecondary.IsDark() ? Colors.White : Colors.Black,

                // Background & surfaces
                BackgroundDark = darkBackground,
                Surface0Dark = Lighten(darkBackground, 0.05f),
                Surface1Dark = Lighten(darkBackground, 0.10f),
                Surface2Dark = Lighten(darkBackground, 0.15f),
                Surface3Dark = Lighten(darkBackground, 0.20f),
                
                // On colors
                OnBackgroundDark = darkBackground.IsDark() ? Colors.White : Colors.Black,
                
                // Feedback & accents
                ErrorDark = darkError,
                SuccessDark = darkSuccess,
                InfoDark = darkInfo,

                // Borders & focus
                BorderDark = Lighten(darkBackground, 0.20f),
                FocusOutlineDark = darkInfo.WithAlpha(0.8f)
            };
            
            // OnSurface needs to be set after Surface1 is initialized
            theme.OnSurface = theme.Surface1.IsDark() ? Colors.White : Colors.Black;
            theme.OnSurfaceDark = theme.Surface1Dark.IsDark() ? Colors.White : Colors.Black;

            return theme;
        }
        
        /// <summary>
        /// Creates a ColorTheme based on primary, secondary, and background colors.
        /// This overload is kept for backwards compatibility.
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
            if (isDarkTheme)
            {
                return Generate(primary, primary, secondary, secondary, background, background);
            }
            else
            {
                return Generate(primary, primary, secondary, secondary, background, background);
            }
        }

        /// <summary>
        /// Applies a generated theme into a ResourceDictionary for runtime theming in .NET MAUI.
        /// This method uses the CommunityToolkit.Maui's AppThemeColor to handle both light and dark themes.
        /// </summary>
        public static void ApplyTheme(this ResourceDictionary resources, ColorTheme theme)
        {
            // The AppThemeColor objects will be created dynamically at runtime
            // We'll use reflection to create these objects since they're part of the CommunityToolkit.Maui

            // AppThemeColor for Primary colors
            var primaryType = Type.GetType("CommunityToolkit.Maui.Markup.AppThemeColorExtension, CommunityToolkit.Maui");
            if (primaryType != null)
            {
                // Use dynamic AppThemeColor from the toolkit
                ApplyAppThemeColor(resources, nameof(theme.Primary), theme.Primary, theme.PrimaryDark);
                ApplyAppThemeColor(resources, nameof(theme.PrimaryHover), theme.PrimaryHover, theme.PrimaryHoverDark);
                ApplyAppThemeColor(resources, nameof(theme.PrimaryPressed), theme.PrimaryPressed, theme.PrimaryPressedDark);
                ApplyAppThemeColor(resources, nameof(theme.PrimaryDisabled), theme.PrimaryDisabled, theme.PrimaryDisabledDark);
                ApplyAppThemeColor(resources, nameof(theme.OnPrimary), theme.OnPrimary, theme.OnPrimaryDark);

                ApplyAppThemeColor(resources, nameof(theme.Secondary), theme.Secondary, theme.SecondaryDark);
                ApplyAppThemeColor(resources, nameof(theme.SecondaryHover), theme.SecondaryHover, theme.SecondaryHoverDark);
                ApplyAppThemeColor(resources, nameof(theme.SecondaryPressed), theme.SecondaryPressed, theme.SecondaryPressedDark);
                ApplyAppThemeColor(resources, nameof(theme.SecondaryDisabled), theme.SecondaryDisabled, theme.SecondaryDisabledDark);
                ApplyAppThemeColor(resources, nameof(theme.OnSecondary), theme.OnSecondary, theme.OnSecondaryDark);

                ApplyAppThemeColor(resources, nameof(theme.Background), theme.Background, theme.BackgroundDark);
                ApplyAppThemeColor(resources, nameof(theme.Surface0), theme.Surface0, theme.Surface0Dark);
                ApplyAppThemeColor(resources, nameof(theme.Surface1), theme.Surface1, theme.Surface1Dark);
                ApplyAppThemeColor(resources, nameof(theme.Surface2), theme.Surface2, theme.Surface2Dark);
                ApplyAppThemeColor(resources, nameof(theme.Surface3), theme.Surface3, theme.Surface3Dark);
                ApplyAppThemeColor(resources, nameof(theme.OnBackground), theme.OnBackground, theme.OnBackgroundDark);
                ApplyAppThemeColor(resources, nameof(theme.OnSurface), theme.OnSurface, theme.OnSurfaceDark);

                ApplyAppThemeColor(resources, nameof(theme.Error), theme.Error, theme.ErrorDark);
                ApplyAppThemeColor(resources, nameof(theme.Success), theme.Success, theme.SuccessDark);
                ApplyAppThemeColor(resources, nameof(theme.Info), theme.Info, theme.InfoDark);

                ApplyAppThemeColor(resources, nameof(theme.Border), theme.Border, theme.BorderDark);
                ApplyAppThemeColor(resources, nameof(theme.FocusOutline), theme.FocusOutline, theme.FocusOutlineDark);
            }
            else
            {
                // Fallback to regular Colors if the AppThemeColor is not available
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
        }
        
        /// <summary>
        /// Helper method to apply an AppThemeColor to the ResourceDictionary.
        /// </summary>
        private static void ApplyAppThemeColor(ResourceDictionary resources, string key, Color lightColor, Color darkColor)
        {
            // Create AppThemeColor directly instead of using reflection
            var appThemeColorType = Type.GetType("CommunityToolkit.Maui.AppThemeColor, CommunityToolkit.Maui");
            if (appThemeColorType != null)
            {
                var appThemeColor = Activator.CreateInstance(appThemeColorType);
                
                // Use reflection to set the Light and Dark properties
                var lightProperty = appThemeColorType.GetProperty("Light");
                var darkProperty = appThemeColorType.GetProperty("Dark");
                
                if (lightProperty != null && darkProperty != null)
                {
                    lightProperty.SetValue(appThemeColor, lightColor);
                    darkProperty.SetValue(appThemeColor, darkColor);
                    resources[key] = appThemeColor;
                }
                else
                {
                    // Fallback if properties not found
                    resources[key] = lightColor;
                }
            }
            else
            {
                // Fallback if type not found
                resources[key] = lightColor;
            }
        }

        /// <summary>
        /// Generates a XAML representation of the ColorTheme that can be displayed in an Editor or saved to a file.
        /// Uses the .NET Community Toolkit's AppThemeColor to express both Light and Dark colors together.
        /// </summary>
        /// <param name="theme">The ColorTheme to convert to XAML</param>
        /// <param name="resourceDictionaryName">Optional name for the ResourceDictionary</param>
        /// <returns>A string containing the XAML representation of the theme</returns>
        public static string ToXaml(this ColorTheme theme, string resourceDictionaryName = "ThemeColors")
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
            sb.AppendLine("<ResourceDictionary ");
            sb.AppendLine("    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
            sb.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\"");
            sb.AppendLine("    xmlns:toolkit=\"http://schemas.microsoft.com/dotnet/2022/maui/toolkit\"");
            sb.AppendLine($"    x:Name=\"{resourceDictionaryName}\">");
            sb.AppendLine();

            // Primary colors
            AppendAppThemeColorResource(sb, nameof(theme.Primary), theme.Primary, theme.PrimaryDark);
            AppendAppThemeColorResource(sb, nameof(theme.PrimaryHover), theme.PrimaryHover, theme.PrimaryHoverDark);
            AppendAppThemeColorResource(sb, nameof(theme.PrimaryPressed), theme.PrimaryPressed, theme.PrimaryPressedDark);
            AppendAppThemeColorResource(sb, nameof(theme.PrimaryDisabled), theme.PrimaryDisabled, theme.PrimaryDisabledDark);
            AppendAppThemeColorResource(sb, nameof(theme.OnPrimary), theme.OnPrimary, theme.OnPrimaryDark);
            sb.AppendLine();

            // Secondary colors
            AppendAppThemeColorResource(sb, nameof(theme.Secondary), theme.Secondary, theme.SecondaryDark);
            AppendAppThemeColorResource(sb, nameof(theme.SecondaryHover), theme.SecondaryHover, theme.SecondaryHoverDark);
            AppendAppThemeColorResource(sb, nameof(theme.SecondaryPressed), theme.SecondaryPressed, theme.SecondaryPressedDark);
            AppendAppThemeColorResource(sb, nameof(theme.SecondaryDisabled), theme.SecondaryDisabled, theme.SecondaryDisabledDark);
            AppendAppThemeColorResource(sb, nameof(theme.OnSecondary), theme.OnSecondary, theme.OnSecondaryDark);
            sb.AppendLine();

            // Background and Surfaces
            AppendAppThemeColorResource(sb, nameof(theme.Background), theme.Background, theme.BackgroundDark);
            AppendAppThemeColorResource(sb, nameof(theme.Surface0), theme.Surface0, theme.Surface0Dark);
            AppendAppThemeColorResource(sb, nameof(theme.Surface1), theme.Surface1, theme.Surface1Dark);
            AppendAppThemeColorResource(sb, nameof(theme.Surface2), theme.Surface2, theme.Surface2Dark);
            AppendAppThemeColorResource(sb, nameof(theme.Surface3), theme.Surface3, theme.Surface3Dark);
            AppendAppThemeColorResource(sb, nameof(theme.OnBackground), theme.OnBackground, theme.OnBackgroundDark);
            AppendAppThemeColorResource(sb, nameof(theme.OnSurface), theme.OnSurface, theme.OnSurfaceDark);
            sb.AppendLine();

            // Feedback colors
            AppendAppThemeColorResource(sb, nameof(theme.Error), theme.Error, theme.ErrorDark);
            AppendAppThemeColorResource(sb, nameof(theme.Success), theme.Success, theme.SuccessDark);
            AppendAppThemeColorResource(sb, nameof(theme.Info), theme.Info, theme.InfoDark);
            sb.AppendLine();

            // Borders and Focus
            AppendAppThemeColorResource(sb, nameof(theme.Border), theme.Border, theme.BorderDark);
            AppendAppThemeColorResource(sb, nameof(theme.FocusOutline), theme.FocusOutline, theme.FocusOutlineDark);
            
            sb.AppendLine("</ResourceDictionary>");
            
            return sb.ToString();
        }

        /// <summary>
        /// Helper method to append an AppThemeColor resource to the StringBuilder.
        /// </summary>
        private static void AppendAppThemeColorResource(StringBuilder sb, string name, Color lightColor, Color darkColor)
        {
            if (lightColor != Colors.Transparent || darkColor != Colors.Transparent)
            {
                string lightColorHex = ToHex(lightColor);
                string darkColorHex = ToHex(darkColor);
                sb.AppendLine($"    <toolkit:AppThemeColor x:Key=\"{name}\" Light=\"{lightColorHex}\" Dark=\"{darkColorHex}\" />");
            }
        }
        
        /// <summary>
        /// Helper method for backwards compatibility.
        /// </summary>
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
