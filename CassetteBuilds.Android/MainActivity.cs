using System;
using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.ReactiveUI;

namespace CassetteBuilds.Android;

[Activity(
	Label = "Cassette Builds",
	Theme = "@style/MyTheme.NoActionBar",
	Icon = "@drawable/icon",
	MainLauncher = true,
	ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
	protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
	{
		try
		{
			return base.CustomizeAppBuilder(builder)
				.WithInterFont()
				.UseReactiveUI();
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
		return builder;
	}
}
