using Theming;
using System.IO;
using System.Globalization;
using System.Text;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;
using Path = System.IO.Path;

namespace dotnet_colorthemes;

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
    private ColorTheme? _currentTheme;
      public MainPage()
    {
        InitializeComponent();

        // Add a handler to update the color hex values when the app theme changes
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += (s, e) => UpdateColorSwatches();

        // Initial update of color swatches
        // UpdateHexEntry(PrimaryColorEntry, "Primary");
        // UpdateHexEntry(SecondaryColorEntry, "Secondary");        
        // UpdateHexEntry(LightBackgroundColorEntry, "Background");
        // UpdateHexEntry(DarkBackgroundColorEntry, "BackgroundDark");
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

    private void UpdateHexEntry(Entry hexEntry, string resourceKey)
    {
        if (Application.Current?.Resources == null)
            return;
            
        if (Application.Current.Resources.TryGetValue(resourceKey, out var colorValue) && colorValue is Color color)
        {            
            hexEntry.Text = ColorToHex(color);
        }
        else
        {
            hexEntry.Text = "#??????";
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
    }    private void GenerateXamlButton_Clicked(object sender, EventArgs e)
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

            // Determine which format to use based on the selected radio button
            CodeGenerationFormat format = GetSelectedCodeFormat();
            
            // Generate code in the selected format
            string generatedCode = GenerateCodeForTheme(_currentTheme, format);
            
            // Display in the editor
            ThemeXamlEditor.Text = generatedCode;
            
            // Enable save button
            SaveXamlButton.IsEnabled = true;
        }
    }    private async void SaveXamlButton_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(ThemeXamlEditor.Text))
            return;
        
        // Get the file extension based on the selected format
        string fileExtension = GetSelectedCodeFormat() switch
        {
            CodeGenerationFormat.MauiReactor => ".cs",
            CodeGenerationFormat.CSharpMarkup => ".cs",
            _ => ".xaml"
        };
        
        string fileName = $"ThemeColors{fileExtension}";
            
        try
        {
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
                await File.WriteAllTextAsync(file.Path, ThemeXamlEditor.Text);
                await DisplayAlert("Success", "File saved successfully", "OK");
            }
#else
            // On other platforms, use a predefined path
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(documentsPath, fileName);
            
            await File.WriteAllTextAsync(filePath, ThemeXamlEditor.Text);
            await DisplayAlert("Success", $"File saved to {filePath}", "OK");
#endif
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
        }
    }

    private void ApplyColorsButton_Clicked(object sender, EventArgs e)
    {
        App.ApplyCurrentTheme(
            PrimaryColorEntry.Text,
            SecondaryColorEntry.Text,
            LightBackgroundColorEntry.Text,
            DarkBackgroundColorEntry.Text
        );
        
        // Update color swatches to reflect the applied theme
        UpdateColorSwatches();
        
        // Optionally, you can show a message indicating that the theme has been applied
        DisplayAlert("Success", "Theme applied successfully", "OK");
    }
    
    /// <summary>
    /// Determines which code generation format is selected by the user
    /// </summary>
    /// <returns>The selected code generation format</returns>
    private CodeGenerationFormat GetSelectedCodeFormat()
    {
        var selectedFormat = CodeFormatPicker.SelectedItem as string;
        if(selectedFormat == "AppThemeColor")
            return CodeGenerationFormat.AppThemeColor;
        else if(selectedFormat == "AppThemeBinding")
            return CodeGenerationFormat.AppThemeBinding;
        else if(selectedFormat == "DynamicResource")
            return CodeGenerationFormat.DynamicResource;
        else if(selectedFormat == "MauiReactor")
            return CodeGenerationFormat.MauiReactor;
        else if(selectedFormat == "C# Markup (CommunityToolkit)")
            return CodeGenerationFormat.CSharpMarkup;
        
        // Default option
        return CodeGenerationFormat.AppThemeColor;
    }
    
    /// <summary>
    /// Generates code for the theme in the specified format
    /// </summary>
    /// <param name="theme">The color theme to generate code for</param>
    /// <param name="format">The format to generate code in</param>
    /// <returns>Generated code as string</returns>
    private string GenerateCodeForTheme(ColorTheme theme, CodeGenerationFormat format)
    {
        if (theme == null)
            return string.Empty;
            
        return format switch
        {
            CodeGenerationFormat.AppThemeColor => GenerateAppThemeColorCode(theme),
            CodeGenerationFormat.AppThemeBinding => GenerateAppThemeBindingCode(theme),
            CodeGenerationFormat.DynamicResource => GenerateDynamicResourceCode(theme),
            CodeGenerationFormat.MauiReactor => GenerateMauiReactorCode(theme),
            CodeGenerationFormat.CSharpMarkup => GenerateCSharpMarkupCode(theme),
            _ => GenerateAppThemeColorCode(theme)  // Default to AppThemeColor
        };
    }
    
    /// <summary>
    /// Generates code for the theme using AppThemeColor format
    /// </summary>
    private string GenerateAppThemeColorCode(ColorTheme theme)
    {
        // Current implementation - keeping this as is
        return theme.ToXaml("ThemeColors");
    }
      /// <summary>
    /// Generates code for the theme using AppThemeBinding with StaticResources
    /// </summary>
    private string GenerateAppThemeBindingCode(ColorTheme theme)
    {
        if (theme == null)
            return string.Empty;
            
        StringBuilder xaml = new StringBuilder();
        
        // Add XML declaration and header
        xaml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        xaml.AppendLine("<?xaml-comp compile=\"true\" ?>");
        xaml.AppendLine("<ResourceDictionary ");
        xaml.AppendLine("    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
        xaml.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\">");
        xaml.AppendLine();
        xaml.AppendLine("    <!-- Note: For Android please see also Platforms\\Android\\Resources\\values\\colors.xml -->");
        xaml.AppendLine();
        
        // Primary colors
        xaml.AppendLine($"    <Color x:Key=\"Primary\">{ColorToHex(theme.Primary)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"PrimaryDark\">{ColorToHex(theme.PrimaryDark)}</Color>");
        
        // Secondary colors
        xaml.AppendLine($"    <Color x:Key=\"Secondary\">{ColorToHex(theme.Secondary)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"SecondaryDark\">{ColorToHex(theme.SecondaryDark)}</Color>");
        
        // Background colors
        xaml.AppendLine($"    <Color x:Key=\"Background\">{ColorToHex(theme.Background)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"BackgroundDark\">{ColorToHex(theme.BackgroundDark)}</Color>");
        
        // OnBackground colors
        xaml.AppendLine();
        xaml.AppendLine($"    <Color x:Key=\"OnBackground\">{ColorToHex(theme.OnBackground)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"OnBackgroundDark\">{ColorToHex(theme.OnBackgroundDark)}</Color>");
        
        // Surface colors
        xaml.AppendLine();
        xaml.AppendLine($"    <Color x:Key=\"Surface0\">{ColorToHex(theme.Surface0)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface0Dark\">{ColorToHex(theme.Surface0Dark)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface1\">{ColorToHex(theme.Surface1)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface1Dark\">{ColorToHex(theme.Surface1Dark)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface2\">{ColorToHex(theme.Surface2)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface2Dark\">{ColorToHex(theme.Surface2Dark)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface3\">{ColorToHex(theme.Surface3)}</Color>");
        xaml.AppendLine($"    <Color x:Key=\"Surface3Dark\">{ColorToHex(theme.Surface3Dark)}</Color>");
        
        // Gray shades
        xaml.AppendLine();
        xaml.AppendLine("    <Color x:Key=\"Gray100\">#E1E1E1</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray200\">#C8C8C8</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray300\">#ACACAC</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray400\">#919191</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray500\">#6E6E6E</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray600\">#404040</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray900\">#212121</Color>");
        xaml.AppendLine("    <Color x:Key=\"Gray950\">#141414</Color>");
        
        // Example of AppThemeBinding usage
        xaml.AppendLine();
        xaml.AppendLine("    <!-- ");
        xaml.AppendLine("    Example usage with AppThemeBinding:");
        xaml.AppendLine("    <Button BackgroundColor=\"{AppThemeBinding Light={StaticResource Primary}, Dark={StaticResource PrimaryDark}}\"");
        xaml.AppendLine("            TextColor=\"{AppThemeBinding Light={StaticResource OnBackground}, Dark={StaticResource OnBackgroundDark}}\"");
        xaml.AppendLine("            Text=\"Themed Button\" />");
        xaml.AppendLine("    -->");
        xaml.AppendLine();
        xaml.AppendLine("</ResourceDictionary>");
        
        return xaml.ToString();
    }
      /// <summary>
    /// Generates code for the theme using Color with DynamicResources
    /// </summary>
    private string GenerateDynamicResourceCode(ColorTheme theme)
    {
        if (theme == null)
            return string.Empty;

        StringBuilder xaml = new StringBuilder();
        
        // Add XML declaration and header
        xaml.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
        xaml.AppendLine("<?xaml-comp compile=\"true\" ?>");
        xaml.AppendLine("<ResourceDictionary ");
        xaml.AppendLine("    xmlns=\"http://schemas.microsoft.com/dotnet/2021/maui\"");
        xaml.AppendLine("    xmlns:x=\"http://schemas.microsoft.com/winfx/2009/xaml\">");
        xaml.AppendLine();
        
        // Add comment about usage
        xaml.AppendLine("    <!-- ");
        xaml.AppendLine("    Usage example with DynamicResource:");
        xaml.AppendLine("    <Button Background=\"{DynamicResource PrimaryBackground}\" TextColor=\"{DynamicResource PrimaryForeground}\" />");
        xaml.AppendLine("    -->");
        xaml.AppendLine();
        
        // Light theme resources
        xaml.AppendLine("    <!-- Light theme resources -->");
        xaml.AppendLine("    <Style TargetType=\"ResourceDictionary\" Class=\"LightTheme\">");
        xaml.AppendLine("        <Setter Property=\"ResourceDictionary.MergedDictionaries\">");
        xaml.AppendLine("            <ResourceDictionary>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryBackground\">{ColorToHex(theme.Primary)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryForeground\">{ColorToHex(theme.OnPrimary)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryHoverBackground\">{ColorToHex(theme.PrimaryHover)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryPressedBackground\">{ColorToHex(theme.PrimaryPressed)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryDisabledBackground\">{ColorToHex(theme.PrimaryDisabled)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"SecondaryBackground\">{ColorToHex(theme.Secondary)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryForeground\">{ColorToHex(theme.OnSecondary)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryHoverBackground\">{ColorToHex(theme.SecondaryHover)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryPressedBackground\">{ColorToHex(theme.SecondaryPressed)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryDisabledBackground\">{ColorToHex(theme.SecondaryDisabled)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"AppBackground\">{ColorToHex(theme.Background)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"AppForeground\">{ColorToHex(theme.OnBackground)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"Surface0Background\">{ColorToHex(theme.Surface0)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"Surface1Background\">{ColorToHex(theme.Surface1)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"Surface2Background\">{ColorToHex(theme.Surface2)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"Surface3Background\">{ColorToHex(theme.Surface3)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SurfaceForeground\">{ColorToHex(theme.OnSurface)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"BorderColor\">{ColorToHex(theme.Border)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"FocusColor\">{ColorToHex(theme.FocusOutline)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"ErrorColor\">{ColorToHex(theme.Error)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SuccessColor\">{ColorToHex(theme.Success)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"InfoColor\">{ColorToHex(theme.Info)}</Color>");
        xaml.AppendLine("            </ResourceDictionary>");
        xaml.AppendLine("        </Setter>");
        xaml.AppendLine("    </Style>");
        xaml.AppendLine();
        
        // Dark theme resources
        xaml.AppendLine("    <!-- Dark theme resources -->");
        xaml.AppendLine("    <Style TargetType=\"ResourceDictionary\" Class=\"DarkTheme\" ApplyToDerivedTypes=\"True\">");
        xaml.AppendLine("        <Setter Property=\"ResourceDictionary.MergedDictionaries\">");
        xaml.AppendLine("            <ResourceDictionary>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryBackground\">{ColorToHex(theme.PrimaryDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryForeground\">{ColorToHex(theme.OnPrimaryDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryHoverBackground\">{ColorToHex(theme.PrimaryHoverDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryPressedBackground\">{ColorToHex(theme.PrimaryPressedDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"PrimaryDisabledBackground\">{ColorToHex(theme.PrimaryDisabledDark)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"SecondaryBackground\">{ColorToHex(theme.SecondaryDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryForeground\">{ColorToHex(theme.OnSecondaryDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryHoverBackground\">{ColorToHex(theme.SecondaryHoverDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryPressedBackground\">{ColorToHex(theme.SecondaryPressedDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SecondaryDisabledBackground\">{ColorToHex(theme.SecondaryDisabledDark)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"AppBackground\">{ColorToHex(theme.BackgroundDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"AppForeground\">{ColorToHex(theme.OnBackgroundDark)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"Surface0Background\">{ColorToHex(theme.Surface0Dark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"Surface1Background\">{ColorToHex(theme.Surface1Dark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"Surface2Background\">{ColorToHex(theme.Surface2Dark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"Surface3Background\">{ColorToHex(theme.Surface3Dark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SurfaceForeground\">{ColorToHex(theme.OnSurfaceDark)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"BorderColor\">{ColorToHex(theme.BorderDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"FocusColor\">{ColorToHex(theme.FocusOutlineDark)}</Color>");
        xaml.AppendLine();
        xaml.AppendLine($"                <Color x:Key=\"ErrorColor\">{ColorToHex(theme.ErrorDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"SuccessColor\">{ColorToHex(theme.SuccessDark)}</Color>");
        xaml.AppendLine($"                <Color x:Key=\"InfoColor\">{ColorToHex(theme.InfoDark)}</Color>");
        xaml.AppendLine("            </ResourceDictionary>");
        xaml.AppendLine("        </Setter>");
        xaml.AppendLine("    </Style>");
        
        // Add usage notes
        xaml.AppendLine();
        xaml.AppendLine("    <!-- ");
        xaml.AppendLine("    To apply theme in App.xaml:");
        xaml.AppendLine("    <Application.Resources>");
        xaml.AppendLine("        <ResourceDictionary>");
        xaml.AppendLine("            <ResourceDictionary.MergedDictionaries>");
        xaml.AppendLine("                <ResourceDictionary Source=\"Resources/Styles/ThemeColors.xaml\" />");
        xaml.AppendLine("            </ResourceDictionary.MergedDictionaries>");
        xaml.AppendLine("        </ResourceDictionary>");
        xaml.AppendLine("    </Application.Resources>");
        xaml.AppendLine("    -->");
        xaml.AppendLine();
        xaml.AppendLine("</ResourceDictionary>");
        
        return xaml.ToString();
    }
      /// <summary>
    /// Generates code for the theme using MauiReactor format
    /// </summary>
    private string GenerateMauiReactorCode(ColorTheme theme)
    {
        if (theme == null)
            return string.Empty;
            
        StringBuilder code = new StringBuilder();
        
        code.AppendLine("using MauiReactor;");
        code.AppendLine("using System;");
        code.AppendLine("using Microsoft.Maui.Graphics;");
        code.AppendLine();
        
        code.AppendLine("namespace YourApp.Resources.Styles;");
        code.AppendLine("class AppTheme : Theme");
        code.AppendLine("{");
        code.AppendLine("    // Primary colors");
        code.AppendLine($"    public static readonly Color Primary = Color.FromArgb(\"{ColorToHex(theme.Primary)}\");");
        code.AppendLine($"    public static readonly Color PrimaryDark = Color.FromArgb(\"{ColorToHex(theme.PrimaryDark)}\");");
        code.AppendLine();
        
        code.AppendLine("    // Secondary colors");
        code.AppendLine($"    public static readonly Color Secondary = Color.FromArgb(\"{ColorToHex(theme.Secondary)}\");");
        code.AppendLine($"    public static readonly Color SecondaryDark = Color.FromArgb(\"{ColorToHex(theme.SecondaryDark)}\");");
        code.AppendLine();
        
        code.AppendLine("    // Background colors");
        code.AppendLine($"    public static readonly Color Background = Color.FromArgb(\"{ColorToHex(theme.Background)}\");");
        code.AppendLine($"    public static readonly Color BackgroundDark = Color.FromArgb(\"{ColorToHex(theme.BackgroundDark)}\");");
        code.AppendLine();
        
        code.AppendLine("    // OnBackground colors");
        code.AppendLine($"    public static readonly Color OnBackground = Color.FromArgb(\"{ColorToHex(theme.OnBackground)}\");");
        code.AppendLine($"    public static readonly Color OnBackgroundDark = Color.FromArgb(\"{ColorToHex(theme.OnBackgroundDark)}\");");
        code.AppendLine();
        
        code.AppendLine("    // Surface colors");
        code.AppendLine($"    public static readonly Color Surface0 = Color.FromArgb(\"{ColorToHex(theme.Surface0)}\");");
        code.AppendLine($"    public static readonly Color Surface0Dark = Color.FromArgb(\"{ColorToHex(theme.Surface0Dark)}\");");
        code.AppendLine($"    public static readonly Color Surface1 = Color.FromArgb(\"{ColorToHex(theme.Surface1)}\");");
        code.AppendLine($"    public static readonly Color Surface1Dark = Color.FromArgb(\"{ColorToHex(theme.Surface1Dark)}\");");
        code.AppendLine();
        
        code.AppendLine("    // Get theme color based on app theme");
        code.AppendLine("    public static Color GetThemeColor(Color lightThemeColor, Color darkThemeColor)");
        code.AppendLine("    {");
        code.AppendLine("        return Application.Current?.RequestedTheme == AppTheme.Dark ? darkThemeColor : lightThemeColor;");
        code.AppendLine("    }");
        code.AppendLine();
        
        code.AppendLine("    // Helper method to get primary color for current theme");
        code.AppendLine("    public static Color GetPrimaryColor() => GetThemeColor(Primary, PrimaryDark);");
        code.AppendLine();
        
        code.AppendLine("    // Helper method to get secondary color for current theme");
        code.AppendLine("    public static Color GetSecondaryColor() => GetThemeColor(Secondary, SecondaryDark);");
        code.AppendLine();
        
        code.AppendLine("    // Helper method to get background color for current theme");
        code.AppendLine("    public static Color GetBackgroundColor() => GetThemeColor(Background, BackgroundDark);");
        code.AppendLine();
        
        code.AppendLine("    protected override void OnApply()");
        code.AppendLine("    {");
        code.AppendLine("        // Example MauiReactor styles with theme colors applied");
        code.AppendLine("        ButtonStyles.Default = _ => _");
        code.AppendLine("          .TextColor(IsLightTheme ? OnBackground : OnBackgroundDark)");
        code.AppendLine("          .BackgroundColor(IsLightTheme ? Primary : PrimaryDark)");
        code.AppendLine("          .FontSize(14)");
        code.AppendLine("          .BorderWidth(0)");
        code.AppendLine("          .CornerRadius(8)");
        code.AppendLine("          .Padding(14, 10)");
        code.AppendLine("          .MinimumHeightRequest(44)");
        code.AppendLine("          .MinimumWidthRequest(44)");
        code.AppendLine("          .VisualState(\"CommonStates\", \"Disable\", MauiControls.Button.TextColorProperty, IsLightTheme ? Gray950 : Gray200)");
        code.AppendLine("          .VisualState(\"CommonStates\", \"Disable\", MauiControls.Button.BackgroundColorProperty, IsLightTheme ? Gray200 : Gray600);");
        code.AppendLine("");
        code.AppendLine("        ButtonStyles.Themes[\"Secondary\"] = _ => _");
        code.AppendLine("          .TextColor(IsLightTheme ? OnBackground : OnBackgroundDark)");
        code.AppendLine("          .BackgroundColor(IsLightTheme ? Secondary : SecondaryDark)");
        code.AppendLine("          .FontSize(14)");
        code.AppendLine("          .BorderWidth(0)");
        code.AppendLine("          .CornerRadius(8)");
        code.AppendLine("          .Padding(14, 10)");
        code.AppendLine("          .MinimumHeightRequest(44)");
        code.AppendLine("          .MinimumWidthRequest(44)");
        code.AppendLine("          .VisualState(\"CommonStates\", \"Disable\", MauiControls.Button.TextColorProperty, IsLightTheme ? Gray200 : Gray500)");
        code.AppendLine("          .VisualState(\"CommonStates\", \"Disable\", MauiControls.Button.BackgroundColorProperty, IsLightTheme ? Gray300 : Gray600);");
        code.AppendLine("    }");
        code.AppendLine();
        code.AppendLine("}");
        
        return code.ToString();
    }
    
    /// <summary>
    /// Generates code for the theme using C# Markup (CommunityToolkit) format
    /// </summary>
    private string GenerateCSharpMarkupCode(ColorTheme theme)
    {
        if (theme == null)
            return string.Empty;
            
        StringBuilder code = new StringBuilder();
        
        code.AppendLine("using Microsoft.Maui.Controls;");
        code.AppendLine("using Microsoft.Maui.Graphics;");
        code.AppendLine("using CommunityToolkit.Maui.Markup;");
        code.AppendLine("using static CommunityToolkit.Maui.Markup.GridRowsColumns;");
        code.AppendLine();
        
        code.AppendLine("namespace YourApp.Resources");
        code.AppendLine("{");
        code.AppendLine("    public static class ThemeColors");
        code.AppendLine("    {");
        code.AppendLine("        // Initialize theme resources");
        code.AppendLine("        public static void InitializeThemeColors(ResourceDictionary resources)");
        code.AppendLine("        {");
        code.AppendLine("            // Primary colors");
        code.AppendLine($"            resources.Add(\"Primary\", Color.FromArgb(\"{ColorToHex(theme.Primary)}\"));");
        code.AppendLine($"            resources.Add(\"PrimaryDark\", Color.FromArgb(\"{ColorToHex(theme.PrimaryDark)}\"));");
        code.AppendLine();
        
        code.AppendLine("            // Secondary colors");
        code.AppendLine($"            resources.Add(\"Secondary\", Color.FromArgb(\"{ColorToHex(theme.Secondary)}\"));");
        code.AppendLine($"            resources.Add(\"SecondaryDark\", Color.FromArgb(\"{ColorToHex(theme.SecondaryDark)}\"));");
        code.AppendLine();
        
        code.AppendLine("            // Background colors");
        code.AppendLine($"            resources.Add(\"Background\", Color.FromArgb(\"{ColorToHex(theme.Background)}\"));");
        code.AppendLine($"            resources.Add(\"BackgroundDark\", Color.FromArgb(\"{ColorToHex(theme.BackgroundDark)}\"));");
        code.AppendLine();
        
        code.AppendLine("            // OnBackground colors");
        code.AppendLine($"            resources.Add(\"OnBackground\", Color.FromArgb(\"{ColorToHex(theme.OnBackground)}\"));");
        code.AppendLine($"            resources.Add(\"OnBackgroundDark\", Color.FromArgb(\"{ColorToHex(theme.OnBackgroundDark)}\"));");
        code.AppendLine();
        
        code.AppendLine("            // Surface colors");
        code.AppendLine($"            resources.Add(\"Surface0\", Color.FromArgb(\"{ColorToHex(theme.Surface0)}\"));");
        code.AppendLine($"            resources.Add(\"Surface0Dark\", Color.FromArgb(\"{ColorToHex(theme.Surface0Dark)}\"));");
        code.AppendLine($"            resources.Add(\"Surface1\", Color.FromArgb(\"{ColorToHex(theme.Surface1)}\"));");
        code.AppendLine($"            resources.Add(\"Surface1Dark\", Color.FromArgb(\"{ColorToHex(theme.Surface1Dark)}\"));");
        code.AppendLine("            // Add more colors as needed...");
        code.AppendLine("        }");
        code.AppendLine();
        
        code.AppendLine("        // Example of creating a themed button using C# Markup");
        code.AppendLine("        public static Button CreateThemedButton(string text)");
        code.AppendLine("        {");
        code.AppendLine("            return new Button()");
        code.AppendLine("                .Text(text)");
        code.AppendLine("                .AppThemeBinding(");
        code.AppendLine("                    Button.BackgroundColorProperty,");
        code.AppendLine("                    light: AppResource<Color>(\"Primary\"),");
        code.AppendLine("                    dark: AppResource<Color>(\"PrimaryDark\"))");
        code.AppendLine("                .AppThemeBinding(");
        code.AppendLine("                    Button.TextColorProperty,");
        code.AppendLine("                    light: AppResource<Color>(\"OnPrimary\"),");
        code.AppendLine("                    dark: AppResource<Color>(\"OnPrimaryDark\"));");
        code.AppendLine("        }");
        code.AppendLine();
        
        code.AppendLine("        // Example page using themed components with C# Markup");
        code.AppendLine("        public class ThemedPage : ContentPage");
        code.AppendLine("        {");
        code.AppendLine("            public ThemedPage()");
        code.AppendLine("            {");
        code.AppendLine("                Content = new Grid");
        code.AppendLine("                {");
        code.AppendLine("                    RowDefinitions = Rows.Define(Auto, Star, Auto),");
        code.AppendLine("                    ColumnDefinitions = Columns.Define(Star, Star),");
        code.AppendLine("                    Children =");
        code.AppendLine("                    {");
        code.AppendLine("                        new Label()");
        code.AppendLine("                            .Text(\"Themed Page\")");
        code.AppendLine("                            .FontSize(24)");
        code.AppendLine("                            .CenterHorizontal()");
        code.AppendLine("                            .AppThemeBinding(");
        code.AppendLine("                                Label.TextColorProperty,");
        code.AppendLine("                                light: AppResource<Color>(\"Primary\"),");
        code.AppendLine("                                dark: AppResource<Color>(\"PrimaryDark\"))");
        code.AppendLine("                            .Row(0).ColumnSpan(2),");
        code.AppendLine();
        code.AppendLine("                        CreateThemedButton(\"Primary Button\")");
        code.AppendLine("                            .Row(2).Column(0),");
        code.AppendLine();
        code.AppendLine("                        new Button()");
        code.AppendLine("                            .Text(\"Secondary Button\")");
        code.AppendLine("                            .AppThemeBinding(");
        code.AppendLine("                                Button.BackgroundColorProperty,");
        code.AppendLine("                                light: AppResource<Color>(\"Secondary\"),");
        code.AppendLine("                                dark: AppResource<Color>(\"SecondaryDark\"))");
        code.AppendLine("                            .AppThemeBinding(");
        code.AppendLine("                                Button.TextColorProperty,");
        code.AppendLine("                                light: AppResource<Color>(\"OnSecondary\"),");
        code.AppendLine("                                dark: AppResource<Color>(\"OnSecondaryDark\"))");
        code.AppendLine("                            .Row(2).Column(1)");
        code.AppendLine("                    }");
        code.AppendLine("                }.AppThemeBinding(");
        code.AppendLine("                    Grid.BackgroundColorProperty,");
        code.AppendLine("                    light: AppResource<Color>(\"Background\"),");
        code.AppendLine("                    dark: AppResource<Color>(\"BackgroundDark\"));");
        code.AppendLine("            }");
        code.AppendLine("        }");
        code.AppendLine("    }");
        code.AppendLine("}");
        
        return code.ToString();
    }
}
