using System;
using Kryz.Collections;
using Kryz.CSV;

namespace Cassette_Builds.Code.Database
{
	public static class DataDeserializer
	{
		public static Monster[] DeserializeMonsters(string path)
		{
			return CsvDeserializer.Deserialize(path, CreateMonsterFromRow);
		}

		public static Move[] DeserializeMoves(string path)
		{
			return CsvDeserializer.Deserialize(path, CreateMoveFromRow);
		}

		public static MoveMonsterPair[] DeserializeMoveMonsterPairs(string path)
		{
			return CsvDeserializer.Deserialize(path, CreateMoveMonsterPairFromRow);
		}

		public static MoveMonsterPair[] DeserializeMoveMonsterPairsOld(string path)
		{
			return CsvDeserializer.Deserialize(path, CreateMoveMonsterPairFromRowOld);
		}

		private static Monster CreateMonsterFromRow(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache)
		{
			return new Monster(index,
				number: int.TryParse(row.GetSection(1, separatorCache), out int n) ? n : -1,
				name: string.Intern(new(row.GetSection(0, separatorCache))),
				type: string.Intern(new(row.GetSection(2, separatorCache))),
				hp: int.Parse(row.GetSection(3, separatorCache)),
				meleeAttack: int.Parse(row.GetSection(4, separatorCache)),
				meleeDefense: int.Parse(row.GetSection(5, separatorCache)),
				rangedAttack: int.Parse(row.GetSection(6, separatorCache)),
				rangedDefense: int.Parse(row.GetSection(7, separatorCache)),
				speed: int.Parse(row.GetSection(8, separatorCache)),
				wikiLink: string.Intern(new(row.GetSection(9, separatorCache)))
			);
		}

		private static Move CreateMoveFromRow(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache)
		{
			ReadOnlySpan<char> accuracy = row.GetSection(4, separatorCache);
			accuracy = accuracy[..Math.Max(0, accuracy.IndexOf('%'))];

			ReadOnlySpan<char> cost = row.GetSection(5, separatorCache);
			cost = cost[..Math.Max(0, cost.IndexOf("AP"))];

			return new Move(index,
				name: string.Intern(new(row.GetSection(0, separatorCache))),
				type: string.Intern(new(row.GetSection(1, separatorCache))),
				category: string.Intern(new(row.GetSection(2, separatorCache))),
				power: int.TryParse(row.GetSection(3, separatorCache), out int n) ? n : -1,
				accuracy: int.TryParse(accuracy, out n) ? n : -1,
				cost: int.TryParse(cost, out n) ? n : -1,
				wikiLink: string.Intern(new(row.GetSection(6, separatorCache)))
			);
		}

		private static MoveMonsterPair CreateMoveMonsterPairFromRow(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache)
		{
			return new MoveMonsterPair(
				move: string.Intern(new(row.GetSection(1, separatorCache))),
				monster: string.Intern(new(row.GetSection(0, separatorCache)))
			);
		}

		private static MoveMonsterPair CreateMoveMonsterPairFromRowOld(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache)
		{
			return new MoveMonsterPair(
				move: string.Intern(new(row.GetSection(0, separatorCache))),
				monster: string.Intern(new(row.GetSection(1, separatorCache)))
			);
		}
	}
}