using System.Text.Json;
using ColorThemes.Models;

namespace ColorThemes.Repositories;

/// <summary>
/// Repository for managing ColorTheme objects, providing methods to save, load, and manage themes.
/// </summary>
public class ThemeRepository
{
    private readonly string _themesFilePath;
    private readonly string _defaultThemesFilePath;
    private List<ColorTheme> _cachedThemes = new();
    private bool _isInitialized = false;

    /// <summary>
    /// Initializes a new instance of the ThemeRepository class.
    /// </summary>
    public ThemeRepository()
    {
        _themesFilePath = Path.Combine(FileSystem.AppDataDirectory, "user_themes.json");
        _defaultThemesFilePath = "default_themes.json";
    }

    /// <summary>
    /// Gets all available themes, initializing from storage if needed.
    /// </summary>
    /// <returns>A collection of ColorTheme objects.</returns>
    public async Task<IReadOnlyList<ColorTheme>> GetAllThemesAsync()
    {
        await EnsureInitializedAsync();
        return _cachedThemes;
    }

    /// <summary>
    /// Gets a theme by its title.
    /// </summary>
    /// <param name="title">The title of the theme to retrieve.</param>
    /// <returns>The ColorTheme if found; otherwise, null.</returns>
    public async Task<ColorTheme?> GetThemeByTitleAsync(string title)
    {
        await EnsureInitializedAsync();
        if (string.IsNullOrWhiteSpace(title))
            return null;
            
        // Use explicit loop to ensure proper comparison
        foreach (var theme in _cachedThemes)
        {
            if (string.Equals(theme.Title, title, StringComparison.OrdinalIgnoreCase))
            {
                return theme;
            }
        }
        
        return null;
    }

    /// <summary>
    /// Adds a new theme to the collection.
    /// </summary>
    /// <param name="theme">The theme to add.</param>
    /// <returns>True if the theme was added successfully; otherwise, false.</returns>
    public async Task<bool> AddThemeAsync(ColorTheme theme)
    {
        await EnsureInitializedAsync();

        if (theme == null || string.IsNullOrWhiteSpace(theme.Title))
            return false;

        // Check if a theme with the same title already exists using explicit comparison
        foreach (var existingTheme in _cachedThemes)
        {
            if (string.Equals(existingTheme.Title, theme.Title, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        _cachedThemes.Add(theme);
        await SaveThemesAsync();
        return true;
    }

    /// <summary>
    /// Updates an existing theme.
    /// </summary>
    /// <param name="theme">The updated theme.</param>
    /// <returns>True if the theme was updated successfully; otherwise, false.</returns>
    public async Task<bool> UpdateThemeAsync(ColorTheme theme)
    {
        await EnsureInitializedAsync();

        if (theme == null || string.IsNullOrWhiteSpace(theme.Title))
            return false;

        // Find the index using an explicit loop with proper comparison
        for (int i = 0; i < _cachedThemes.Count; i++)
        {
            if (string.Equals(_cachedThemes[i].Title, theme.Title, StringComparison.OrdinalIgnoreCase))
            {
                // Verify we found the right theme
                System.Diagnostics.Debug.WriteLine($"Updating theme at index {i}: {_cachedThemes[i].Title} -> {theme.Title}");
                
                _cachedThemes[i] = theme;
                await SaveThemesAsync();
                return true;
            }
        }

        // No matching theme found
        System.Diagnostics.Debug.WriteLine($"No theme found with title: {theme.Title}");
        return false;
    }

    /// <summary>
    /// Deletes a theme by its title.
    /// </summary>
    /// <param name="title">The title of the theme to delete.</param>
    /// <returns>True if the theme was deleted successfully; otherwise, false.</returns>
    public async Task<bool> DeleteThemeAsync(string title)
    {
        await EnsureInitializedAsync();

        if (string.IsNullOrWhiteSpace(title))
            return false;

        // Find the index using an explicit loop with proper comparison
        for (int i = 0; i < _cachedThemes.Count; i++)
        {
            if (string.Equals(_cachedThemes[i].Title, title, StringComparison.OrdinalIgnoreCase))
            {
                // Verify we found the right theme
                System.Diagnostics.Debug.WriteLine($"Deleting theme at index {i}: {_cachedThemes[i].Title}");
                
                _cachedThemes.RemoveAt(i);
                await SaveThemesAsync();
                return true;
            }
        }

        // No matching theme found
        System.Diagnostics.Debug.WriteLine($"No theme found with title: {title}");
        return false;
    }

    /// <summary>
    /// Debug helper to inspect the current themes in the repository
    /// </summary>
    public void DebugThemeCache()
    {
        System.Diagnostics.Debug.WriteLine($"Theme Cache Contents (Count: {_cachedThemes.Count}):");
        for (int i = 0; i < _cachedThemes.Count; i++)
        {
            var theme = _cachedThemes[i];
            System.Diagnostics.Debug.WriteLine($"  [{i}] Title: \"{theme.Title}\", Primary: {theme.PrimaryHex}, Secondary: {theme.SecondaryHex}");
        }
    }

    /// <summary>
    /// Ensures the repository is initialized by loading themes from storage or seeding default themes.
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (_isInitialized)
        {
            return;
        }

        try
        {
            if (File.Exists(_themesFilePath))
            {
                string json = await File.ReadAllTextAsync(_themesFilePath);
                var options = new JsonSerializerOptions { 
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                };
                
                var themes = JsonSerializer.Deserialize<List<ColorTheme>>(json, options);
                if (themes != null && themes.Count > 0)
                {
                    _cachedThemes = themes;
                    _isInitialized = true;
                    
                    // Initialize the actual Color objects from hex strings
                    foreach (var theme in _cachedThemes)
                    {
                        theme.UpdateColorsFromHex();
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Loaded {_cachedThemes.Count} themes from storage");
                    DebugThemeCache();
                    return;
                }
            }

            // If no themes were loaded, seed with default themes
            await SeedDefaultThemesAsync();
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            // Log the exception and try to seed default themes
            System.Diagnostics.Debug.WriteLine($"Error loading themes: {ex.Message}");
            await SeedDefaultThemesAsync();
            _isInitialized = true;
        }
    }

    /// <summary>
    /// Seeds the repository with default themes from the embedded resource.
    /// </summary>
    private async Task SeedDefaultThemesAsync()
    {
        try
        {
            System.Diagnostics.Debug.WriteLine("Seeding default themes...");
            using var stream = await FileSystem.OpenAppPackageFileAsync(_defaultThemesFilePath);
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();
            
            var options = new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            
            var themes = JsonSerializer.Deserialize<List<ColorTheme>>(json, options);
            if (themes != null)
            {
                _cachedThemes = themes;
                
                // Initialize the actual Color objects from hex strings
                foreach (var theme in _cachedThemes)
                {
                    theme.UpdateColorsFromHex();
                }
                
                System.Diagnostics.Debug.WriteLine($"Seeded {_cachedThemes.Count} default themes");
                DebugThemeCache();
                
                await SaveThemesAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error seeding default themes: {ex.Message}");
            // If we can't load default themes, at least create an empty list
            _cachedThemes = new List<ColorTheme>();
        }
    }

    /// <summary>
    /// Saves the current collection of themes to storage.
    /// </summary>
    private async Task SaveThemesAsync()
    {
        try
        {
            // Make sure all hex values are up-to-date before serializing
            foreach (var theme in _cachedThemes)
            {
                theme.UpdateHexFromColors();
            }
            
            var options = new JsonSerializerOptions
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            string json = JsonSerializer.Serialize(_cachedThemes, options);
            
            // Ensure the directory exists
            string? directory = Path.GetDirectoryName(_themesFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(_themesFilePath, json);
            System.Diagnostics.Debug.WriteLine($"Saved {_cachedThemes.Count} themes to {_themesFilePath}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving themes: {ex.Message}");
            await App.DisplayToastAsync($"Failed to save themes: {ex.Message}");
        }
    }
}
