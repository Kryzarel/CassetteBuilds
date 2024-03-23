using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CassetteBuilds.Code.Logic
{
	public static class TypeImageDatabase
	{
		private static readonly Dictionary<string, Bitmap> images = new(20);

		public static Bitmap GetImage(string type)
		{
			if (!images.TryGetValue(type, out Bitmap? image))
			{
				using Stream stream = AssetLoader.Open(new Uri($"avares://CassetteBuilds/Assets/Images/Types/{type}.png"));
				images[type] = image = new Bitmap(stream);
			}
			return image;
		}
	}
}