using System;

namespace Cassette_Builds.Code.Core
{
	public static class MoveFinder
	{
		public static ReadOnlySpan<int> GetMoveIndexes(string[] moves, in Span<int> indexes)
		{
			if (moves.Length == 0 || moves.Length > indexes.Length)
				return default;

			for (int i = 0; i < moves.Length; i++)
			{
				indexes[i] = Array.FindIndex(Database.Database.Moves, m => m.Name == moves[i]);
			}
			return indexes;
		}
	}
}