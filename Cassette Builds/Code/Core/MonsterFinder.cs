using System;
using Cassette_Builds.Code.Database;

namespace Cassette_Builds.Code.Core
{
	public static class MonsterFinder
	{
		public static int GetMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes, in Span<int> buffer)
		{
			ReadOnlySpan<bool> monsterMoves = Database.Database.MonsterMoves.Span;
			int monstersLen = Database.Database.Monsters.Length;
			if (buffer.Length < monstersLen)
				return -1;

			int count = 0;
			for (int i = 0; i < monstersLen; i++)
			{
				bool isCompatible = true;
				foreach (int j in moveIndexes)
				{
					if (!monsterMoves.Get(monstersLen, i, j))
					{
						isCompatible = false;
						break;
					}
				}
				if (isCompatible)
				{
					buffer[count++] = i;
				}
			}
			return count;
		}
	}
}