using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CassetteBuilds.Code.Core;
using CassetteBuilds.Code.Data;
using CassetteBuildsUI.Models;
using ReactiveUI;

namespace CassetteBuildsUI.ViewModels
{
	public class MonsterSearchViewModel : ViewModelBase
	{
		private string? searchText;
		public string? SearchText { get => searchText; set => this.RaiseAndSetIfChanged(ref searchText, value); }

		private string? selectedMove;
		public string? SelectedMove { get => selectedMove; set => this.RaiseAndSetIfChanged(ref selectedMove, value); }

		public static IReadOnlyList<string> MoveNames { get; }
		public static IReadOnlyList<MoveModel> Moves => Logic.Database.Moves;
		public static IReadOnlyList<MonsterModel> Monsters => Logic.Database.Monsters;

		public ReadOnlyObservableCollection<MoveModel> MovesFilter { get; }
		public ReadOnlyObservableCollection<MonsterModel> Results { get; }
		public ReactiveCommand<string?, Unit> AddMoveCommand { get; }

		private readonly ObservableCollection<MoveModel> movesFilter = [];
		private readonly ObservableCollection<MonsterModel> results = [];

		private int count = 0;
		private readonly int[] moveIndexes = new int[8]; // A monster can't have more than 8 moves

		static MonsterSearchViewModel()
		{
			string[] moveNames = new string[Database.Moves.Length];
			foreach (Move move in Database.Moves.Span)
			{
				moveNames[move.Index] = move.Name;
			}
			MoveNames = moveNames;
		}

		public MonsterSearchViewModel()
		{
			MovesFilter = new(movesFilter);
			Results = new(results);
			RecalculateResults();

			IObservable<bool> canAddMove = this.WhenAnyValue(x => x.SelectedMove, CanAddMove);
			AddMoveCommand = ReactiveCommand.Create<string?>(AddMove, canAddMove);
		}

		public bool CanAddMove(string? moveName)
		{
			if (count >= moveIndexes.Length || moveName == null)
				return false;
			if (!MoveFinder.TryGetMoveIndex(moveName, out _))
				return false;
			return true;
		}

		public void AddMove(string? moveName)
		{
			if (count >= moveIndexes.Length || moveName == null)
				return;
			if (!MoveFinder.TryGetMoveIndex(moveName, out int index))
				return;
			AddMove(Moves[index]);
		}

		public void AddMove(MoveModel? move)
		{
			if (count >= moveIndexes.Length || move == null)
				return;

			SearchText = null;
			SelectedMove = null;

			moveIndexes[count++] = move.Index;
			movesFilter.Add(move);
			RecalculateResults();
		}

		public void RemoveMove(MoveModel? move)
		{
			if (count <= 0 || move == null)
				return;

			int index = movesFilter.IndexOf(move);
			if (index < 0 || index > count)
				return;

			movesFilter.RemoveAt(index);
			count--;
			if (index < count)
			{
				Array.Copy(moveIndexes, index + 1, moveIndexes, index, count - index);
			}
			RecalculateResults();
		}

		private void RecalculateResults()
		{
			results.Clear();
			ReadOnlySpan<int> indexes = moveIndexes.AsSpan()[..count];

			if (indexes.IsEmpty)
			{
				foreach (MonsterModel monster in Monsters)
				{
					results.Add(monster);
				}
			}

			foreach (ref readonly Monster monster in MonsterFinder.EnumerateMonstersCompatibleWith(indexes))
			{
				results.Add(Logic.Database.Monsters[monster.Index]);
			}
		}
	}
}