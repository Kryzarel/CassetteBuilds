using System;
using System.Diagnostics;

namespace CassetteBuilds.Code.Misc
{
	public static class PlatformHelpers
	{
		private static ProcessStartInfo? _processStartInfo;
		private static ProcessStartInfo processStartInfo => _processStartInfo ??= new() { CreateNoWindow = true, UseShellExecute = true };

		public static void OpenBrowser(string? url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return;

			if (OperatingSystem.IsWindows())
			{
				processStartInfo.FileName = url;
				processStartInfo.Arguments = null;
				Process.Start(processStartInfo);
			}
			else if (OperatingSystem.IsLinux())
			{
				processStartInfo.FileName = "xdg-open";
				processStartInfo.Arguments = url;
				Process.Start(processStartInfo);
			}
			else if (OperatingSystem.IsMacOS())
			{
				processStartInfo.FileName = "open";
				processStartInfo.Arguments = url;
				Process.Start(processStartInfo);
			}
			else
			{
				// TODO: Implement for other platforms. Probably use Avalonia v11.1.0 beta with it's ILauncher service
			}
		}

		public static bool SupportsBrowser()
		{
			return OperatingSystem.IsWindows() || OperatingSystem.IsLinux() || OperatingSystem.IsMacOS();
		}
	}
}