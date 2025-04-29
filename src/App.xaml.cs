using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using ColorThemes.Models;
using ColorThemes.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Theming;

namespace ColorThemes;

public partial class App : Application
{
    public static string Primary = "#F04B25";
    public static string Secondary = "#AC3420";
    public static string LightBackground = "#FBF8D4";
    public static string DarkBackground = "#1C201E";
    
    // Collection of available themes
    public static List<ColorTheme> AvailableThemes { get; private set; } = new List<ColorTheme>();
    
    // Theme repository for persistence
    private ThemeRepository? _themeRepository;
    
    // Currently selected theme
    public static ColorTheme? CurrentTheme { get; private set; }
    
    public App()
    {
        InitializeComponent();

        ApplyCurrentTheme();        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
            
        // Initialize themes in the background
        InitializeThemesAsync();
    }
    
    /// <summary>
    /// Initializes themes by loading them from the repository.
    /// </summary>
    private async void InitializeThemesAsync()
    {
        try
        {
            _themeRepository = Handler?.MauiContext?.Services.GetService<ThemeRepository>();
            if (_themeRepository != null)
            {
                var themes = await _themeRepository.GetAllThemesAsync();
                AvailableThemes = themes.ToList();
                
                // Display a toast when themes are loaded
                if (AvailableThemes.Count > 0)
                {
                    await DisplayToastAsync($"Loaded {AvailableThemes.Count} color themes");
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading themes: {ex.Message}");
            await DisplayToastAsync("Failed to load themes");
        }
    }
    
    /// <summary>
    /// Applies a theme by its title.
    /// </summary>
    /// <param name="themeTitle">The title of the theme to apply.</param>
    /// <returns>True if the theme was applied successfully; otherwise, false.</returns>
    public static async Task<bool> ApplyThemeByTitleAsync(string themeTitle)
    {
        var theme = AvailableThemes.FirstOrDefault(t => t.Title.Equals(themeTitle, StringComparison.OrdinalIgnoreCase));
        if (theme == null)
            return false;
            
        CurrentTheme = theme;
        
        // Apply the theme
        ApplyCurrentTheme(theme.PrimaryHex, theme.SecondaryHex, theme.BackgroundHex, theme.BackgroundDarkHex);
        
        return true;
    }
    
    void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        // If we have a current theme, use its colors
        if (CurrentTheme != null)
        {
            ApplyCurrentTheme(
                CurrentTheme.PrimaryHex, 
                CurrentTheme.SecondaryHex, 
                CurrentTheme.BackgroundHex, 
                CurrentTheme.BackgroundDarkHex
            );
        }
        else
        {
            // Otherwise use default colors
            ApplyCurrentTheme(Primary, Secondary, LightBackground, DarkBackground);
        }
    }

    public static void ApplyCurrentTheme(string primary = "#F04B25", string secondary = "#AC3420", string lightBackground = "#FBF8D4", string darkBackground = "#1C201E")
    {
        // save theme colors to static variables
        Primary = primary;
        Secondary = secondary;
        LightBackground = lightBackground;
        DarkBackground = darkBackground;

        // Check if the app is in dark mode
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        // Generate using your base colors (e.g. from config or defaults)
        var theme = ColorThemeGenerator.Generate(
            primary: Color.FromArgb(primary),
            secondary: Color.FromArgb(secondary),
            background: isDark ? Color.FromArgb(darkBackground) : Color.FromArgb(lightBackground),
            isDarkTheme: isDark);

        // Overwrite all the DynamicResources
        App.Current?.Resources.ApplyTheme(theme);
    }


    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    public static Task DisplayToastAsync(string message)
        {
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;
            var toast = Toast.Make(message, duration, fontSize);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            return toast.Show(cancellationTokenSource.Token);
        }
}