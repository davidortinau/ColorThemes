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
        
        // Apply default theme
        _themeService.ApplyThemeColors(
            "#F04B25", "#AC3420", "#FBF8D4", "#1C201E");
        
        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
            
        // Load themes in the background
        LoadThemesAsync();
    }
    
    private async void LoadThemesAsync()
    {
        try
        {
            await _themeService.LoadThemesAsync();
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