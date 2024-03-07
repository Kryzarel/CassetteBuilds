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

		private static Monster CreateMonsterFromRow(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache)
		{
			return new Monster(index,
				number: int.Parse(row.GetSection(0, separatorCache)),
				name: string.Intern(new(row.GetSection(1, separatorCache))),
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
			return new Move(index,
				name: string.Intern(new(row.GetSection(0, separatorCache))),
				type: string.Intern(new(row.GetSection(1, separatorCache))),
				category: string.Intern(new(row.GetSection(2, separatorCache))),
				power: int.Parse(row.GetSection(3, separatorCache)),
				accuracy: int.Parse(row.GetSection(4, separatorCache)),
				cost: int.Parse(row.GetSection(5, separatorCache)),
				wikiLink: string.Intern(new(row.GetSection(6, separatorCache)))
			);
		}

		private static MoveMonsterPair CreateMoveMonsterPairFromRow(int index, in ReadOnlySpan<char> row, in ReadOnlySpan<int> separatorCache)
		{
			return new MoveMonsterPair(
				move: string.Intern(new(row.GetSection(0, separatorCache))),
				monster: string.Intern(new(row.GetSection(1, separatorCache)))
			);
		}
	}
}