#nullable disable
using System.Text;
using ColorThemes.Models;

namespace ColorThemes
{
    

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

            // Base feedback colors (we maintain their hues but adjust other properties)
            Color baseErrorLight = Color.FromArgb("#B00020");  // Deep red
            Color baseSuccessLight = Color.FromArgb("#2E7D32"); // Deep green
            Color baseInfoLight = Color.FromArgb("#0288D1");   // Deep blue
            
            // Generate feedback colors that blend with the theme colors
            var lightError = BlendFeedbackColor(baseErrorLight, lightPrimary, lightBackground, false);
            var darkError = BlendFeedbackColor(baseErrorLight, darkPrimary, darkBackground, true);
            
            var lightSuccess = BlendFeedbackColor(baseSuccessLight, lightPrimary, lightBackground, false);
            var darkSuccess = BlendFeedbackColor(baseSuccessLight, darkPrimary, darkBackground, true);
            
            var lightInfo = BlendFeedbackColor(baseInfoLight, lightPrimary, lightBackground, false);
            var darkInfo = BlendFeedbackColor(baseInfoLight, darkPrimary, darkBackground, true);

            // Helper lambdas for light/dark variants
            static Color Lighten(Color c, float amount) => c.WithLuminosity(MathF.Min(c.GetLuminosity() + amount, 1f));
            static Color Darken(Color c, float amount) => c.WithLuminosity(MathF.Max(c.GetLuminosity() - amount, 0f));

            var theme = new ColorTheme
            {
                // Light theme colors
                // Primary variants
                Primary = lightPrimary,
                OnPrimary = lightPrimary.IsDark() ? Colors.White : Colors.Black,

                // Secondary variants
                Secondary = lightSecondary,
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

                // Dark theme colors
                // Primary variants
                PrimaryDark = darkPrimary,
                OnPrimaryDark = darkPrimary.IsDark() ? Colors.White : Colors.Black,

                // Secondary variants
                SecondaryDark = darkSecondary,
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

            };
            
            // OnSurface needs to be set after Surface1 is initialized
            theme.OnSurface = theme.Surface1.IsDark() ? Colors.White : Colors.Black;
            theme.OnSurfaceDark = theme.Surface1Dark.IsDark() ? Colors.White : Colors.Black;

            theme.UpdateHexFromColors();

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
        /// Creates a ColorTheme with both light and dark mode colors.
        /// This overload uses the exact light and dark background colors provided, while
        /// automatically generating dark variants of the primary and secondary colors.
        /// </summary>
        /// <param name="primary">Your app's primary accent color (light variant).</param>
        /// <param name="secondary">Your app's secondary accent color (light variant).</param>
        /// <param name="lightBackground">Base background for light mode.</param>
        /// <param name="darkBackground">Base background for dark mode.</param>
        /// <param name="isDarkTheme">True for dark mode, false for light.</param>
        /// <returns>A ColorTheme with appropriate color variants for both light and dark themes.</returns>
        public static ColorTheme Generate(
            Color primary,
            Color secondary,
            Color lightBackground,
            Color darkBackground,
            bool isDarkTheme)
        {
            // Generate dark variants of primary and secondary colors
            Color darkPrimary = AdjustColorForDarkTheme(primary);
            Color darkSecondary = AdjustColorForDarkTheme(secondary);
            
            // Call the full Generate method with our calculated dark variants
            return Generate(primary, darkPrimary, secondary, darkSecondary, lightBackground, darkBackground);
        }
        
        /// <summary>
        /// Helper method to adjust a color for dark theme by increasing its vibrancy and brightness.
        /// </summary>
        /// <param name="color">The light theme color to adjust</param>
        /// <returns>A color adjusted for dark theme</returns>
        public static Color AdjustColorForDarkTheme(Color color)
        {
            // Convert to HSL to manipulate the saturation and luminosity
            color.ToHsl(out float hue, out float saturation, out float luminosity);
            
            // Increase saturation for more vibrant colors in dark mode
            float darkSaturation = MathF.Min(saturation * 1.2f, 1.0f);
            
            // Increase luminosity for better visibility in dark mode
            float darkLuminosity = MathF.Min(luminosity + 0.15f, 0.8f);
            
            return Color.FromHsla(hue, darkSaturation, darkLuminosity);
        }

        /// <summary>
        /// Applies a generated theme into a ResourceDictionary for runtime theming in .NET MAUI.
        /// This method uses the CommunityToolkit.Maui's AppThemeColor to handle both light and dark themes.
        /// </summary>
        public static void ApplyTheme(this ResourceDictionary resources, ColorTheme theme, bool isDarkMode)
        {
                resources[nameof(theme.Primary)] = isDarkMode ? theme.PrimaryDark : theme.Primary;
                resources[nameof(theme.OnPrimary)] = isDarkMode ? theme.OnPrimaryDark : theme.OnPrimary;

                resources[nameof(theme.Secondary)] = isDarkMode ? theme.SecondaryDark : theme.Secondary;
                resources[nameof(theme.OnSecondary)] = isDarkMode ? theme.OnSecondaryDark : theme.OnSecondary;

                resources[nameof(theme.Background)] = isDarkMode ? theme.BackgroundDark : theme.Background;
                resources[nameof(theme.Surface0)] = isDarkMode ? theme.Surface0Dark : theme.Surface0;
                resources[nameof(theme.Surface1)] = isDarkMode ? theme.Surface1Dark : theme.Surface1;
                resources[nameof(theme.Surface2)] = isDarkMode ? theme.Surface2Dark : theme.Surface2;
                resources[nameof(theme.Surface3)] = isDarkMode ? theme.Surface3Dark : theme.Surface3;
                resources[nameof(theme.OnBackground)] = isDarkMode ? theme.OnBackgroundDark : theme.OnBackground;
                resources[nameof(theme.OnSurface)] = isDarkMode ? theme.OnSurfaceDark : theme.OnSurface;

                resources[nameof(theme.Error)] = isDarkMode ? theme.ErrorDark : theme.Error;
                resources[nameof(theme.Success)] = isDarkMode ? theme.SuccessDark : theme.Success;
                resources[nameof(theme.Info)] = isDarkMode ? theme.InfoDark : theme.Info;
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
        
        /// <summary>
        /// Blends a feedback color with primary color while maintaining contrast with background.
        /// </summary>
        /// <param name="baseFeedbackColor">Base semantic color (error, success, info)</param>
        /// <param name="primaryColor">The theme's primary color to blend with</param>
        /// <param name="backgroundColor">The background color to ensure contrast against</param>
        /// <param name="isDarkTheme">Whether we're in dark theme mode</param>
        /// <returns>A blended feedback color that maintains its semantic meaning</returns>
        private static Color BlendFeedbackColor(Color baseFeedbackColor, Color primaryColor, Color backgroundColor, bool isDarkTheme)
        {
            // Extract hue from the base feedback color (we want to maintain this for semantic clarity)
            baseFeedbackColor.ToHsl(out float baseHue, out float _, out float _);
            
            // Get saturation influence from primary (adds theme cohesion)
            primaryColor.ToHsl(out float _, out float primarySaturation, out float primaryLuminosity);
            
            // Calculate saturation - weighted blend favoring the base feedback color
            float saturation = (baseFeedbackColor.GetSaturation() * 0.7f) + (primarySaturation * 0.3f);
            
            // For dark themes, we generally want more vibrant colors
            if (isDarkTheme)
            {
                saturation = MathF.Min(saturation * 1.2f, 1.0f);
            }
            
            // Calculate luminosity based on background to ensure contrast
            float targetLuminosity;
            
            if (isDarkTheme)
            {
                // In dark mode, make feedback colors brighter
                targetLuminosity = 0.6f + (primaryLuminosity * 0.2f);
            }
            else
            {
                // In light mode, make colors deeper
                targetLuminosity = MathF.Min(0.4f + (primaryLuminosity * 0.1f), 0.5f);
            }
            
            // Create our final color maintaining the original hue
            return Color.FromHsla(baseHue, saturation, targetLuminosity);
        }

    }
}
