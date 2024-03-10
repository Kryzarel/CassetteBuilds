using System;
using BenchmarkDotNet.Attributes;
using Cassette_Builds.Code.Core;

namespace Cassette_Builds.Code
{
    [MemoryDiagnoser]
	public class Benchmarks
	{
		// private static readonly string[] moves = new string[] { "Custom Starter", "Critical Mass", "Echolocation", "Mind-Meld" };
		private static readonly string[] moves = new string[] { "Hypnotise", "Mind-Meld", "Sticky Spray" };
		// private static readonly string[] moves = new string[] { "Beast Wall", "Nurse", "Leech", "Doc Leaf" };
		// private static readonly string[] moves = new string[] { "Mind-Meld", "Echolocation", "Magnet" };
		// private static readonly string[] moves = new string[] { "Hypnotise", "Mind-Meld", "Nurse", "Doc Leaf", "Beast Wall" };
		private static readonly int[] moveIndexes;

		static Benchmarks()
		{
			moveIndexes = new int[moves.Length];
			MoveFinder.GetMoveIndexes(moves, moveIndexes);
		}

		public static void PrintTest()
		{
			Span<int> monsterIndexes = stackalloc int[Database.Database.Monsters.Length];
			int count = MonsterFinder.GetMonstersCompatibleWith(moveIndexes, monsterIndexes);
			PrintMonsters(monsterIndexes[..count]);
		}

		public static void PrintMonsters(in ReadOnlySpan<int> monsterIndexes)
		{
			if (monsterIndexes.IsEmpty)
			{
				Console.WriteLine("No monsters found");
				return;
			}

			foreach (int index in monsterIndexes)
			{
				Console.WriteLine(Database.Database.Monsters.Span[index]);
			}
		}

		[Benchmark]
		public int GetMonstersCompatibleWith()
		{
			Span<int> monsterIndexes = stackalloc int[Database.Database.Monsters.Length];
			return MonsterFinder.GetMonstersCompatibleWith(moveIndexes, monsterIndexes);
		}

		// [Benchmark]
		// public int MoveIndexesLookup_Default()
		// {
		// 	ReadOnlySpan<int> indexes = MoveFinder.GetMoveIndexes(moves, moveIndexes);
		// 	return indexes[0];
		// }

		// [Benchmark]
		// public int MoveIndexesLookup_Reverse()
		// {
		// 	ReadOnlySpan<int> indexes = MoveFinder.GetMoveIndexes_ReverseLookup(moves, moveIndexes);
		// 	return indexes[0];
		// }
	}
}