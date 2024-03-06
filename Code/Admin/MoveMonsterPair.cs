namespace Cassette_Builds.Code.Admin
{
	public readonly struct MoveMonsterPair
	{
		public readonly string Move;
		public readonly string Monster;

		public MoveMonsterPair(string move, string monster)
		{
			Move = move;
			Monster = monster;
		}
	}
}