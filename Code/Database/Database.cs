using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Cassette_Builds.Code.Database
{
	public static class Database
	{
		public static readonly Monster[] Monsters;
		public static readonly Move[] Moves;
		public static readonly bool[,] MonsterMoves;
		public static readonly ReadOnlyDictionary<string, int> MovesReverseLookup;

		static Database()
		{
			Monsters = DataDeserializer.DeserializeMonsters("Data/Monsters.csv");
			Moves = DataDeserializer.DeserializeMoves("Data/Moves.csv");
			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs("Data/MovesPerMonster.csv");
			MonsterMoves = ComputeMonsterMoves(movesPerMonster, Monsters, Moves);

			Dictionary<string, int> reverseLookup = new(Moves.Length);
			MovesReverseLookup = reverseLookup.AsReadOnly();
			for (int i = 0; i < Moves.Length; i++)
			{
				reverseLookup[Moves[i].Name] = i;
			}
		}

		public static bool[,] ComputeMonsterMoves(MoveMonsterPair[] movesPerMonster, Monster[] monsters, Move[] moves)
		{
			bool[,] monsterMoves = new bool[monsters.Length, moves.Length];

			string lastMonster = "", lastMove = "";
			int monsterIndex = -1, moveIndex = -1;
			foreach (MoveMonsterPair item in movesPerMonster)
			{
				if (lastMonster != item.Monster)
				{
					monsterIndex = Array.FindIndex(monsters, m => m.Name == item.Monster);
				}
				if (lastMove != item.Move)
				{
					moveIndex = Array.FindIndex(moves, m => m.Name == item.Move);
				}
				monsterMoves[monsterIndex, moveIndex] = true;
			}
			return monsterMoves;
		}
	}
}