using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CassetteBuilds.Code.Core;
using CassetteBuilds.Code.Data;
using CassetteBuildsUI.Models;

namespace CassetteBuildsUI.ViewModels;

public class MonstersSearchViewModel : ViewModelBase
{
	public IReadOnlyList<MoveToSearch> MovesToSearch { get; }
	public ReadOnlyObservableCollection<string> MonstersFound { get; }

	private readonly int[] moveIndexes = new int[8];
	private readonly int[] monsterBuffer = new int[Logic.Database.Monsters.Count];
	private readonly ObservableCollection<string> monstersFound = [];

	public MonstersSearchViewModel()
	{
		ReadOnlySpan<Move> moves = Database.Moves.Span;
		string[] moveNames = new string[moves.Length];
		for (int i = 0; i < moves.Length; i++)
		{
			moveNames[i] = moves[i].Name;
		}

		MoveToSearch[] movesToSearch = new MoveToSearch[moveIndexes.Length];
		for (int i = 0; i < movesToSearch.Length; i++)
		{
			moveIndexes[i] = -1;
			movesToSearch[i] = new(i, moveNames);
			movesToSearch[i].PropertyChanged += OnMoveChanged;
		}
		MovesToSearch = movesToSearch;

		MonstersFound = new ReadOnlyObservableCollection<string>(monstersFound);
	}

	public void OnMoveChanged(object? sender, PropertyChangedEventArgs args)
	{
		if (sender is MoveToSearch moveToSearch && args.PropertyName == nameof(MoveToSearch.Move))
		{
			moveIndexes[moveToSearch.Index] = MoveFinder.GetMoveIndex(moveToSearch.Move);

			monstersFound.Clear();
			ReadOnlySpan<Monster> monsters = Database.Monsters.Span;
			foreach (int index in MonsterFinder.GetMonstersCompatibleWithAsSpan(moveIndexes, monsterBuffer))
			{
				monstersFound.Add(monsters[index].Name);
			}
		}
	}
}