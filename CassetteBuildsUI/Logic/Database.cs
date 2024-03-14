using System;
using System.Collections.Generic;
using CassetteBuilds.Code.Data;
using CassetteBuildsUI.Models;
using CoreDB = CassetteBuilds.Code.Core.Database;

namespace CassetteBuildsUI.Logic
{
	public static class Database
	{
		public static readonly IReadOnlyList<MonsterModel> Monsters;
		public static readonly IReadOnlyList<MoveModel> Moves;

		static Database()
		{
			MonsterModel[] monsters = new MonsterModel[CoreDB.Monsters.Length];
			ReadOnlySpan<Monster> coreMonsters = CoreDB.Monsters.Span;
			for (int i = 0; i < coreMonsters.Length; i++)
			{
				monsters[i] = new MonsterModel(coreMonsters[i]);
			}

			MoveModel[] moves = new MoveModel[CoreDB.Moves.Length];
			ReadOnlySpan<Move> coreMoves = CoreDB.Moves.Span;
			for (int i = 0; i < coreMoves.Length; i++)
			{
				moves[i] = new MoveModel(coreMoves[i]);
			}

			Monsters = monsters;
			Moves = moves;
		}
	}
}