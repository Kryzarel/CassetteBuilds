using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CassetteBuilds.Code.Admin
{
	public static class DataUpdater
	{
		public static readonly string AssetsPath = Path.Combine(GetProjectPath(), "..", "CassetteBuilds", "Assets");
		public static readonly string DataPath = Path.Combine(AssetsPath, "Data");
		public static readonly string ImagesPath = Path.Combine(AssetsPath, "Images");
		public static readonly string WikiPagesPath = Path.Combine(GetProjectPath(), "Wiki Pages Cache");

		public const string WebsiteUrl = "https://wiki.cassettebeasts.com";
		public const int MaxConcurrentDownloads = 25;

		private static string GetProjectPath()
		{
			ReadOnlySpan<char> assembly = typeof(DataUpdater).Assembly.GetName().Name;
			ReadOnlySpan<char> directory = AppContext.BaseDirectory;
			return directory[..(directory.LastIndexOf(assembly, StringComparison.Ordinal) + assembly.Length)].ToString();
		}

		public static async Task<Exception?> UpdateAll(bool clearCache = false)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();
			Console.WriteLine("Update Started");
			try
			{
				if (clearCache)
				{
					if (Directory.Exists(WikiPagesPath))
						Directory.Delete(WikiPagesPath, recursive: true);

					if (Directory.Exists(ImagesPath))
						Directory.Delete(ImagesPath, recursive: true);
				}
				Directory.CreateDirectory(DataPath);
				await Task.WhenAll(UpdateMonsters(stopwatch), UpdateMoves(stopwatch));
			}
			catch (Exception e)
			{
				Console.WriteLine("Update failed with error: " + e);
				return e;
			}
			stopwatch.Stop();
			Console.WriteLine($"Update successful. Time elasped: {stopwatch.Elapsed.TotalSeconds}s");
			return null;
		}

		public static async Task UpdateMonsters(Stopwatch stopwatch)
		{
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Updating Monsters");
			string speciesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Species", $"{WikiPagesPath}/Species.html");
			using FileStream stream = File.Create($"{DataPath}/Monsters.csv");
			using StreamWriter writer = new(stream);
			SpeciesHtmlParser.Result result = SpeciesHtmlParser.Parse(speciesHtml, WebsiteUrl, writer);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Monsters...Done");

			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Updating Monster Moves and Images");
			await ParseMonsters(result.MonsterNamesAndLinks);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Monster Moves and Images...Done");

			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Updating Types and Images");
			await ParseTypes(result.TypeNamesAndLinks);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Types...Done");
		}

		private static async Task ParseMonsters(IReadOnlyList<(string, string)> namesAndLinks)
		{
			string imagesPath = ImagesPath + "/Monsters";
			Task<StringWriter>[] tasks = new Task<StringWriter>[Math.Min(MaxConcurrentDownloads, namesAndLinks.Count)];

			using FileStream stream = File.Create($"{DataPath}/MovesPerMonster.csv");
			using StreamWriter writer = new(stream);
			writer.Write("Monster,Move");

			int j = 0;
			for (int i = 0; i < namesAndLinks.Count; i++, j = i % tasks.Length)
			{
				(string monsterName, string link) = namesAndLinks[i];
				tasks[j] = ParseMonster(monsterName, link, imagesPath);
				if (j + 1 >= tasks.Length)
				{
					await Task.WhenAll(tasks); // Wait for the current batch of tasks
					foreach (Task<StringWriter> task in tasks)
					{
						writer.Write(task.Result.GetStringBuilder());
					}
				}
			}

			await Task.WhenAll(tasks); // Wait for any remaining tasks (it's fine to await the same task multiple times)
			for (int i = 0; i <= j; i++) // Make sure to write only until "j", don't write duplicate info
			{
				writer.Write(tasks[i].Result.GetStringBuilder());
			}
		}

		private static async Task<StringWriter> ParseMonster(string name, string url, string directory)
		{
			StringWriter writer = new(new StringBuilder(3000));
			string monsterHtml = await Downloader.ReadFileOrDownload(url, $"{WikiPagesPath}/Monsters/{name}.html");
			string imageLink = MonsterHtmlParser.Parse(monsterHtml, WebsiteUrl, name, writer);
			string imagePath = Path.ChangeExtension(Path.Combine(directory, name), Path.GetExtension(imageLink));
			if (!File.Exists(imagePath))
			{
				Directory.CreateDirectory(directory);
				await Downloader.DownloadAndSaveFile(imageLink, imagePath);
			}
			return writer;
		}

		private static async Task ParseTypes(IReadOnlyList<(string, string)> namesAndLinks)
		{
			string imagesPath = ImagesPath + "/Types";
			Task[] tasks = new Task[namesAndLinks.Count];

			for (int i = 0; i < namesAndLinks.Count; i++)
			{
				(string typeName, string link) = namesAndLinks[i];
				tasks[i] = ParseType(typeName, link, imagesPath);
			}

			await Task.WhenAll(tasks); // Wait for any remaining tasks (it's fine to await the same task multiple times)
		}

		private static async Task ParseType(string name, string url, string directory)
		{
			string typeHtml = await Downloader.ReadFileOrDownload(url, $"{WikiPagesPath}/Types/{name}.html");
			string imageLink = TypeHtmlParser.Parse(typeHtml, WebsiteUrl);
			string imagePath = Path.ChangeExtension(Path.Combine(directory, name), Path.GetExtension(imageLink));
			if (!File.Exists(imagePath))
			{
				Directory.CreateDirectory(directory);
				await Downloader.DownloadAndSaveFile(imageLink, imagePath);
			}
		}

		public static async Task UpdateMoves(Stopwatch stopwatch)
		{
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Updating Moves");
			string movesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Moves", $"{WikiPagesPath}/Moves.html");
			using FileStream stream = File.Create($"{DataPath}/Moves.csv");
			using StreamWriter writer = new(stream);
			MovesHtmlParser.Parse(movesHtml, WebsiteUrl, writer);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Moves...Done");
		}
	}
}