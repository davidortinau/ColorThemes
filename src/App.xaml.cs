using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Theming;

namespace dotnet_colorthemes;

public partial class App : Application
{
    public static string Primary = "#F04B25";
    public static string Secondary = "#AC3420";
    public static string LightBackground = "#FBF8D4";
    public static string DarkBackground = "#1C201E";
    public App()
    {
        InitializeComponent();

        ApplyCurrentTheme();

        if (Application.Current != null)
            Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyCurrentTheme(Primary, Secondary, LightBackground, DarkBackground);
    }

    public static void ApplyCurrentTheme(string primary = "#F04B25", string secondary = "#AC3420", string lightBackground = "#FBF8D4", string darkBackground = "#1C201E")
    {
        // save theme colors to static variables
        Primary = primary;
        Secondary = secondary;
        LightBackground = lightBackground;
        DarkBackground = darkBackground;

        // Check if the app is in dark mode
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        // Generate using your base colors (e.g. from config or defaults)
        var theme = ColorThemeGenerator.Generate(
            primary: Color.FromArgb(primary),
            secondary: Color.FromArgb(secondary),
            background: isDark ? Color.FromArgb(darkBackground) : Color.FromArgb(lightBackground),
            isDarkTheme: isDark);

        // Overwrite all the DynamicResources
        App.Current?.Resources.ApplyTheme(theme);
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