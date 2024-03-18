using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using CassetteBuilds.Code.Logic;
using CassetteBuilds.Code.Models;
using ReactiveUI;

namespace CassetteBuilds.ViewModels
{
	public class MonsterSearchViewModel : ViewModelBase
	{
		private string? searchText;
		public string? SearchText { get => searchText; set => this.RaiseAndSetIfChanged(ref searchText, value); }

		private string? selectedMove;
		public string? SelectedMove { get => selectedMove; set => this.RaiseAndSetIfChanged(ref selectedMove, value); }

		public static IReadOnlyList<string> MoveNames { get; }
		public static IReadOnlyList<Move> Moves => Database.Moves;
		public static IReadOnlyList<Monster> Monsters => Database.Monsters;

		public ReadOnlyObservableCollection<Move> MovesFilter { get; }
		public ReadOnlyObservableCollection<Monster> Results { get; }
		public ReactiveCommand<string?, Unit> AddMoveCommand { get; }

		private readonly ObservableCollection<Move> movesFilter = [];
		private readonly ObservableCollection<Monster> results = [];

		private int count = 0;
		private readonly int[] moveIndexes = new int[8]; // A monster can't have more than 8 moves

		static MonsterSearchViewModel()
		{
			string[] moveNames = new string[Database.Moves.Count];
			for (int i = 0; i < Database.Moves.Count; i++)
			{
				Move move = Database.Moves[i];
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

		public void AddMove(Move? move)
		{
			if (count >= moveIndexes.Length || move == null)
				return;

			SearchText = null;
			SelectedMove = null;

			moveIndexes[count++] = move.Index;
			movesFilter.Add(move);
			RecalculateResults();
		}

		public void RemoveMove(Move? move)
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
				foreach (Monster monster in Monsters)
				{
					results.Add(monster);
				}
			}

			foreach (ref readonly Monster monster in MonsterFinder.EnumerateMonstersCompatibleWith(indexes))
			{
				results.Add(monster);
			}
		}
	}
}