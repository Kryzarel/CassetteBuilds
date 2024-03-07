using System;
using Cassette_Builds.Code.Admin;
using Cassette_Builds.Code.Database;
using Monster = Cassette_Builds.Code.Database.Monster;
using Move = Cassette_Builds.Code.Database.Move;

// await DataUpdater.UpdateAll(clearCache: false);

foreach (Monster item in Database.Monsters)
{
	Console.WriteLine(item);
}
Console.WriteLine();
foreach (Move item in Database.Moves)
{
	Console.WriteLine(item);
}