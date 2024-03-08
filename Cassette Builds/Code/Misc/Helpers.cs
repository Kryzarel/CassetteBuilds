using System;
using System.Collections.Generic;
using Cassette_Builds.Code.Database;

namespace Cassette_Builds.Code.Misc
{
	public static class Helpers
	{
		public static void Print<T>(IList<T> list, string? title = null)
		{
			if (!string.IsNullOrEmpty(title))
				Console.WriteLine(title);

			for (int i = 0; i < list.Count; i++)
			{
				Console.WriteLine(list[i]);
			}
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