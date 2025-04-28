using Theming;
using System.IO;
using System.Globalization;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Path = System.IO.Path;

namespace dotnet_colorthemes;

public partial class MainPage : ContentPage
{
    private ColorTheme? _currentTheme;
      public MainPage()
    {
        InitializeComponent();

        // Add a handler to update the color hex values when the app theme changes
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += (s, e) => UpdateColorSwatches();
        
        // Initial update of color swatches
        UpdateColorSwatches();
    }
    
    /// <summary>
    /// Updates all color swatches with the current theme colors
    /// </summary>
    private void UpdateColorSwatches()
    {
        if (Application.Current?.Resources == null)
            return;
            
        // Update hex labels for all color boxes
        UpdateHexLabel(PrimaryColorBox, PrimaryHexLabel, "Primary");
        UpdateHexLabel(SecondaryColorBox, SecondaryHexLabel, "Secondary");
        UpdateHexLabel(TertiaryColorBox, TertiaryHexLabel, "Tertiary");
        
        UpdateHexLabel(Surface0ColorBox, Surface0HexLabel, "Surface0");
        UpdateHexLabel(Surface1ColorBox, Surface1HexLabel, "Surface1");
        UpdateHexLabel(Surface2ColorBox, Surface2HexLabel, "Surface2");
        
        UpdateHexLabel(ErrorColorBox, ErrorHexLabel, "Error");
        UpdateHexLabel(SuccessColorBox, SuccessHexLabel, "Success");
        UpdateHexLabel(InfoColorBox, InfoHexLabel, "Info");
        
        UpdateHexLabel(BackgroundColorBox, BackgroundHexLabel, "Background");
        UpdateHexLabel(OnBackgroundColorBox, OnBackgroundHexLabel, "OnBackground");
        UpdateHexLabel(OnSurfaceColorBox, OnSurfaceHexLabel, "OnSurface");
    }
    
    /// <summary>
    /// Updates a hex label with the hex value of the associated color
    /// </summary>
    private void UpdateHexLabel(BoxView colorBox, Label hexLabel, string resourceKey)
    {
        if (Application.Current?.Resources == null)
            return;
            
        if (Application.Current.Resources.TryGetValue(resourceKey, out var colorValue) && colorValue is Color color)
        {
            // Make sure the color box has the latest color
            colorBox.Color = color;
            
            // Update the hex label
            hexLabel.Text = ColorToHex(color);
        }
        else
        {
            hexLabel.Text = "#??????";
        }
    }
    
    /// <summary>
    /// Converts a Color to its hex representation
    /// </summary>
    private static string ColorToHex(Color color)
    {
        return $"#{(int)(color.Red * 255):X2}{(int)(color.Green * 255):X2}{(int)(color.Blue * 255):X2}";
    }

    private void Button_Clicked(object sender, EventArgs e)
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
    }

    private void GenerateXamlButton_Clicked(object sender, EventArgs e)
    {
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
            _currentTheme = ColorThemeGenerator.Generate(primary, secondary, background, isDarkTheme);
            
            // Generate XAML
            string xaml = _currentTheme.ToXaml("ThemeColors");
            
            // Display in the editor
            ThemeXamlEditor.Text = xaml;
            
            // Enable save button
            SaveXamlButton.IsEnabled = true;
        }
    }

    private async void SaveXamlButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ThemeXamlEditor.Text))
            return;
            
        try
        {
#if WINDOWS
            // On Windows, use file picker
            var fileSaver = new Windows.Storage.Pickers.FileSavePicker();
            fileSaver.FileTypeChoices.Add("XAML Files", new List<string>() { ".xaml" });
            fileSaver.SuggestedFileName = "ThemeColors";
            var file = await fileSaver.PickSaveFileAsync();
            if (file != null)
            {
                await File.WriteAllTextAsync(file.Path, ThemeXamlEditor.Text);
                await DisplayAlert("Success", "XAML file saved successfully", "OK");
            }
#else
            // On other platforms, use a predefined path
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = "ThemeColors.xaml";
            string filePath = Path.Combine(documentsPath, fileName);
            
            await File.WriteAllTextAsync(filePath, ThemeXamlEditor.Text);
            await DisplayAlert("Success", $"XAML file saved to {filePath}", "OK");
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
        }
    }

    
}
