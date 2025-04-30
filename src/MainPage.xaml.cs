using System.Text;
using Path = System.IO.Path;
using Scriban;
using ColorThemes.Models;
using ColorThemes.PageModels;

namespace ColorThemes;

/// <summary>
/// Represents the available code generation formats for color themes
/// </summary>
public enum CodeGenerationFormat
{
    AppThemeColor,
    AppThemeBinding,
    DynamicResource,
    MauiReactor,
    CSharpMarkup
}

public partial class MainPage : ContentPage
{
    private readonly MainPageModel _viewModel;
    
    public MainPage(MainPageModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // We don't need to initialize the CodeFormatPicker.ItemsSource here
        // since it's now done in XAML with data binding
        
        // Add a handler to update the color hex values when the app theme changes
        if (Application.Current != null)
        {
            Application.Current.RequestedThemeChanged += (s, e) => 
            {
                UpdateColorSwatches();
                _viewModel.UpdateThemeName();
            };
        }
            
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
        
        UpdateHexLabel(BackgroundBox, BackgroundHexLabel, "Background");
        UpdateHexLabel(OnBackgroundBox, OnBackgroundHexLabel, "OnBackground");
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

    private void ToggleThemeButton_Clicked(object sender, EventArgs e)
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
                Application.Current.UserAppTheme = AppTheme.Light;        }
    }    
    
}
