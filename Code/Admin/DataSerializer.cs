using System.IO;
using System.Threading.Tasks;

namespace Cassette_Builds.Code.Admin
{
	public static class DataSerializer
	{
		public static async Task SerializeToCsv(string path, Monster[] monsters)
		{
			using FileStream stream = File.Create(path);
			using StreamWriter writer = new(stream);

			await writer.WriteAsync($"{nameof(Monster.Number)},{nameof(Monster.Name)},{nameof(Monster.Type)},{nameof(Monster.HP)},{nameof(Monster.MeleeAttack)},{nameof(Monster.MeleeDefense)},{nameof(Monster.RangedAttack)},{nameof(Monster.RangedDefense)},{nameof(Monster.Speed)},{nameof(Monster.Link)}");

			foreach (Monster monster in monsters)
			{
				await writer.WriteLineAsync();
				await writer.WriteAsync($"{monster.Number},{monster.Name},{monster.Type},{monster.HP},{monster.MeleeAttack},{monster.MeleeDefense},{monster.RangedAttack},{monster.RangedDefense},{monster.Speed},{monster.Link}");
			}
		}

		public static async Task SerializeToCsv(string path, Move[] moves)
		{
			using FileStream stream = File.Create(path);
			using StreamWriter writer = new(stream);

			await writer.WriteAsync($"{nameof(Move.Name)},{nameof(Move.Type)},{nameof(Move.Category)},{nameof(Move.Power)},{nameof(Move.Accuracy)},{nameof(Move.Cost)},{nameof(Move.Link)}");

			foreach (Move move in moves)
			{
				await writer.WriteLineAsync();
				await writer.WriteAsync($"{move.Name},{move.Type},{move.Category},{move.Power},{move.Accuracy},{move.Cost},{move.Link}");
			}
		}

		public static async Task SerializeToCsv(string path, MoveMonsterPair[] movesPerMonster)
		{
			using FileStream stream = File.Create(path);
			using StreamWriter writer = new(stream);

			await writer.WriteAsync($"{nameof(MoveMonsterPair.Move)},{nameof(MoveMonsterPair.Monster)}");

			foreach (MoveMonsterPair pair in movesPerMonster)
			{
				await writer.WriteLineAsync();
				await writer.WriteAsync($"{pair.Move},{pair.Monster}");
			}
		}
	}
}