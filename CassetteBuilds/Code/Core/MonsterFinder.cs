using System;
using System.Runtime.CompilerServices;
using CassetteBuilds.Code.Data;

namespace CassetteBuilds.Code.Core
{
	public static class MonsterFinder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes, in Span<Monster> buffer)
		{
			ReadOnlySpan<Monster> monsters = Database.MonstersMem.Span;
			ReadOnlySpan<bool> monsterMoves = Database.MonsterMovesMem.Span;
			int monstersLen = Database.MonsterMovesMem.Length;
			if (buffer.Length < monstersLen)
				return default;

			int count = 0;
			for (int i = 0; i < monstersLen; i++)
			{
				if (IsCompatible(moveIndexes, monsterMoves, monstersLen, i))
				{
					buffer[count++] = monsters[i];
				}
			}
			return count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<Monster> GetMonstersCompatibleWithAsSpan(in ReadOnlySpan<int> moveIndexes, in Span<Monster> buffer)
		{
			return buffer[..GetMonstersCompatibleWith(moveIndexes, buffer)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes, in Span<int> buffer)
		{
			ReadOnlySpan<bool> monsterMoves = Database.MonsterMovesMem.Span;
			int monstersLen = Database.MonstersMem.Length;
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
			return count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int> GetMonstersCompatibleWithAsSpan(in ReadOnlySpan<int> moveIndexes, in Span<int> buffer)
		{
			return buffer[..GetMonstersCompatibleWith(moveIndexes, buffer)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<string> moves, in Span<int> buffer)
		{
			ReadOnlySpan<int> moveIndexes = MoveFinder.GetMoveIndexesAsSpan(moves, stackalloc int[moves.Length]);
			return GetMonstersCompatibleWith(moveIndexes, buffer);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<int> GetMonstersCompatibleWithAsSpan(in ReadOnlySpan<string> moves, in Span<int> buffer)
		{
			return buffer[..GetMonstersCompatibleWith(moves, buffer)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool IsCompatible(in ReadOnlySpan<int> moveIndexes, in ReadOnlySpan<bool> monsterMoves, int monstersLen, int monsterIndex)
		{
			foreach (int moveIndex in moveIndexes)
			{
				if (moveIndex >= 0 && !monsterMoves.Get(monstersLen, monsterIndex, moveIndex))
				{
					return false;
				}
			}
			return !moveIndexes.IsEmpty;
		}
	}
}