using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CassetteBuilds.Code.Data;

namespace CassetteBuilds.Code.Core
{
	public static class Database
	{
		public static readonly IReadOnlyList<Monster> Monsters;
		public static readonly IReadOnlyList<Move> Moves;
		public static readonly IReadOnlyList<bool> MonsterMoves;
		public static readonly ReadOnlyDictionary<string, int> MovesReverseLookup;

		public static readonly ReadOnlyMemory<Monster> MonstersMem;
		public static readonly ReadOnlyMemory<Move> MovesMem;
		public static readonly ReadOnlyMemory<bool> MonsterMovesMem;

		static Database()
		{
			string baseDir = AppContext.BaseDirectory;
			string monstersPath = Path.Combine(baseDir, "Data", "Monsters.csv");
			string movesPath = Path.Combine(baseDir, "Data", "Moves.csv");
			string movesPerMonsterPath = Path.Combine(baseDir, "Data", "MovesPerMonster.csv");

			Monster[] monsters = DataDeserializer.DeserializeMonsters(monstersPath);
			Move[] moves = DataDeserializer.DeserializeMoves(movesPath);
			MovesReverseLookup = ComputeMovesReverseLookup(moves);
			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs(movesPerMonsterPath);
			bool[] monsterMoves = ComputeMonsterMoves(movesPerMonster, monsters, moves);

			Monsters = monsters; MonstersMem = monsters;
			Moves = moves; MovesMem = moves;
			MonsterMoves = monsterMoves; MonsterMovesMem = monsterMoves;
		}

		public static ReadOnlyDictionary<string, int> ComputeMovesReverseLookup(in ReadOnlySpan<Move> moves)
		{
			Dictionary<string, int> reverseLookup = new(moves.Length);
			for (int i = 0; i < moves.Length; i++)
			{
				reverseLookup[moves[i].Name] = i;
			}
			return reverseLookup.AsReadOnly();
		}

		public static bool[] ComputeMonsterMoves(in ReadOnlySpan<MoveMonsterPair> movesPerMonster, in ReadOnlySpan<Monster> monsters, in ReadOnlySpan<Move> moves)
		{
			bool[] monsterMoves = new bool[monsters.Length * moves.Length];

			string lastMonster = "", lastMove = "";
			int monsterIndex = -1, moveIndex = -1;
			foreach (MoveMonsterPair item in movesPerMonster)
			{
				if (lastMonster != item.Monster)
				{
					lastMonster = item.Monster;
					monsterIndex = monsters.FindIndexByName(item.Monster);
				}
				if (lastMove != item.Move)
				{
					lastMove = item.Move;
					moveIndex = moves.FindIndexByName(item.Move);
				}
				monsterMoves.RefGet(monsters.Length, monsterIndex, moveIndex) = true;
			}
			return monsterMoves;
		}
	}
}