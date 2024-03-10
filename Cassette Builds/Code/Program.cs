using System;
using BenchmarkDotNet.Running;
using Cassette_Builds.Code;
using Cassette_Builds.Code.Admin;
using Cassette_Builds.Code.Core;
using Cassette_Builds.Code.Misc;

// await DataUpdater.UpdateAll(clearCache: false);

// Helpers.PrintMovePagesWithMissingMonsters();
// Helpers.Print(Database.Monsters, nameof(Database.Monsters));
// Console.WriteLine();
// Helpers.Print(Database.Moves, nameof(Database.Moves));

// BenchmarkRunner.Run<Benchmarks>();

string[] moves = new string[] { "Custom Starter", "Critical Mass", "Echolocation", "Mind-Meld" };
// string[] moves = new string[] { "Hypnotise", "Mind-Meld", "Sticky Spray" };
// string[] moves = new string[] { "Beast Wall", "Nurse", "Leech", "Doc Leaf" };
// string[] moves = new string[] { "Mind-Meld", "Echolocation", "Magnet" };
// string[] moves = new string[] { "Hypnotise", "Mind-Meld", "Nurse", "Doc Leaf", "Beast Wall" };
PrintMonsters(MonsterFinder.GetMonstersCompatibleWithAsSpan(moves, stackalloc int[Database.Monsters.Length]));

static void PrintMonsters(in ReadOnlySpan<int> monsterIndexes)
{
	if (monsterIndexes.IsEmpty)
	{
		Console.WriteLine("No monsters found");
		return;
	}

	foreach (int index in monsterIndexes)
	{
		Console.WriteLine(Database.Monsters.Span[index]);
	}
}