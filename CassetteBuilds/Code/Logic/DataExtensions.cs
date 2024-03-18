using System;
using System.Runtime.CompilerServices;
using CassetteBuilds.Code.Models;

namespace CassetteBuilds.Code.Logic
{
	public static class DataExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindIndexByName(this in ReadOnlySpan<Monster> monsters, string name)
		{
			for (int i = 0; i < monsters.Length; i++)
			{
				if (monsters[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int FindIndexByName(this in ReadOnlySpan<Move> moves, string name)
		{
			for (int i = 0; i < moves.Length; i++)
			{
				if (moves[i].Name == name)
				{
					return i;
				}
			}
			return -1;
		}
	}
}