using ColorThemes.Services;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace ColorThemes;

public partial class App : Application
{
    private readonly ThemeService _themeService;
      public App(ThemeService themeService)
    {
        _themeService = themeService;
        
        InitializeComponent();
        
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
            
        // Load themes in the background and apply the first theme
        LoadThemesAsync();
    }
      private async void LoadThemesAsync()
    {
        try
        {
            await _themeService.LoadThemesAsync();
            
            // Apply the first theme after loading
            if (_themeService.AvailableThemes.Count > 0)
            {
                var firstTheme = _themeService.AvailableThemes.First();
                await _themeService.ApplyThemeByTitleAsync(firstTheme.Title);
            }
        }
        catch
        {
            await DisplayToastAsync("Failed to load themes");
        }
    }
    
    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        _themeService.HandleAppThemeChanged();
    }
    
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
    
    public static Task DisplayToastAsync(string message)
    {
        ToastDuration duration = ToastDuration.Long;
        double fontSize = 14;
        var toast = Toast.Make(message, duration, fontSize);
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        return toast.Show(cancellationTokenSource.Token);
    }
}