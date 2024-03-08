using System;
using Kryz.Collections;

namespace Cassette_Builds.Code.Core
{
	public static class MonsterFinder
	{
		public static ReadOnlyNonAllocBuffer<int> GetMonstersCompatibleWith(in ReadOnlySpan<int> moveIndexes)
		{
			bool[,] monsterMoves = Database.Database.MonsterMoves;
			int monstersLen = monsterMoves.GetLength(0);
			NonAllocBuffer<int> buffer = new(monstersLen);

			for (int i = 0; i < monstersLen; i++)
			{
				bool isCompatible = true;
				foreach (int j in moveIndexes)
				{
					if (!monsterMoves[i, j])
					{
						isCompatible = false;
						break;
					}
				}
				if (isCompatible)
				{
					buffer.Add(i);
				}
			}
			return buffer;
		}
	}
}