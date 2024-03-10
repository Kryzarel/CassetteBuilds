using System;
using Cassette_Builds.Code.Database;

namespace Cassette_Builds.Code.Core
{
	public static class MoveFinder
	{
		public static ReadOnlySpan<int> GetMoveIndexes_ReverseLookup(string[] moves, in Span<int> indexes)
		{
			if (moves.Length == 0 || moves.Length > indexes.Length)
				return default;

			for (int i = 0; i < moves.Length; i++)
			{
				indexes[i] = Database.Database.MovesReverseLookup.TryGetValue(moves[i], out int index) ? index : -1;
			}
			return indexes;
		}

		public static ReadOnlySpan<int> GetMoveIndexes(string[] moves, in Span<int> indexes)
		{
			if (moves.Length == 0 || moves.Length > indexes.Length)
				return default;

			for (int i = 0; i < moves.Length; i++)
			{
				indexes[i] = Database.Database.Moves.Span.FindIndexByName(moves[i]);
			}
			return indexes;
		}
	}
}