using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Platform;
using CassetteBuilds.Code.Logic;

namespace CassetteBuilds.Code.Misc
{
    public static class Helpers
	{
		public static void Print<T, R>(in ReadOnlySpan<T> span, Func<T, R>? select = null, string? title = null, string? messageWhenEmpty = null)
		{
			if (!string.IsNullOrEmpty(title))
				Console.WriteLine(title);
			if (span.IsEmpty && !string.IsNullOrEmpty(messageWhenEmpty))
				Console.WriteLine(messageWhenEmpty);

			foreach (T item in span)
			{
				Console.WriteLine(select != null ? select(item) : item);
			}
		}

		public static void PrintSingleLine<T>(in ReadOnlySpan<T> span, string? title = null, string? messageWhenEmpty = null)
		{
			if (!string.IsNullOrEmpty(title))
				Console.WriteLine(title);

			if (span.IsEmpty)
			{
				if (!string.IsNullOrEmpty(messageWhenEmpty))
					Console.WriteLine(messageWhenEmpty);
				return;
			}

			Console.Write(span[0]);
			for (int i = 1; i < span.Length; i++)
			{
				Console.Write(", ");
				Console.Write(span[i]);
			}
			Console.WriteLine();
		}

		public static void PrintMovePagesWithMissingMonsters()
		{
			using TextReader monsterMovesReader = new StreamReader(AssetLoader.Open(new Uri("/Assets/Data/MovesPerMonster.csv")));
			using TextReader monsterMovesOldReader = new StreamReader(AssetLoader.Open(new Uri("/Assets/Data/MovesPerMonster_Old.csv")));

			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs(monsterMovesReader);
			MoveMonsterPair[] movesPerMonsterOld = DataDeserializer.DeserializeMoveMonsterPairsOld(monsterMovesOldReader);

			HashSet<MoveMonsterPair> missing = new(movesPerMonster);
			missing.ExceptWith(movesPerMonsterOld);

			Console.WriteLine("Moves with missing Monsters");
			foreach (MoveMonsterPair item in missing)
			{
				Console.WriteLine(item);
			}
			Console.WriteLine();
		}
	}
}