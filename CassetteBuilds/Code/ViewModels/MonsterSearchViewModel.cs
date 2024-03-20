using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using CassetteBuilds.Code.Logic;
using CassetteBuilds.Code.Models;
using CassetteBuilds.Code.Misc;
using ReactiveUI;

namespace CassetteBuilds.ViewModels
{
	public class MonsterSearchViewModel : ViewModelBase
	{
		public string? Name { get; }
		public int Number { get; }

		private string? _searchText;
		public string? SearchText { get => _searchText; set => this.RaiseAndSetIfChanged(ref _searchText, value); }

		private string? _selectedMove;
		public string? SelectedMove { get => _selectedMove; set => this.RaiseAndSetIfChanged(ref _selectedMove, value); }

		public static IReadOnlyList<string> MoveNames { get; }
		public static IReadOnlyList<Move> Moves => Database.Moves;
		public static IReadOnlyList<Monster> Monsters => Database.Monsters;

		public FlatTreeDataGridSource<Monster> Results { get; }
		public ReadOnlyObservableCollection<Move> MovesFilter { get; }
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

		public MonsterSearchViewModel(int number = 1)
		{
			Number = number;
			Name = $"Search {number}";

			TextColumnOptions<Monster> numberOptions = new() { CompareAscending = Monster.NumberComparisonAsc, CompareDescending = Monster.NumberComparisonDes, };
			FuncDataTemplate<Monster> imageTemplate = new((value, scope) => new Image() { Height = 40, Width = 40, [!Image.SourceProperty] = new Binding("Image") });
			Results = new(results)
			{
				Columns = {
					new TemplateColumn<Monster>("", imageTemplate),
					new TextColumn<Monster, string>("Name", m => m.Name),
					new TextColumn<Monster, string>("Number", m => m.DisplayNumber, options: numberOptions),
					new TextColumn<Monster, string>("Type", m => m.Type),
					new TextColumn<Monster, int>("HP", m => m.HP),
					new TextColumn<Monster, int>("M. Attack", m => m.MeleeAttack),
					new TextColumn<Monster, int>("M. Defense", m => m.MeleeDefense),
					new TextColumn<Monster, int>("R. Attack", m => m.RangedAttack),
					new TextColumn<Monster, int>("R. Defense", m => m.RangedDefense),
					new TextColumn<Monster, int>("Speed", m => m.Speed),
					new TextColumn<Monster, string>("Wiki Link", m => m.WikiLink),
				},
			};
			Results.DisableSelection();
			MovesFilter = new(movesFilter);

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