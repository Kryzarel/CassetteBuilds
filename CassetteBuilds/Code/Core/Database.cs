using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CassetteBuilds.Code.Data;

namespace CassetteBuilds.Code.Core
{
	public static class Database
	{
		public static readonly ReadOnlyMemory<Monster> Monsters;
		public static readonly ReadOnlyMemory<Move> Moves;
		public static readonly ReadOnlyMemory<bool> MonsterMoves;
		public static readonly ReadOnlyDictionary<string, int> MovesReverseLookup;

		static Database()
		{
			string baseDir = AppContext.BaseDirectory;
			string monstersPath = Path.Combine(baseDir, "Data", "Monsters.csv");
			string movesPath = Path.Combine(baseDir, "Data", "Moves.csv");
			string movesPerMonsterPath = Path.Combine(baseDir, "Data", "MovesPerMonster.csv");

			Monsters = DataDeserializer.DeserializeMonsters(monstersPath);
			Moves = DataDeserializer.DeserializeMoves(movesPath);
			MovesReverseLookup = ComputeMovesReverseLookup(Moves.Span);
			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs(movesPerMonsterPath);
			MonsterMoves = ComputeMonsterMoves(movesPerMonster, Monsters.Span, Moves.Span);
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