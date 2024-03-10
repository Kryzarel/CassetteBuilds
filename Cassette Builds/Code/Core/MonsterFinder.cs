using System;
using System.Runtime.CompilerServices;
using Cassette_Builds.Code.Data;

namespace Cassette_Builds.Code.Core
{
	public static class MonsterFinder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<string> moves, in Span<int> buffer)
		{
			ReadOnlySpan<int> moveIndexes = MoveFinder.GetMoveIndexesAsSpan(moves, stackalloc int[moves.Length]);
			return GetMonstersCompatibleWithAsSpan(moveIndexes, buffer).Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes, in Span<int> buffer)
		{
			return GetMonstersCompatibleWithAsSpan(moveIndexes, buffer).Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int> GetMonstersCompatibleWithAsSpan(in ReadOnlySpan<string> moves, in Span<int> buffer)
		{
			ReadOnlySpan<int> moveIndexes = MoveFinder.GetMoveIndexesAsSpan(moves, stackalloc int[moves.Length]);
			ReadOnlySpan<bool> monsterMoves = Database.MonsterMoves.Span;
			int monstersLen = Database.Monsters.Length;
			if (buffer.Length < monstersLen)
				return default;

			int count = 0;
			for (int i = 0; i < monstersLen; i++)
			{
				if (IsCompatible(moveIndexes, monsterMoves, monstersLen, i))
				{
					buffer[count++] = i;
				}
			}
			return buffer[..count];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int> GetMonstersCompatibleWithAsSpan(in ReadOnlySpan<int> moveIndexes, in Span<int> buffer)
		{
			ReadOnlySpan<bool> monsterMoves = Database.MonsterMoves.Span;
			int monstersLen = Database.Monsters.Length;
			if (buffer.Length < monstersLen)
				return default;

			int count = 0;
			for (int i = 0; i < monstersLen; i++)
			{
				if (IsCompatible(moveIndexes, monsterMoves, monstersLen, i))
				{
					buffer[count++] = i;
				}
			}
			return buffer[..count];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsCompatible(in ReadOnlySpan<int> moveIndexes, in ReadOnlySpan<bool> monsterMoves, int monstersLen, int monsterIndex)
		{
			foreach (int moveIndex in moveIndexes)
			{
				if (moveIndex < 0 || !monsterMoves.Get(monstersLen, monsterIndex, moveIndex))
				{
					return false;
				}
			}
			return true;
		}
	}
}