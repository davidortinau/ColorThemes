using ColorThemes.Models;
using ColorThemes.Repositories;

namespace ColorThemes.Services;

public class ThemeService
{
    private readonly ThemeRepository _themeRepository;
    private ColorTheme? _currentTheme;

    // Event that others can subscribe to
    public event EventHandler? ThemeChanged;

    public List<ColorTheme> AvailableThemes { get; private set; } = [];

    public ColorTheme? CurrentTheme
    {
        get => _currentTheme;
        private set
        {
            _currentTheme = value;
            OnThemeChanged();
        }
    }

    public ThemeService(ThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
    }

    public async Task LoadThemesAsync()
    {
        try
        {
            var themes = await _themeRepository.GetAllThemesAsync();
            AvailableThemes = themes.ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading themes: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> ApplyThemeByTitleAsync(string themeTitle)
    {
        var theme = AvailableThemes.FirstOrDefault(t =>
            t.Title.Equals(themeTitle, StringComparison.OrdinalIgnoreCase));

        if (theme == null)
            return false;

        CurrentTheme = theme;

        ApplyThemeColors(
            theme.PrimaryHex,
            theme.SecondaryHex,
            theme.BackgroundHex,
            theme.BackgroundDarkHex
        );

        return true;
    }

    public void ApplyCustomTheme(string title, string primary, string secondary,
                                string lightBackground, string darkBackground)
    {
        var customTheme = new ColorTheme
        {
            Title = title,
            PrimaryHex = primary,
            SecondaryHex = secondary,
            BackgroundHex = lightBackground,
            BackgroundDarkHex = darkBackground
        };

        CurrentTheme = customTheme;

        ApplyThemeColors(primary, secondary, lightBackground, darkBackground);
    }

    public void ApplyThemeColors(string primary, string secondary,
                               string lightBackground, string darkBackground)
    {
        // Check if the app is in dark mode
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        // Generate the theme
        var theme = ColorThemeGenerator.Generate(
            primary: Color.FromArgb(primary),
            secondary: Color.FromArgb(secondary),
            background: isDark ? Color.FromArgb(darkBackground) : Color.FromArgb(lightBackground),
            isDarkTheme: isDark);

        // Apply the theme
        Application.Current?.Resources.ApplyTheme(theme);
    }

    public void HandleAppThemeChanged()
    {
        if (CurrentTheme != null)
        {
            ApplyThemeColors(
                CurrentTheme.PrimaryHex,
                CurrentTheme.SecondaryHex,
                CurrentTheme.BackgroundHex,
                CurrentTheme.BackgroundDarkHex
            );
        }
    }

    // Helper method to raise the ThemeChanged event
    private void OnThemeChanged()
    {
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }
}

