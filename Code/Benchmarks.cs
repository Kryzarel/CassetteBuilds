using BenchmarkDotNet.Attributes;
using Cassette_Builds.Code.Database;

namespace Cassette_Builds.Code
{
	[MemoryDiagnoser]
	public class Benchmarks
	{
		public static readonly Monster[] Monsters = DataDeserializer.DeserializeMonsters("Data/Monsters.csv");
		public static readonly Move[] Moves = DataDeserializer.DeserializeMoves("Data/Moves.csv");
		public static readonly MoveMonsterPair[] MoveMonsterPairs = DataDeserializer.DeserializeMoveMonsterPairs("Data/MovesPerMonster.csv");

		static Benchmarks()
		{
			_ = Database.Database.Moves;
		}

		[Benchmark]
		public Monster[] MonstersBench()
		{
			return DataDeserializer.DeserializeMonsters("Data/Monsters.csv");
		}

		[Benchmark]
		public Move[] MovesBench()
		{
			return DataDeserializer.DeserializeMoves("Data/Moves.csv");
		}

		[Benchmark]
		public MoveMonsterPair[] MovesPerMonsterBench()
		{
			return DataDeserializer.DeserializeMoveMonsterPairs("Data/MovesPerMonster.csv");
		}

		[Benchmark]
		public bool[,] MonsterMoves2DBench()
		{
			return Database.Database.ComputeMonsterMoves(MoveMonsterPairs, Monsters, Moves);
		}
	}
}