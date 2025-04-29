using ColorThemes.Models;
using ColorThemes.Repositories;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace ColorThemes.PageModels;

/// <summary>
/// View model for the main page that handles theme management and UI interactions
/// </summary>
public partial class MainPageModel : ObservableObject
{
    private readonly ThemeRepository _themeRepository;
    
    [ObservableProperty]
    private ObservableCollection<ColorTheme> _themes = new();
    
    [ObservableProperty]
    private ColorTheme? _selectedTheme;
    
    [ObservableProperty]
    private string _themeTitle = string.Empty;
    
    [ObservableProperty]
    private bool _isBusy;
    
    [ObservableProperty]
    private bool _isEditMode;
    
    /// <summary>
    /// Initializes a new instance of the MainPageModel class.
    /// </summary>
    /// <param name="themeRepository">The repository for managing themes</param>
    public MainPageModel(ThemeRepository themeRepository)
    {
        _themeRepository = themeRepository;
        
        // Load themes when the model is created
        LoadThemesAsync();
    }
    
    /// <summary>
    /// Loads themes from the repository
    /// </summary>
    private async void LoadThemesAsync()
    {
        try
        {
            IsBusy = true;
            
            // Clear existing themes
            Themes.Clear();
            
            // Load themes from repository
            var loadedThemes = await _themeRepository.GetAllThemesAsync();
            
            // Add loaded themes to the collection
            foreach (var theme in loadedThemes)
            {
                Themes.Add(theme);
            }
            
            // Select the first theme if available
            SelectedTheme = Themes.FirstOrDefault();
            
            if (SelectedTheme != null)
            {
                // Apply the selected theme
                await App.ApplyThemeByTitleAsync(SelectedTheme.Title);
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error loading themes: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Creates a new theme with the current color settings
    /// </summary>
    [RelayCommand]
    async Task CreateNewThemeAsync()
    {
        if (string.IsNullOrWhiteSpace(ThemeTitle))
        {
            await App.DisplayToastAsync("Please enter a title for the theme");
            return;
        }
        
        try
        {
            IsBusy = true;
            
            // Create a new theme based on current app colors
            var newTheme = new ColorTheme
            {
                Title = ThemeTitle,
                PrimaryHex = App.Primary,
                SecondaryHex = App.Secondary,
                BackgroundHex = App.LightBackground,
                BackgroundDarkHex = App.DarkBackground
            };
            
            // Update all color values from hex strings
            newTheme.UpdateColorsFromHex();
            
            // Add to repository
            bool success = await _themeRepository.AddThemeAsync(newTheme);
            
            if (success)
            {
                // Add to the collection
                Themes.Add(newTheme);
                
                // Select the new theme
                SelectedTheme = newTheme;
                
                // Clear the title field
                ThemeTitle = string.Empty;
                
                await App.DisplayToastAsync($"Theme '{newTheme.Title}' created successfully");
            }
            else
            {
                await App.DisplayToastAsync($"A theme with the title '{newTheme.Title}' already exists");
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error creating theme: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task SaveTheme()
    {
        var existingTheme = Themes.FirstOrDefault(t => 
            t.Title.Equals(SelectedTheme.Title, StringComparison.OrdinalIgnoreCase) && 
            t != SelectedTheme);

        try
        {
            IsBusy = true;

            // Update all color values from hex strings
            SelectedTheme.UpdateColorsFromHex();

            // Update in repository
            bool success = await _themeRepository.UpdateThemeAsync(SelectedTheme);

            if (success)
            {
                await App.DisplayToastAsync($"Theme '{SelectedTheme.Title}' saved successfully");

                if (existingTheme == null)
                {
                    // Remove the old theme from the collection
                    Themes.Add(SelectedTheme);
                }

                // Force UI refresh
                OnPropertyChanged(nameof(Themes));
            }
            else
            {
                await App.DisplayToastAsync($"Failed to save theme '{SelectedTheme.Title}'");
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error saving theme: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Updates an existing theme with current color settings
    /// </summary>
    [RelayCommand]
    async Task UpdateThemeAsync()
    {
        if (SelectedTheme == null)
        {
            await App.DisplayToastAsync("Please select a theme to update");
            return;
        }
        
        try
        {
            IsBusy = true;
            
            // Update the selected theme with current app colors
            SelectedTheme.PrimaryHex = App.Primary;
            SelectedTheme.SecondaryHex = App.Secondary;
            SelectedTheme.BackgroundHex = App.LightBackground;
            SelectedTheme.BackgroundDarkHex = App.DarkBackground;
            
            // Update all color values from hex strings
            SelectedTheme.UpdateColorsFromHex();
            
            // Update in repository
            bool success = await _themeRepository.UpdateThemeAsync(SelectedTheme);
            
            if (success)
            {
                await App.DisplayToastAsync($"Theme '{SelectedTheme.Title}' updated successfully");
                
                // Force UI refresh
                OnPropertyChanged(nameof(Themes));
            }
            else
            {
                await App.DisplayToastAsync($"Failed to update theme '{SelectedTheme.Title}'");
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error updating theme: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    
    
    /// <summary>
    /// Deletes the selected theme
    /// </summary>
    [RelayCommand]
    async Task DeleteThemeAsync()
    {
        if (SelectedTheme == null)
        {
            await App.DisplayToastAsync("Please select a theme to delete");
            return;
        }
        
        try
        {
            IsBusy = true;
            
            string themeTitle = SelectedTheme.Title;
            
            // Delete from repository
            bool success = await _themeRepository.DeleteThemeAsync(themeTitle);
            
            if (success)
            {
                // Remove from collection
                Themes.Remove(SelectedTheme);
                
                // Select the first theme if available
                SelectedTheme = Themes.FirstOrDefault();
                
                await App.DisplayToastAsync($"Theme '{themeTitle}' deleted successfully");
            }
            else
            {
                await App.DisplayToastAsync($"Failed to delete theme '{themeTitle}'");
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error deleting theme: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Applies the selected theme
    /// </summary>
    [RelayCommand]
    async Task ApplySelectedThemeAsync()
    {
        if (SelectedTheme == null)
        {
            await App.DisplayToastAsync("Please select a theme to apply");
            return;
        }
        
        try
        {
            IsBusy = true;
            
            // Apply the selected theme
            bool success = await App.ApplyThemeByTitleAsync(SelectedTheme.Title);
            
            if (success)
            {
                await App.DisplayToastAsync($"Theme '{SelectedTheme.Title}' applied successfully");
            }
            else
            {
                await App.DisplayToastAsync($"Failed to apply theme '{SelectedTheme.Title}'");
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error applying theme: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task LoadTheme(ColorTheme theme)
    {
        SelectedTheme = theme;
    }
    
    /// <summary>
    /// Refreshes the theme list from the repository
    /// </summary>
    [RelayCommand]
    async Task RefreshThemesAsync()
    {
        try
        {
            IsBusy = true;
            
            // Clear existing themes
            Themes.Clear();
            
            // Load themes from repository
            var loadedThemes = await _themeRepository.GetAllThemesAsync();
            
            // Add loaded themes to the collection
            foreach (var theme in loadedThemes)
            {
                Themes.Add(theme);
            }
            
            // Select the first theme if available and none is currently selected
            if (SelectedTheme == null)
            {
                SelectedTheme = Themes.FirstOrDefault();
            }
            
            await App.DisplayToastAsync("Themes refreshed");
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error refreshing themes: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}