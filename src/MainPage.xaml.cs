using Theming;
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
    private ColorTheme? _currentTheme;
    
    public MainPage(MainPageModel viewModel)
    {
        InitializeComponent();
        
        _viewModel = viewModel;
        BindingContext = _viewModel;
        
        // Initialize the color format picker
        CodeFormatPicker.ItemsSource = new List<string>
        {
            "AppThemeColor",
            "AppThemeBinding",
            "DynamicResource",
            "MauiReactor"
        };
        CodeFormatPicker.SelectedIndex = 0;
        
        // Add a handler to update the color hex values when the app theme changes
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += (s, e) => UpdateColorSwatches();
            
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
                Application.Current.UserAppTheme = AppTheme.Light;
        }
    }    
    
    private async void GenerateXamlButton_Clicked(object sender, EventArgs e)
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

            string templateFileName = format switch
            {
                CodeGenerationFormat.AppThemeColor => "AppThemeColor.scriban-txt",
                CodeGenerationFormat.AppThemeBinding => "AppThemeBinding.scriban-txt",
                CodeGenerationFormat.DynamicResource => "DynamicResource.scriban-txt",
                CodeGenerationFormat.MauiReactor => "MauiReactor.scriban-txt",
                CodeGenerationFormat.CSharpMarkup => "CSharpMarkup.scriban-txt",
                _ => throw new NotSupportedException($"The format {format} is not supported.")
            };

            // load and parse scriban template
            var generatedCode = string.Empty;     
            using Stream templateStream = await FileSystem.OpenAppPackageFileAsync(templateFileName);
            using (StreamReader reader = new StreamReader(templateStream))
            {
                var template = Template.Parse(await reader.ReadToEndAsync() );
                generatedCode = await template.RenderAsync(new
                {
                    primary = _currentTheme.Primary.ToArgbHex(),
                    primary_dark = _currentTheme.PrimaryDark.ToArgbHex(),
                    secondary = _currentTheme.Secondary.ToArgbHex(),
                    secondary_dark = _currentTheme.SecondaryDark.ToArgbHex(),
                    background = _currentTheme.Background.ToArgbHex(),
                    background_dark = _currentTheme.BackgroundDark.ToArgbHex(),
                    on_background = _currentTheme.OnBackground.ToArgbHex(),
                    on_background_dark = _currentTheme.OnBackgroundDark.ToArgbHex(),
                    surface_0 = _currentTheme.Surface0.ToArgbHex(),
                    surface_0_dark = _currentTheme.Surface0Dark.ToArgbHex(),
                    surface_1 = _currentTheme.Surface1.ToArgbHex(),
                    surface_1_dark = _currentTheme.Surface1Dark.ToArgbHex(),
                    surface_2 = _currentTheme.Surface2.ToArgbHex(),
                    surface_2_dark = _currentTheme.Surface2Dark.ToArgbHex(),
                    surface_3 = _currentTheme.Surface3.ToArgbHex(),
                    surface_3_dark = _currentTheme.Surface3Dark.ToArgbHex(),
                    include_styles = IncludeStylesCheckBox.IsChecked
                }); 

                // //Debug.WriteLine(prompt);
            }
                        
            // Display in the editor
            ThemeXamlEditor.Text = generatedCode;
            
            // Enable save button
            SaveXamlButton.IsEnabled = true;
        }
    }

    private async void SaveXamlButton_Clicked(object sender, EventArgs e)
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

    private async void CopyButton_Clicked(object sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(ThemeXamlEditor.Text);
        await App.DisplayToastAsync("Copied to clipboard!");
    }
}
