using System;
using System.Collections.Generic;
using BenchmarkDotNet.Running;
using Cassette_Builds.Code;
using Cassette_Builds.Code.Admin;
using Cassette_Builds.Code.Database;
using Cassette_Builds.Code.Misc;

// BenchmarkRunner.Run<Benchmarks>();

// await DataUpdater.UpdateAll(clearCache: false);

_ = Database.Monsters;

Helpers.PrintMovePagesWithMissingMonsters();
Helpers.Print(Database.Monsters, nameof(Database.Monsters));
Console.WriteLine();
Helpers.Print(Database.Moves, nameof(Database.Moves));