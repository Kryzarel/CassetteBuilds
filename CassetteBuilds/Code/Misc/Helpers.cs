using System;
using System.Collections.Generic;
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
			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs("Data/MovesPerMonster.csv");
			MoveMonsterPair[] movesPerMonsterOld = DataDeserializer.DeserializeMoveMonsterPairsOld("Data/MovesPerMonster_Old.csv");

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