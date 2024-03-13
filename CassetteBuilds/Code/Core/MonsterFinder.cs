using System;
using System.Runtime.CompilerServices;
using CassetteBuilds.Code.Data;

namespace CassetteBuilds.Code.Core
{
	public static class MonsterFinder
	{
		public ref struct Enumerator
		{
			private readonly ReadOnlySpan<int> moveIndexes;
			private readonly ReadOnlySpan<bool> monsterMoves;
			private readonly ReadOnlySpan<Monster> monsters;
			private int index;

			public readonly ref readonly Monster Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => ref monsters[index];
			}

			public Enumerator(ReadOnlySpan<int> moveIndexes, ReadOnlySpan<bool> monsterMoves, ReadOnlySpan<Monster> monsters)
			{
				this.moveIndexes = moveIndexes;
				this.monsterMoves = monsterMoves;
				this.monsters = monsters;
				index = -1;
			}

			public bool MoveNext()
			{
				int i = index;
				while (++i < monsters.Length)
				{
					if (IsCompatible(moveIndexes, monsterMoves, monsters.Length, i))
					{
						index = i;
						return true;
					}
				}
				return false;
			}

			public readonly Enumerator GetEnumerator() => this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Enumerator EnumerateMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes)
		{
			return new Enumerator(moveIndexes, Database.MonsterMovesMem.Span, Database.MonstersMem.Span);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes, in Span<Monster> buffer)
		{
			if (buffer.Length < Database.MonstersMem.Length)
				return default;

			int count = 0;
			foreach (Monster monster in EnumerateMonstersCompatibleWith(moveIndexes))
			{
				buffer[count++] = monster;
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
			if (buffer.Length < Database.MonstersMem.Length)
				return default;

			int count = 0;
			foreach (Monster monster in EnumerateMonstersCompatibleWith(moveIndexes))
			{
				buffer[count++] = monster.Index;
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