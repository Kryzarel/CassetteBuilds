using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Platform;
using CassetteBuilds.Code.Models;

namespace CassetteBuilds.Code.Logic
{
	public static class Database
	{
		public static readonly IReadOnlyList<Monster> Monsters;
		public static readonly IReadOnlyList<Move> Moves;
		public static readonly IReadOnlyList<bool> MonsterMoves;
		public static readonly ReadOnlyDictionary<string, int> MovesReverseLookup;

		public static ReadOnlySpan<Monster> MonstersSpan => monsters;
		public static ReadOnlySpan<Move> MovesSpan => moves;
		public static ReadOnlySpan<bool> MonsterMovesSpan => monsterMoves;

		private static readonly Monster[] monsters;
		private static readonly Move[] moves;
		private static readonly bool[] monsterMoves;

		static Database()
		{
			using TextReader monstersReader = new StreamReader(AssetLoader.Open(new Uri("avares://CassetteBuilds/Assets/Data/Monsters.csv")));
			using TextReader movesReader = new StreamReader(AssetLoader.Open(new Uri("avares://CassetteBuilds/Assets/Data/Moves.csv")));
			using TextReader monsterMovesReader = new StreamReader(AssetLoader.Open(new Uri("avares://CassetteBuilds/Assets/Data/MovesPerMonster.csv")));

			Monsters = monsters = DataDeserializer.DeserializeMonsters(monstersReader);
			Moves = moves = DataDeserializer.DeserializeMoves(movesReader);
			MovesReverseLookup = ComputeMovesReverseLookup(moves);
			MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs(monsterMovesReader);
			MonsterMoves = monsterMoves = ComputeMonsterMoves(movesPerMonster, monsters, moves);
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