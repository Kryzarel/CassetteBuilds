using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cassette_Builds.Code.Admin
{
	public static class Downloader
	{
		public static readonly HttpClient Client = new();

		public static async Task SaveText(string path, string html)
		{
			string? directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory!);
			}
			await File.WriteAllTextAsync(path, html);
		}

		/// <summary>
		/// For some reason downloading multiple HTMLs from the wiki in parallel sometimes causes it to throw an HTTP error.
		/// This version try-catches errors and re-attempts the download after a delay when that happens.
		/// </summary>
		public static async Task<string> DownloadAndSaveText(string url, string path, int maxRetries)
		{
			int retries = 0;
			const int retryDelay = 100; // Delay in milliseconds to wait before retrying

			while (retries < maxRetries)
			{
				try
				{
					return await DownloadAndSaveText(url, path);
				}
				catch (Exception e)
				{
					retries++;
					Console.WriteLine($"Failed to download {url} with error: {e}\nRetrying in {retryDelay}ms");
					await Task.Delay(retryDelay);
				}
			}
			return string.Empty;
		}

		private static async Task<string> DownloadAndSaveText(string url, string path)
		{
			string html = await Client.GetStringAsync(url);
			await SaveText(path, html);
			return html;
		}

		public static async Task<string> ReadFileOrDownload(string url, string path)
		{
			return File.Exists(path) ? await File.ReadAllTextAsync(path) : await DownloadAndSaveText(url, path, maxRetries: 5);
		}

		public static async Task<byte[]> DownloadAndSaveFile(string url, string path)
		{
			byte[] bytes = await Client.GetByteArrayAsync(url);
			await File.WriteAllBytesAsync(path, bytes);
			return bytes;
		}
	}
}