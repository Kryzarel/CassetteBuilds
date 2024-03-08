using System;
using BenchmarkDotNet.Attributes;
using Cassette_Builds.Code.Core;
using Kryz.Collections;

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
			_ = Database.Database.Moves;
			moveIndexes = new int[moves.Length];
			MoveFinder.GetMoveIndexes(moves, moveIndexes);
		}

		public static void PrintTest()
		{
			using ReadOnlyNonAllocBuffer<int> monsterIndexes = MonsterFinder.GetMonstersCompatibleWith(moveIndexes);
			PrintMonsters(monsterIndexes);
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
				Console.WriteLine(Database.Database.Monsters[index]);
			}
		}

		[Benchmark]
		public void Compatible()
		{
			using ReadOnlyNonAllocBuffer<int> monsterIndexes = MonsterFinder.GetMonstersCompatibleWith(moveIndexes);
		}
	}
}