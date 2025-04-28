using Theming;
using Microsoft.Maui.Graphics;
using System.IO;

namespace dotnet_colorthemes;

public partial class MainPage : ContentPage
{
    private ColorTheme? _currentTheme;

    public MainPage()
    {
        InitializeComponent();
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
            _currentTheme = Theming.ColorThemeGenerator.Generate(primary, secondary, background, isDarkTheme);
            
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
#pragma warning disable CS0618
                await DisplayAlert("Success", "XAML file saved successfully", "OK");
#pragma warning restore CS0618
            }
#else
            // On other platforms, use a predefined path
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string fileName = "ThemeColors.xaml";
            string filePath = Path.Combine(documentsPath, fileName);
            
            await File.WriteAllTextAsync(filePath, ThemeXamlEditor.Text);
#pragma warning disable CS0618
            await DisplayAlert("Success", $"XAML file saved to {filePath}", "OK");
#pragma warning restore CS0618
#endif
        }
        catch (Exception ex)
        {
#pragma warning disable CS0618
            await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
#pragma warning restore CS0618
        }
    }
}
