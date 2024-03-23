using CassetteBuilds.Code.Logic;
using Foundation;
using UIKit;

namespace CassetteBuilds.iOS.Code
{
	public class IOSUrlOpener : IUrlOpener
	{
		private static readonly UIApplicationOpenUrlOptions emptyOptions = new();

		public bool OpenUrl(string? url)
		{
			if (string.IsNullOrWhiteSpace(url))
				return false;

			NSUrl uri = new(url);
			if (UIApplication.SharedApplication.CanOpenUrl(uri))
			{
				return UIApplication.SharedApplication.OpenUrlAsync(uri, emptyOptions).Result;
			}
			return false;
		}
	}
}