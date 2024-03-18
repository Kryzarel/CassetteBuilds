using System;
using System.IO;
using CassetteBuilds.Code.Models;
using Kryz.CSV;

namespace CassetteBuilds.Code.Logic
{
    public static class DataDeserializer
	{
		public static Monster[] DeserializeMonsters(TextReader reader)
		{
			return CsvDeserializer.FromTextReader(reader, CreateMonsterFromRow);
		}

		public static Move[] DeserializeMoves(TextReader reader)
		{
			return CsvDeserializer.FromTextReader(reader, CreateMoveFromRow);
		}

		public static MoveMonsterPair[] DeserializeMoveMonsterPairs(TextReader reader)
		{
			return CsvDeserializer.FromTextReader(reader, CreateMoveMonsterPairFromRow);
		}

		public static MoveMonsterPair[] DeserializeMoveMonsterPairsOld(TextReader reader)
		{
			return CsvDeserializer.FromTextReader(reader, CreateMoveMonsterPairFromRowOld);
		}

		private static Monster CreateMonsterFromRow(in CsvRow row)
		{
			return new Monster(row.Index,
				number: int.TryParse(row[1], out int n) ? n : -1,
				name: string.Intern(row[0].ToString()),
				type: string.Intern(row[2].ToString()),
				hp: int.Parse(row[3]),
				meleeAttack: int.Parse(row[4]),
				meleeDefense: int.Parse(row[5]),
				rangedAttack: int.Parse(row[6]),
				rangedDefense: int.Parse(row[7]),
				speed: int.Parse(row[8]),
				wikiLink: string.Intern(row[9].ToString())
			);
		}

		private static Move CreateMoveFromRow(in CsvRow row)
		{
			return new Move(row.Index,
				name: string.Intern(row[0].ToString()),
				type: string.Intern(row[1].ToString()),
				category: string.Intern(row[2].ToString()),
				power: int.TryParse(row[3], out int n) ? n : -1,
				accuracy: int.TryParse(row[4].TrimEnd('%'), out n) ? n : -1,
				cost: int.TryParse(row[5].TrimEnd("AP"), out n) ? n : -1,
				wikiLink: string.Intern(row[6].ToString())
			);
		}

		private static MoveMonsterPair CreateMoveMonsterPairFromRow(in CsvRow row)
		{
			return new MoveMonsterPair(
				move: string.Intern(new(row[1])),
				monster: string.Intern(new(row[0]))
			);
		}

		private static MoveMonsterPair CreateMoveMonsterPairFromRowOld(in CsvRow row)
		{
			return new MoveMonsterPair(
				move: string.Intern(new(row[0])),
				monster: string.Intern(new(row[1]))
			);
		}
	}
}