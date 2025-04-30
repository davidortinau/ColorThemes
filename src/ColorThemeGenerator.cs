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
                ApplyAppThemeColor(resources, nameof(theme.OnPrimary), theme.OnPrimary, theme.OnPrimaryDark);

                ApplyAppThemeColor(resources, nameof(theme.Secondary), theme.Secondary, theme.SecondaryDark);
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
            }
            else
            {
                // Fallback to regular Colors if the AppThemeColor is not available
                resources[nameof(theme.Primary)] = theme.Primary;
                resources[nameof(theme.OnPrimary)] = theme.OnPrimary;

                resources[nameof(theme.Secondary)] = theme.Secondary;
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
