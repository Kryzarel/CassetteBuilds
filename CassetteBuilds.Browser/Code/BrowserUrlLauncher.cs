using System.Runtime.InteropServices.JavaScript;
using System.Runtime.Versioning;
using CassetteBuilds.Code.Logic;

namespace CassetteBuilds.Browser.Code
{
	[SupportedOSPlatform("browser")]
	public static partial class JSInterop
	{
		[JSImport("globalThis.window.open")]
		public static partial JSObject? WindowOpen(string uri, string target);
	}

	public class BrowserUrlLauncher : IUrlOpener
	{
		public bool OpenUrl(string? url)
		{
			return !string.IsNullOrWhiteSpace(url) && JSInterop.WindowOpen(url, "_blank") is not null;
		}
	}
}