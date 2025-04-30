using ColorThemes.Models;
using ColorThemes.Repositories;
using ColorThemes.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Scriban;

namespace ColorThemes.PageModels;

/// <summary>
/// View model for the main page that handles theme management and UI interactions
/// </summary>
public partial class MainPageModel : ObservableObject
{
    private readonly ThemeRepository _themeRepository;
    private readonly ThemeService _themeService;
    
    [ObservableProperty]
    private List<ColorTheme> _themes = new();
    
    [ObservableProperty]
    private ColorTheme? _selectedTheme;
    
    [ObservableProperty]
    private string _themeTitle = string.Empty;
    
    [ObservableProperty]
    private bool _isBusy;
    
    [ObservableProperty]
    private bool _isEditMode;
    
    [ObservableProperty]
    private string _generatedCode = string.Empty;
    
    [ObservableProperty]
    private bool _includeStyles;
    
    [ObservableProperty]
    private string _selectedCodeFormat = "AppThemeColor";
    
    [ObservableProperty]
    private bool _canSaveGeneratedCode;
    
    [ObservableProperty]
    private string _currentThemeName = string.Empty;
    
    /// <summary>
    /// Initializes a new instance of the MainPageModel class.
    /// </summary>
    /// <param name="themeRepository">The repository for managing themes</param>
    /// <param name="themeService">The service for managing themes</param>
    public MainPageModel(ThemeRepository themeRepository, ThemeService themeService)
    {
        _themeRepository = themeRepository;
        _themeService = themeService;
        
        // Set default values
        IncludeStyles = true;
        
        // Set initial theme name
        if (_themeService.CurrentTheme != null)
        {
            CurrentThemeName = _themeService.CurrentTheme.Title;
        }
        else
        {
            CurrentThemeName = "Default";
        }
        
        // Subscribe to theme changed events
        _themeService.ThemeChanged += OnThemeChanged;
        
        // Load themes when the model is created
        _ = LoadThemesAsync();
    }
    
    /// <summary>
    /// Handler for theme changed events
    /// </summary>
    private void OnThemeChanged(object? sender, EventArgs e)
    {
        UpdateThemeName();
    }
    
    /// <summary>
    /// Loads themes from the repository
    /// </summary>
    private async Task LoadThemesAsync(bool setSelected = true)
    {
        try
        {
            IsBusy = true;
            
            // Load themes from the theme service
            await _themeService.LoadThemesAsync();
            
            // Copy themes from the service to our viewmodel
            Themes = [.. _themeService.AvailableThemes];

            // Select the first theme if available
            if (setSelected && Themes.Count > 0)
            {
                SelectedTheme = Themes.FirstOrDefault();
                ThemeTitle = SelectedTheme?.Title ?? string.Empty;
            }
            
            if (SelectedTheme != null)
            {
                // Apply the selected theme
                await _themeService.ApplyThemeByTitleAsync(SelectedTheme.Title);
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
    
    [RelayCommand]
    async Task SaveTheme()
    {
        if (SelectedTheme == null)
        {
            await App.DisplayToastAsync("No theme selected to save");
            return;
        }
        
        var existingTheme = await _themeRepository.GetThemeByTitleAsync(ThemeTitle);

        try
        {
            IsBusy = true;

            // Update all color values from hex strings
            SelectedTheme.UpdateColorsFromHex();

            // Update in repository
            bool success = false;
            if (existingTheme != null)
            {
                success = await _themeRepository.UpdateThemeAsync(SelectedTheme);
            }
            else
            {
                // Create a new theme with the current title
                // Create a new theme instance to avoid modifying the original object
                var newTheme = new ColorTheme
                {
                    Title = ThemeTitle,
                    Primary = SelectedTheme.Primary,
                    Secondary = SelectedTheme.Secondary,
                    Background = SelectedTheme.Background,
                    BackgroundDark = SelectedTheme.BackgroundDark
                };
                newTheme.UpdateColorsFromHex();
                success = await _themeRepository.AddThemeAsync(newTheme);
                
                if (success)
                {
                    // Update the selected theme to the new one
                    SelectedTheme = newTheme;
                }
            }

            if (success)
                {
                    await App.DisplayToastAsync($"Theme '{SelectedTheme.Title}' saved successfully");

                    await LoadThemesAsync(false);
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
                await LoadThemesAsync();
                                
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
            bool success = await _themeService.ApplyThemeByTitleAsync(SelectedTheme.Title);
            
            if (success)
            {
                // Update the current theme name in the UI
                CurrentThemeName = SelectedTheme.Title;
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
        if (theme == null)
        {
            return;
        }
        
        SelectedTheme = theme;
        ThemeTitle = theme.Title;
        
        // Apply the theme if needed
        await ApplySelectedThemeAsync();
    }

    // We're using GenerateCodeCommand instead of GenerateColorCodeCommand
    
    /// <summary>
    /// Generates code for the current theme based on the selected format
    /// </summary>
    [RelayCommand]
    async Task GenerateCode(CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            IsBusy = true;
            
            if (Application.Current == null || Application.Current.Resources == null)
                return;
                
            // Extract the current theme from the application resources
            var resources = Application.Current.Resources;
            if (resources.TryGetValue("Primary", out var primaryObj) && 
                resources.TryGetValue("Secondary", out var secondaryObj) && 
                resources.TryGetValue("Background", out var backgroundObj))
            {
                Color primary = (Color)primaryObj;
                Color secondary = (Color)secondaryObj;
                Color background = (Color)backgroundObj;
                bool isDarkTheme = Application.Current.UserAppTheme == AppTheme.Dark;
                
                // Generate the theme
                var currentTheme = ColorThemeGenerator.Generate(primary, secondary, background, isDarkTheme);

                // Determine which format to use based on the selected format
                string templateFileName = GetTemplateFileName(SelectedCodeFormat);

                // Load and parse scriban template
                using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(templateFileName);
                using (StreamReader reader = new StreamReader(templateStream))
                {
                    var template = Template.Parse(await reader.ReadToEndAsync());
                    GeneratedCode = await template.RenderAsync(new
                    {
                        primary = currentTheme.Primary.ToArgbHex(),
                        primary_dark = currentTheme.PrimaryDark.ToArgbHex(),
                        secondary = currentTheme.Secondary.ToArgbHex(),
                        secondary_dark = currentTheme.SecondaryDark.ToArgbHex(),
                        background = currentTheme.Background.ToArgbHex(),
                        background_dark = currentTheme.BackgroundDark.ToArgbHex(),
                        on_background = currentTheme.OnBackground.ToArgbHex(),
                        on_background_dark = currentTheme.OnBackgroundDark.ToArgbHex(),
                        surface_0 = currentTheme.Surface0.ToArgbHex(),
                        surface_0_dark = currentTheme.Surface0Dark.ToArgbHex(),
                        surface_1 = currentTheme.Surface1.ToArgbHex(),
                        surface_1_dark = currentTheme.Surface1Dark.ToArgbHex(),
                        surface_2 = currentTheme.Surface2.ToArgbHex(),
                        surface_2_dark = currentTheme.Surface2Dark.ToArgbHex(),
                        surface_3 = currentTheme.Surface3.ToArgbHex(),
                        surface_3_dark = currentTheme.Surface3Dark.ToArgbHex(),
                        include_styles = IncludeStyles
                    });
                }
                
                CanSaveGeneratedCode = true;
            }
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error generating code: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    /// <summary>
    /// Gets the template file name based on the selected code format
    /// </summary>
    /// <summary>
    /// Gets the template file name based on the selected code format
    /// </summary>
    private string GetTemplateFileName(string format)
    {
        return format switch
        {
            "AppThemeColor" => "AppThemeColor.scriban-txt",
            "AppThemeBinding" => "AppThemeBinding.scriban-txt",
            "DynamicResource" => "DynamicResource.scriban-txt",
            "MauiReactor" => "MauiReactor.scriban-txt",
            "C# Markup (CommunityToolkit)" => "CSharpMarkup.scriban-txt",
            _ => "AppThemeColor.scriban-txt"
        };
    }
    
    /// <summary>
    /// Updates the generated code when the selected code format changes
    /// </summary>
    partial void OnSelectedCodeFormatChanged(string value)
    {
        // Regenerate the code when the format changes
        if (!string.IsNullOrEmpty(GeneratedCode))
        {
            _ = GenerateCode();
        }
    }
    
    /// <summary>
    /// Updates the generated code when the include styles option changes
    /// </summary>
    partial void OnIncludeStylesChanged(bool value)
    {
        // Regenerate the code when the include styles option changes
        if (!string.IsNullOrEmpty(GeneratedCode))
        {
            _ = GenerateCode();
        }
    }
    
    /// <summary>
    /// Gets the file extension for the generated code based on the selected format
    /// </summary>
    public string GetFileExtension()
    {
        return SelectedCodeFormat switch
        {
            "MauiReactor" => ".cs",
            "C# Markup (CommunityToolkit)" => ".cs",
            _ => ".xaml"
        };
    }
    
    /// <summary>
    /// Copies the generated code to clipboard
    /// </summary>
    [RelayCommand]
    async Task CopyToClipboard(CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            
            if (string.IsNullOrEmpty(GeneratedCode))
            {
                await App.DisplayToastAsync("No code to copy");
                return;
            }
            
            await Clipboard.Default.SetTextAsync(GeneratedCode);
            await App.DisplayToastAsync("Copied to clipboard!");
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Error copying to clipboard: {ex.Message}");
        }
    }

    /// <summary>
    /// Saves the generated code to a file
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanSaveGeneratedCode))]
    async Task SaveToFile(CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();
            
            if (string.IsNullOrEmpty(GeneratedCode))
            {
                await App.DisplayToastAsync("No code to save");
                return;
            }
            
            string fileExtension = GetFileExtension();
            string fileName = $"ThemeColors{fileExtension}";

#if WINDOWS
            // On Windows, use file picker
            var fileSaver = new Windows.Storage.Pickers.FileSavePicker();
            
            if (fileExtension == ".xaml")
            {
                fileSaver.FileTypeChoices.Add("XAML Files", new List<string>() { ".xaml" });
            }
            else
            {
                fileSaver.FileTypeChoices.Add("C# Files", new List<string>() { ".cs" });
            }
            
            fileSaver.SuggestedFileName = Path.GetFileNameWithoutExtension(fileName);
            var file = await fileSaver.PickSaveFileAsync();
            if (file != null)
            {
                await File.WriteAllTextAsync(file.Path, GeneratedCode);
                await App.DisplayToastAsync("File saved successfully");
            }
#else
            // On other platforms, use a predefined path
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, fileName);

            await File.WriteAllTextAsync(filePath, GeneratedCode);
            await App.DisplayToastAsync($"File saved to {filePath}");
#endif
        }
        catch (Exception ex)
        {
            await App.DisplayToastAsync($"Failed to save file: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the current theme name from the ThemeService
    /// </summary>
    public void UpdateThemeName()
    {
        this.CurrentThemeName = _themeService.CurrentTheme?.Title ?? "Custom";
    }

    /// <summary>
    /// Command to apply the current color theme
    /// </summary>
    [RelayCommand]
    private async Task ApplyColorsAsync()
    {
        // Get the selected theme title or use "Custom" if none is selected
        string themeTitle = SelectedTheme?.Title ?? "Custom";
        
        // Apply the current theme with the custom colors
        _themeService.ApplyCustomTheme(
            themeTitle,
            SelectedTheme?.PrimaryHex ?? "#000000",
            SelectedTheme?.SecondaryHex ?? "#000000",
            SelectedTheme?.BackgroundHex ?? "#FFFFFF",
            SelectedTheme?.BackgroundDarkHex ?? "#000000"
        );
        
        // Update the current theme name
        CurrentThemeName = themeTitle;
        
        // Inform the user
        await Shell.Current.DisplayAlert("Success", $"Theme '{themeTitle}' applied successfully", "OK");
    }
}