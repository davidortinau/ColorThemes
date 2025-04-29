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
        return _cachedThemes.FirstOrDefault(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Adds a new theme to the collection.
    /// </summary>
    /// <param name="theme">The theme to add.</param>
    /// <returns>True if the theme was added successfully; otherwise, false.</returns>
    public async Task<bool> AddThemeAsync(ColorTheme theme)
    {
        await EnsureInitializedAsync();

        // Check if a theme with the same title already exists
        if (_cachedThemes.Any(t => t.Title.Equals(theme.Title, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
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

        int index = _cachedThemes.FindIndex(t => t.Title.Equals(theme.Title, StringComparison.OrdinalIgnoreCase));
        if (index == -1)
        {
            return false;
        }

        _cachedThemes[index] = theme;
        await SaveThemesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a theme by its title.
    /// </summary>
    /// <param name="title">The title of the theme to delete.</param>
    /// <returns>True if the theme was deleted successfully; otherwise, false.</returns>
    public async Task<bool> DeleteThemeAsync(string title)
    {
        await EnsureInitializedAsync();

        int index = _cachedThemes.FindIndex(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        if (index == -1)
        {
            return false;
        }

        _cachedThemes.RemoveAt(index);
        await SaveThemesAsync();
        return true;
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
                var themes = JsonSerializer.Deserialize<List<ColorTheme>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (themes != null && themes.Count > 0)
                {
                    _cachedThemes = themes;
                    _isInitialized = true;
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
            using var stream = await FileSystem.OpenAppPackageFileAsync(_defaultThemesFilePath);
            using var reader = new StreamReader(stream);
            string json = await reader.ReadToEndAsync();
            
            var themes = JsonSerializer.Deserialize<List<ColorTheme>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (themes != null)
            {
                _cachedThemes = themes;
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
            string json = JsonSerializer.Serialize(_cachedThemes, new JsonSerializerOptions 
            { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            // Ensure the directory exists
            string? directory = Path.GetDirectoryName(_themesFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(_themesFilePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving themes: {ex.Message}");
            await App.DisplayToastAsync($"Failed to save themes: {ex.Message}");
        }
    }
}
