using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cassette_Builds.Code.Admin
{
	public static class DataUpdater
	{
		public const string WebsiteUrl = "https://wiki.cassettebeasts.com";
		public const int MaxConcurrentDownloads = 25;

		public static async Task<Exception?> UpdateAll(bool clearCache = false)
		{
			try
			{
				if (clearCache)
				{
					if (Directory.Exists("Wiki Pages"))
						Directory.Delete("Wiki Pages", recursive: true);

					if (Directory.Exists("Images"))
						Directory.Delete("Images", recursive: true);
				}
				await Task.WhenAll(UpdateMonsters(), UpdateMoves());
			}
			catch (Exception e)
			{
				Console.WriteLine("Update failed with error: " + e);
				return e;
			}
			Console.WriteLine("Update successful");
			return null;
		}

		public static async Task UpdateMonsters()
		{
			string speciesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Species", "Wiki Pages/Species.html");
			List<Monster> monsters = SpeciesHtmlParser.Parse(speciesHtml, WebsiteUrl);

			// Manually set Magikrab's image that is hidden under "SPOILER" in the species list
			// TODO: Change this to get the image from the actual monster page?
			monsters[0] = monsters[0] with { ImageLink = "/images/d/d8/Magikrab.png" };

			Task t1 = DataSerializer.SerializeToCsv("Data/Monsters.csv", monsters);
			Task t2 = DownloadAndSaveImages(monsters, WebsiteUrl, "Images", ".png");
			await Task.WhenAll(t1, t2);
		}

		private static async Task DownloadAndSaveImages(IList<Monster> monsters, string url, string directory, string extension)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			Task[] tasks = new Task[Math.Min(MaxConcurrentDownloads, monsters.Count)];

			for (int i = 0, j = 0; i < monsters.Count; i++, j = i % tasks.Length)
			{
				Monster monster = monsters[i];
				tasks[j] = Downloader.DownloadAndSaveFile(url + monster.ImageLink, directory, monster.Name, extension);
				if (j + 1 >= tasks.Length)
				{
					await Task.WhenAll(tasks); // Wait for the current batch of tasks
				}
			}
			await Task.WhenAll(tasks); // Wait for any remaining tasks (it's fine to await the same tasks multiple times)
		}

		public static async Task UpdateMoves()
		{
			string movesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Moves", "Wiki Pages/Moves.html");
			List<Move> moves = MovesHtmlParser.Parse(movesHtml, WebsiteUrl);
			Task t1 = DataSerializer.SerializeToCsv("Data/Moves.csv", moves);
			Task t2 = UpdateMoveMonsterPairs(moves);
			await Task.WhenAll(t1, t2);
		}

		public static async Task UpdateMoveMonsterPairs(IList<Move> moves)
		{
			Task<string>[] allTasks = new Task<string>[moves.Count];
			Task<string>[] tasks = new Task<string>[Math.Min(MaxConcurrentDownloads, moves.Count)];
			System.Diagnostics.Stopwatch stopwatch = new();
			stopwatch.Restart();

			for (int i = 0, j = 0; i < moves.Count; i++, j = i % tasks.Length)
			{
				Move move = moves[i];
				tasks[j] = allTasks[i] = Downloader.ReadFileOrDownload(move.Link, $"Wiki Pages/Moves/{move.Name}.html");
				if (j + 1 >= tasks.Length)
				{
					await Task.WhenAll(tasks); // Wait for the current batch of tasks
					Console.WriteLine($"Batch {i / tasks.Length} done");
				}
			}
			await Task.WhenAll(allTasks); // Wait for any remaining tasks (it's fine to await the same tasks multiple times)
			stopwatch.Stop();
			Console.WriteLine($"Elapsed: {stopwatch.Elapsed}");

			// Parse synchronously to avoid data corruption since List<T> is not thread-safe
			List<MoveMonsterPair> movesPerMonster = new(15_000);
			for (int i = 0; i < moves.Count; i++)
			{
				MovesPerMonsterHtmlParser.Parse(allTasks[i].Result, moves[i].Name, movesPerMonster);
			}
			movesPerMonster.Sort(Compare);
			await DataSerializer.SerializeToCsv("Data/MovesPerMonster.csv", movesPerMonster);
		}

		private static int Compare(MoveMonsterPair a, MoveMonsterPair b)
		{
			int result = a.Move.CompareTo(b.Move);
			if (result != 0) return result;
			return a.Monster.CompareTo(b.Monster);
		}
	}
}