using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using CassetteBuilds.Code;
using CassetteBuilds.Code.Admin;
using CassetteBuilds.Code.Core;
using CassetteBuilds.Code.Misc;

internal class Program
{
	private static void Main(string[] args)
	{
		FindMonsters(args);

		FindMonsters(["Custom Starter", "Critical Mass", "Echolocation", "Mind-Meld"]);
		FindMonsters(["Hypnotise", "Mind-Meld", "Sticky Spray"]);
		FindMonsters(["Beast Wall", "Nurse", "Leech", "Doc Leaf"]);
		FindMonsters(["Mind-Meld", "Echolocation", "Magnet"]);
		FindMonsters(["Hypnotise", "Mind-Meld", "Nurse", "Doc Leaf", "Beast Wall"]);
	}

	private static async Task Update() => await DataUpdater.UpdateAll(clearCache: false);

	private static void RunBenchmarks() => BenchmarkRunner.Run<Benchmarks>();
	private static void PrintMovePagesWithMissingMonsters() => Helpers.PrintMovePagesWithMissingMonsters();

	private static void FindMonsters(string[] moves)
	{
		Console.WriteLine();
		Helpers.PrintSingleLine<string>(moves, messageWhenEmpty: "No moves selected");
		ReadOnlySpan<int> monsterIndexes = MonsterFinder.GetMonstersCompatibleWithAsSpan(moves, stackalloc int[Database.Monsters.Length]);
		Helpers.Print(monsterIndexes, i => Database.Monsters.Span[i], messageWhenEmpty: "No monsters found");
	}
}