using Android.Content;
using Android.Content.PM;
using CassetteBuilds.Code.Logic;
using Android.Net;

namespace CassetteBuilds.Android.Code
{
	public class AndroidUrlOpener : IUrlOpener
	{
		private readonly Context context;

		public AndroidUrlOpener(Context context)
		{
			this.context = context;
		}

		public bool OpenUrl(string? url)
		{
			if (!string.IsNullOrWhiteSpace(url) && context.PackageManager is PackageManager packageManager)
			{
				Intent intent = new(Intent.ActionView, Uri.Parse(url));
				if (intent.ResolveActivity(packageManager) is not null)
				{
					intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask);
					context.StartActivity(intent);
					return true;
				}
			}
			return false;
		}
	}
}