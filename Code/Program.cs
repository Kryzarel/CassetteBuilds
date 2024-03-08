using System;
using System.Collections.Generic;
using BenchmarkDotNet.Running;
using Cassette_Builds.Code;
using Cassette_Builds.Code.Admin;
using Cassette_Builds.Code.Database;

// BenchmarkRunner.Run<Benchmarks>();

// await DataUpdater.UpdateAll(clearCache: false);

_ = Database.Monsters;

MoveMonsterPair[] movesPerMonster = DataDeserializer.DeserializeMoveMonsterPairs("Data/MovesPerMonster.csv");
MoveMonsterPair[] movesPerMonsterOld = DataDeserializer.DeserializeMoveMonsterPairsOld("Data/MovesPerMonster_Old.csv");

HashSet<MoveMonsterPair> missing = new(movesPerMonster);
missing.ExceptWith(movesPerMonsterOld);

Console.WriteLine("Missing Monsters on Move page");
foreach (MoveMonsterPair item in missing)
{
	Console.WriteLine(item);
}
Console.WriteLine();

foreach (Monster item in Database.Monsters)
{
	Console.WriteLine(item);
}
Console.WriteLine();
foreach (Move item in Database.Moves)
{
	Console.WriteLine(item);
}

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