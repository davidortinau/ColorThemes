using Theming;

namespace dotnet_colorthemes;

public partial class App : Application
{	public App()
	{
		InitializeComponent();

		ApplyCurrentTheme();

		if (Application.Current != null)
		    Application.Current.RequestedThemeChanged += OnRequestedThemeChanged;
    }

    void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ApplyCurrentTheme();
    }

    void ApplyCurrentTheme()
    {
        var isDark = Application.Current?.RequestedTheme == AppTheme.Dark;

        // Generate using your base colors (e.g. from config or defaults)
        var theme = ColorThemeGenerator.Generate(
            primary: Color.FromArgb("#F04B25"),
            secondary: Color.FromArgb("#AC3420"),
            background: isDark ? Color.FromArgb("#1C201E") : Color.FromArgb("#FBF8D4"),
            isDarkTheme: isDark);

        // Overwrite all the DynamicResources
        Resources.ApplyTheme(theme);
    }

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}