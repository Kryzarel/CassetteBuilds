using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Cassette_Builds
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

		public static async Task<string> DownloadAndSaveText(string url, string path)
		{
			string html = await Client.GetStringAsync(url);
			await SaveText(path, html);
			return html;
		}

		public static async Task<string> ReadFileOrDownload(string url, string path)
		{
			return File.Exists(path) ? await File.ReadAllTextAsync(path) : await DownloadAndSaveText(url, path);
		}

		public static async Task DownloadAndSaveFiles(string url, string[] links, string directory, string[] fileNames, string extension)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			Task[] tasks = new Task[links.Length];

			for (int i = 0; i < links.Length; i++)
			{
				Task<byte[]> downloadTask = Client.GetByteArrayAsync(url + links[i]);
				tasks[i] = DownloadAndSaveFile(downloadTask, directory, fileNames[i], extension);
			}
			await Task.WhenAll(tasks);
		}

		private static async Task DownloadAndSaveFile(Task<byte[]> downloadTask, string directory, string fileName, string extension)
		{
			if (!extension.StartsWith('.'))
				extension += '.';

			string fullPath = Path.Combine(directory, fileName) + extension;
			byte[] bytes = await downloadTask;
			await File.WriteAllBytesAsync(fullPath, bytes);
		}
	}
}