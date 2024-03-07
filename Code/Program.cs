using System;
using BenchmarkDotNet.Running;
using Cassette_Builds.Code;
using Cassette_Builds.Code.Admin;
using Cassette_Builds.Code.Database;
using Monster = Cassette_Builds.Code.Database.Monster;
using Move = Cassette_Builds.Code.Database.Move;

// BenchmarkRunner.Run<Benchmarks>();

await DataUpdater.UpdateAll(clearCache: false);

// string monster = "Khufo";
// string[] moves = new string[] { "Echolocation", "Critical Mass", "Wonderful 7", /* "Sticky Spray" */ };

// int monsterIndex = GetIndexes(monster, moves, out int[] moveIndexes);
// Console.WriteLine($"{monster} can use moves? {CanUseMoves(monsterIndex, moveIndexes)}");

// static bool CanUseMoves(int monsterIndex, int[] moveIndexes)
// {
// 	foreach (int moveIndex in moveIndexes)
// 	{
// 		if (!Database.MonsterMoves[monsterIndex, moveIndex])
// 			return false;
// 	}
// 	return true;
// }

// static int GetIndexes(string monster, string[] moves, out int[] moveIndexes)
// {
// 	int monsterIndex = Array.FindIndex(Database.Monsters, m => m.Name == monster);

// 	moveIndexes = new int[moves.Length];
// 	for (int i = 0; i < moves.Length; i++)
// 	{
// 		string item = moves[i];
// 		moveIndexes[i] = Array.FindIndex(Database.Moves, m => m.Name == moves[i]);
// 	}
// 	return monsterIndex;
// }