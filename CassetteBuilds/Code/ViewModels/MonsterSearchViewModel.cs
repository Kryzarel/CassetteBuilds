using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using CassetteBuilds.Code.Logic;
using CassetteBuilds.Code.Models;
using CassetteBuilds.Code.Misc;
using ReactiveUI;
using Avalonia;

namespace CassetteBuilds.ViewModels
{
	public class MonsterSearchViewModel : ViewModelBase
	{
		public string? Name { get; private set; }

		private int _number;
		public int Number { get => _number; set { _number = value; Name = $"Search {_number}"; } }

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
		public ReactiveCommand<string, Unit> OpenLinkCommand { get; }

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

		public MonsterSearchViewModel() : this(1) { }

		public MonsterSearchViewModel(int number)
		{
			Number = number;

			TextColumnOptions<Monster> numberOptions = new() { CompareAscending = Monster.NumberComparisonAsc, CompareDescending = Monster.NumberComparisonDes };
			Results = new(results)
			{
				Columns = {
					new TemplateColumn<Monster>("", new FuncDataTemplate<Monster>(ImageTemplate, supportsRecycling: true)),
					new TextColumn<Monster, string>("Name", m => m.Name),
					new TextColumn<Monster, string>("Number", m => m.DisplayNumber, options: numberOptions),
					new TextColumn<Monster, string>("Type", m => m.Type),
					new TextColumn<Monster, int>("HP", m => m.HP),
					new TextColumn<Monster, int>("M. Attack", m => m.MeleeAttack),
					new TextColumn<Monster, int>("M. Defense", m => m.MeleeDefense),
					new TextColumn<Monster, int>("R. Attack", m => m.RangedAttack),
					new TextColumn<Monster, int>("R. Defense", m => m.RangedDefense),
					new TextColumn<Monster, int>("Speed", m => m.Speed),
				},
			};
			// TODO: Implement this for all platforms
			if (PlatformHelpers.SupportsBrowser())
			{
				Results.Columns.Add(new TemplateColumn<Monster>("Wiki Link", new FuncDataTemplate<Monster>(LinkTemplate, supportsRecycling: true)));
			}
			Results.DisableSelection();
			MovesFilter = new(movesFilter);

			RecalculateResults();

			AddMoveCommand = ReactiveCommand.Create<string?>(AddMove, this.WhenAnyValue(x => x.SelectedMove, CanAddMove));
			OpenLinkCommand = ReactiveCommand.Create<string>(PlatformHelpers.OpenBrowser);
		}

		private static Image? ImageTemplate(Monster monster, INameScope scope)
		{
			return new Image() { Height = 40, Width = 40, [!Image.SourceProperty] = monster.WhenAnyValue(m => m.Image).ToBinding() };
		}

		private Button? LinkTemplate(Monster monster, INameScope scope)
		{
			Button button = new();
			button.Classes.Add("hyperlink");
			button.Bind(Button.ContentProperty, monster.WhenAnyValue(m => m.Name));
			button.Bind(Button.CommandProperty, this.WhenAnyValue(v => v.OpenLinkCommand));
			button.Bind(Button.CommandParameterProperty, monster.WhenAnyValue(m => m.WikiLink));
			return button;
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

		public void Clear()
		{
			SearchText = null;
			SelectedMove = null;
			movesFilter.Clear();
			Array.Clear(moveIndexes);
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
			else
			{
				foreach (ref readonly Monster monster in MonsterFinder.EnumerateMonstersCompatibleWith(indexes))
				{
					results.Add(monster);
				}
			}
		}
	}
}