using System;
using System.Collections.Generic;
using System.IO;
using CassetteBuilds.Code.Data;

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
			string baseDir = AppContext.BaseDirectory;
			string movesPerMonsterPath = Path.Combine(baseDir, "Data", "MovesPerMonster.csv");
			string movesPerMonsterOldPath = Path.Combine(baseDir, "Data", "MovesPerMonster_Old.csv");

			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs(movesPerMonsterPath);
			MoveMonsterPair[] movesPerMonsterOld = DataDeserializer.DeserializeMoveMonsterPairsOld(movesPerMonsterOldPath);

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