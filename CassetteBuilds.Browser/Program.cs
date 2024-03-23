using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Avalonia.ReactiveUI;
using CassetteBuilds;
using CassetteBuilds.Browser.Code;
using CassetteBuilds.Code.Logic;

[assembly: SupportedOSPlatform("browser")]

internal sealed partial class Program
{
	private static Task Main(string[] args)
	{
		try
		{
			Features.UrlOpener = new BrowserUrlLauncher();

			return BuildAvaloniaApp()
				.WithInterFont()
				.UseReactiveUI()
				.StartBrowserAppAsync("out");
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		return Task.CompletedTask;
	}

	public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>();
}