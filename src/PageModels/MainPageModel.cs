using ColorThemes.Models;
using ColorThemes.Repositories;
using ColorThemes.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Scriban;
using Microsoft.Maui.Controls;

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
    
    [ObservableProperty]
    private string _currentThemeDetails = string.Empty;
    
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
        
        // Initialize theme details (will update once themes are loaded)
        UpdateCurrentThemeDetails();
        
        // Load themes when the model is created
        _ = LoadThemesAsync();
        
        // Schedule an update after the app has fully loaded
        MainThread.InvokeOnMainThreadAsync(async () => 
        {
            // Small delay to ensure the app is fully loaded and theme is applied
            await Task.Delay(500);
            UpdateThemeName();
            UpdateCurrentThemeDetails();
        });
    }
    
    /// <summary>
    /// Handler for theme changed events
    /// </summary>
    private void OnThemeChanged(object? sender, EventArgs e)
    {
        UpdateThemeName();
        UpdateCurrentThemeDetails();
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
                // Update the current theme name and details in the UI
                CurrentThemeName = SelectedTheme.Title;
                UpdateCurrentThemeDetails();
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
            
            // Make sure we have a current theme to work with
            if (_themeService.CurrentTheme == null)
                return;
                
            // Use the current theme from ThemeService
            var theme = _themeService.CurrentTheme;
            bool isDarkTheme = Application.Current?.RequestedTheme == AppTheme.Dark;
            
            // Generate the theme using the same values shown in the Editor
            // Use the Generate method with both light and dark background colors
            // This ensures the generated code matches exactly what's shown in the Editor
            var currentTheme = ColorThemeGenerator.Generate(
                primary: Color.FromArgb(theme.PrimaryHex),
                secondary: Color.FromArgb(theme.SecondaryHex),
                lightBackground: Color.FromArgb(theme.BackgroundHex),
                darkBackground: Color.FromArgb(theme.BackgroundDarkHex),
                isDarkTheme: isDarkTheme);

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
    /// Updates the current theme name and details from the ThemeService
    /// </summary>
    public void UpdateThemeName()
    {
        this.CurrentThemeName = _themeService.CurrentTheme?.Title ?? "Custom";
        UpdateCurrentThemeDetails();
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
        
        // Update the current theme name and details
        CurrentThemeName = themeTitle;
        UpdateCurrentThemeDetails();
        
        // Inform the user
        await Shell.Current.DisplayAlert("Success", $"Theme '{themeTitle}' applied successfully", "OK");
    }

    /// <summary>
    /// Toggles between light and dark themes
    /// </summary>
    [RelayCommand]
    private void ToggleTheme()
    {
        // Get the current app theme
        var currentTheme = Application.Current?.RequestedTheme ?? AppTheme.Light;

        // Toggle the theme
        if (currentTheme == AppTheme.Light)
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = AppTheme.Dark;
        }
        else
        {
            if (Application.Current != null)
                Application.Current.UserAppTheme = AppTheme.Light;
        }
        
        // Update theme details to reflect the new mode
        UpdateCurrentThemeDetails();
    }

    /// <summary>
    /// Updates the current theme details with all hex values for both light and dark modes
    /// </summary>
    private void UpdateCurrentThemeDetails()
    {
        if (Application.Current?.Resources == null || _themeService.CurrentTheme == null)
        {
            return;
        }
        
        var theme = _themeService.CurrentTheme;
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Theme: {CurrentThemeName}");
        sb.AppendLine();
        
        // Show original theme values from the theme object
        sb.AppendLine("// Theme Source Values");
        sb.AppendLine($"Primary (Light): {theme.PrimaryHex}");
        sb.AppendLine($"Secondary (Light): {theme.SecondaryHex}");
        sb.AppendLine($"Background (Light): {theme.BackgroundHex}");
        sb.AppendLine($"Background (Dark): {theme.BackgroundDarkHex}");
        sb.AppendLine();
        
        // Get the current mode
        var isDarkMode = Application.Current.RequestedTheme == AppTheme.Dark;
        sb.AppendLine($"// Current Theme Mode: {(isDarkMode ? "Dark" : "Light")}");
        sb.AppendLine();
        
        // Show both light and dark variants side by side
        sb.AppendLine("// Current Applied Colors");
        sb.AppendLine("// Color Name       | Light Mode         | Dark Mode");
        sb.AppendLine("// -------------------------------------------------");
        
        // Generate colors for both modes and display them side by side
        var darkPrimary = ColorThemeGenerator.AdjustColorForDarkTheme(Color.FromArgb(theme.PrimaryHex));
        var darkSecondary = ColorThemeGenerator.AdjustColorForDarkTheme(Color.FromArgb(theme.SecondaryHex));
        
        sb.AppendLine($"Primary           | {theme.PrimaryHex} | {ColorToHex(darkPrimary)}");
        sb.AppendLine($"Secondary         | {theme.SecondaryHex} | {ColorToHex(darkSecondary)}");
        sb.AppendLine($"Background        | {theme.BackgroundHex} | {theme.BackgroundDarkHex}");
        sb.AppendLine();
        
        // Format the output with color names and hex values for current mode
        sb.AppendLine($"// Current Mode Colors ({(isDarkMode ? "Dark" : "Light")})");
        AppendColorIfExists(sb, "Primary");
        AppendColorIfExists(sb, "Secondary");
        AppendColorIfExists(sb, "Tertiary");
        AppendColorIfExists(sb, "Background");
        sb.AppendLine();
        
        sb.AppendLine("// Surface Colors");
        AppendColorIfExists(sb, "Surface0");
        AppendColorIfExists(sb, "Surface1");
        AppendColorIfExists(sb, "Surface2");
        AppendColorIfExists(sb, "Surface3");
        sb.AppendLine();
        
        sb.AppendLine("// Text and Content Colors");
        AppendColorIfExists(sb, "OnBackground");
        AppendColorIfExists(sb, "OnSurface");
        sb.AppendLine();
        
        sb.AppendLine("// Feedback Colors");
        AppendColorIfExists(sb, "Error");
        AppendColorIfExists(sb, "Success");
        AppendColorIfExists(sb, "Info");
        
        CurrentThemeDetails = sb.ToString();
    }
    
    /// <summary>
    /// Gets the hex value of a color resource
    /// </summary>
    private string GetColorHex(ResourceDictionary resources, string colorName)
    {
        if (resources.TryGetValue(colorName, out var colorValue) && colorValue is Color color)
        {
            return ColorToHex(color);
        }
        return "#??????";
    }
    
    /// <summary>
    /// Helper method to append a color and its hex value to the StringBuilder
    /// </summary>
    private void AppendColorIfExists(System.Text.StringBuilder sb, string colorName)
    {
        if (Application.Current?.Resources == null)
        {
            return;
        }
        
        if (Application.Current.Resources.TryGetValue(colorName, out var colorValue) && colorValue is Color color)
        {
            sb.AppendLine($"{colorName}: {ColorToHex(color)}");
        }
    }
    
    /// <summary>
    /// Converts a Color to its hex representation
    /// </summary>
    private static string ColorToHex(Color color)
    {
        return $"#{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
    }
}