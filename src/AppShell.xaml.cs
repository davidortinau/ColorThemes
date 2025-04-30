using ColorThemes.PageModels;
using Microsoft.Extensions.DependencyInjection;

namespace ColorThemes;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		
		// Register routes for navigation
		Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
	}
}
