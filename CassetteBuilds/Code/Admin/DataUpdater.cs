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
		public const string WebsiteUrl = "https://wiki.cassettebeasts.com";
		public const int MaxConcurrentDownloads = 25;

		public static async Task<Exception?> UpdateAll(bool clearCache = false)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();
			Console.WriteLine("Update Started");
			try
			{
				if (clearCache)
				{
					if (Directory.Exists("Wiki Pages"))
						Directory.Delete("Wiki Pages", recursive: true);

					if (Directory.Exists("Images"))
						Directory.Delete("Images", recursive: true);
				}
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
			string speciesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Species", "Wiki Pages/Species.html");
			using FileStream stream = File.Create("Data/Monsters.csv");
			using StreamWriter writer = new(stream);
			List<(string, string)> namesAndLinks = SpeciesHtmlParser.Parse(speciesHtml, WebsiteUrl, writer);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Monsters...Done");

			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Updating Monster Moves and Images");
			await ParseMonsters(namesAndLinks);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Monster Moves and Images...Done");
		}

		private static async Task ParseMonsters(List<(string, string)> namesAndLinks)
		{
			Task<StringWriter>[] tasks = new Task<StringWriter>[Math.Min(MaxConcurrentDownloads, namesAndLinks.Count)];

			using FileStream stream = File.Create("Data/MovesPerMonster.csv");
			using StreamWriter writer = new(stream);
			writer.Write("Monster,Move");

			int j = 0;
			for (int i = 0; i < namesAndLinks.Count; i++, j = i % tasks.Length)
			{
				(string monsterName, string link) = namesAndLinks[i];
				tasks[j] = ParseMonster(link, monsterName, "Images");
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

		private static async Task<StringWriter> ParseMonster(string url, string monsterName, string directory)
		{
			StringWriter writer = new(new StringBuilder(3000));
			string monsterHtml = await Downloader.ReadFileOrDownload(url, $"Wiki Pages/Monsters/{monsterName}.html");
			string imageLink = MonsterHtmlParser.Parse(monsterHtml, WebsiteUrl, monsterName, writer);
			string imagePath = Path.ChangeExtension(Path.Combine(directory, monsterName), ".png");
			if (!File.Exists(imagePath))
			{
				Directory.CreateDirectory(directory);
				await Downloader.DownloadAndSaveFile(imageLink, imagePath);
			}
			return writer;
		}

		public static async Task UpdateMoves(Stopwatch stopwatch)
		{
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Updating Moves");
			string movesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Moves", "Wiki Pages/Moves.html");
			using FileStream stream = File.Create("Data/Moves.csv");
			using StreamWriter writer = new(stream);
			MovesHtmlParser.Parse(movesHtml, WebsiteUrl, writer);
			Console.WriteLine($"{stopwatch.Elapsed.TotalSeconds}) Moves...Done");
		}
	}
}