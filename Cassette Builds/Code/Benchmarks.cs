using System;
using BenchmarkDotNet.Attributes;
using Cassette_Builds.Code.Core;

namespace Cassette_Builds.Code
{
	[MemoryDiagnoser]
	public class Benchmarks
	{
		private static readonly string[] moves = new string[] { "Custom Starter", "Critical Mass", "Echolocation", "Mind-Meld" };
		private static readonly int[] moveIndexes;
		private static readonly Random random = new();

		static Benchmarks()
		{
			moveIndexes = new int[moves.Length];
			MoveFinder.GetMoveIndexes(moves, moveIndexes);
		}

		[Benchmark]
		public int GetMonstersCompatibleWith()
		{
			Span<int> monsterIndexes = stackalloc int[Database.Monsters.Length];
			return MonsterFinder.GetMonstersCompatibleWith(moveIndexes, monsterIndexes);
		}

		[Benchmark]
		public int AverageSpeed()
		{
			Span<int> monsterIndexes = stackalloc int[Database.Monsters.Length];
			Span<int> moveIndexes = stackalloc int[random.Next(8)];
			for (int i = 0; i < moveIndexes.Length; i++)
			{
				moveIndexes[i] = random.Next(Database.Moves.Length);
			}
			return MonsterFinder.GetMonstersCompatibleWith(moveIndexes, monsterIndexes);
		}
	}
}