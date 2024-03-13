using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		public ReadOnlyObservableCollection<string> Moves { get; }
		public ReadOnlyObservableCollection<MonsterModel> Results { get; }
		public static IReadOnlyList<string> MoveNames { get; }

		private readonly ObservableCollection<string> moves = [];
		private readonly ObservableCollection<MonsterModel> results = [];

		private int count = 0;
		private readonly int[] moveIndexes = new int[8]; // A monster can't have more than 8 moves

		static MonsterSearchViewModel()
		{
			string[] names = new string[Database.Moves.Count];
			ReadOnlySpan<Move> moves = Database.MovesMem.Span;
			for (int i = 0; i < names.Length; i++)
			{
				names[i] = moves[i].Name;
			}
			MoveNames = names;
		}

		public MonsterSearchViewModel()
		{
			Moves = new(moves);
			Results = new(results);
		}

		public bool TryAddMove(string? moveName)
		{
			if (count >= moveIndexes.Length || string.IsNullOrWhiteSpace(moveName))
				return false;
			if (!Database.MovesReverseLookup.TryGetValue(moveName, out int index))
				return false;

			SearchText = null;
			selectedMove = null;

			moveIndexes[count++] = index;
			moves.Add(moveName);
			RecalculateResults();
			return true;
		}

		public bool TryRemoveMove(string? moveName)
		{
			if (count <= 0 || string.IsNullOrEmpty(moveName))
				return false;

			int index = moves.IndexOf(moveName);
			if (index < 0 || index > count)
				return false;

			moves.RemoveAt(index);
			count--;
			if (index < count)
			{
				Array.Copy(moveIndexes, index + 1, moveIndexes, index, count - index);
			}
			RecalculateResults();
			return true;
		}

		private void RecalculateResults()
		{
			results.Clear();
			ReadOnlySpan<int> indexes = moveIndexes;
			foreach (Monster monster in MonsterFinder.EnumerateMonstersCompatibleWith(indexes[..count]))
			{
				results.Add(new MonsterModel { Monster = monster });
			}
		}
	}
}