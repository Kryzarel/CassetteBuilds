using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Cassette_Builds
{
	public static class DataUpdater
	{
		public const string WebsiteUrl = "https://wiki.cassettebeasts.com";

		public static async Task<Exception?> UpdateAll(bool clearCache = false)
		{
			try
			{
				if (clearCache && Directory.Exists("Wiki Pages"))
				{
					Directory.Delete("Wiki Pages", recursive: true);
				}
				await Task.WhenAll(UpdateMonsters(), UpdateMoves());
			}
			catch (Exception e)
			{
				Console.WriteLine("Update failed with error: " + e);
				return e;
			}
			return null;
		}

		public static async Task UpdateMonsters()
		{
			string speciesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Species", "Wiki Pages/Species.html");
			Monster[] monsters = SpeciesHtmlParser.Parse(speciesHtml, WebsiteUrl, out string[] imageLinks, out string[] imageNames);
			Task t1 = DataSerializer.SerializeToCsv("Data/Monsters.csv", monsters);
			Task t2 = Downloader.DownloadAndSaveFiles(WebsiteUrl, imageLinks, "Images", imageNames, ".png");
			await Task.WhenAll(t1, t2);
		}

		public static async Task UpdateMoves()
		{
			string movesHtml = await Downloader.ReadFileOrDownload($"{WebsiteUrl}/wiki/Moves", "Wiki Pages/Moves.html");
			Move[] moves = MovesHtmlParser.Parse(movesHtml, WebsiteUrl);
			Task t1 = DataSerializer.SerializeToCsv("Data/Moves.csv", moves);
			Task t2 = UpdateMoveMonsterPairs(moves);
			await Task.WhenAll(t1, t2);
		}

		public static async Task UpdateMoveMonsterPairs(Move[] moves)
		{
			List<MoveMonsterPair> moveMonsterPairs = new(140 * 280);

			Task<string>[] tasks = new Task<string>[moves.Length];
			for (int i = 0; i < moves.Length; i++)
			{
				Move move = moves[i];
				tasks[i] = Downloader.ReadFileOrDownload(move.Link, $"Wiki Pages/Moves/{move.Name}.html");
			}
			await Task.WhenAll(tasks);

			for (int i = 0; i < moves.Length; i++)
			{
				MovesPerMonsterHtmlParser.Parse(tasks[i].Result, moves[i].Name, moveMonsterPairs);
			}

			moveMonsterPairs.Sort(Compare);
			MoveMonsterPair[] movesPerMonster = moveMonsterPairs.ToArray();
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