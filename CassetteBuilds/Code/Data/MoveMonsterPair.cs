using System;

namespace CassetteBuilds.Code.Data
{
	public readonly struct MoveMonsterPair : IEquatable<MoveMonsterPair>
	{
		public readonly string Move;
		public readonly string Monster;

		public MoveMonsterPair(string move, string monster)
		{
			Move = move;
			Monster = monster;
		}

		public bool Equals(MoveMonsterPair other)
		{
			return Move == other.Move && Monster == other.Monster;
		}

		public override bool Equals(object? obj)
		{
			return obj is MoveMonsterPair other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Move, Monster);
		}

		public override string ToString()
		{
			return $"{Move}, {Monster}";
		}

		public static bool operator ==(MoveMonsterPair left, MoveMonsterPair right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(MoveMonsterPair left, MoveMonsterPair right)
		{
			return !(left == right);
		}
	}
}