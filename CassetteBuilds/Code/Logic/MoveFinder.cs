using System;
using System.Runtime.CompilerServices;

namespace CassetteBuilds.Code.Logic
{
	public static class MoveFinder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGetMoveIndex(string? moveName, out int index)
		{
			if (!string.IsNullOrEmpty(moveName) && Database.MovesReverseLookup.TryGetValue(moveName, out index))
			{
				return true;
			}
			index = -1;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMoveIndex(string? moveName)
		{
			TryGetMoveIndex(moveName, out int index);
			return index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMoveIndexes(in ReadOnlySpan<string> moves, in Span<int> buffer)
		{
			return GetMoveIndexesAsSpan(moves, buffer).Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int> GetMoveIndexesAsSpan(in ReadOnlySpan<string> moves, in Span<int> buffer)
		{
			if (moves.Length == 0 || moves.Length > buffer.Length)
				return default;

			for (int i = 0; i < moves.Length; i++)
			{
				buffer[i] = Database.MovesReverseLookup.TryGetValue(moves[i], out int index) ? index : -1;
			}
			return buffer[..moves.Length];
		}
	}
}