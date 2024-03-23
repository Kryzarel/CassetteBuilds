using System;
using System.Diagnostics;

namespace CassetteBuilds.Code.Logic
{
	public interface IUrlOpener
	{
		bool OpenUrl(string? url);
	}

	public class DefaultUrlOpener : IUrlOpener
	{
		private readonly ProcessStartInfo processStartInfo = new() { CreateNoWindow = true, UseShellExecute = true };

		public bool OpenUrl(string? url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return false;

			if (OperatingSystem.IsWindows())
			{
				processStartInfo.FileName = url;
				processStartInfo.Arguments = null;
				Process.Start(processStartInfo);
				return true;
			}
			else if (OperatingSystem.IsLinux())
			{
				processStartInfo.FileName = "xdg-open";
				processStartInfo.Arguments = url;
				Process.Start(processStartInfo);
				return true;
			}
			else if (OperatingSystem.IsMacOS())
			{
				processStartInfo.FileName = "open";
				processStartInfo.Arguments = url;
				Process.Start(processStartInfo);
				return true;
			}
			return false;
		}
	}
}