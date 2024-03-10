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
	}
}