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
using Avalonia.Layout;
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

		private Move? _selectedMove;
		public Move? SelectedMove { get => _selectedMove; set => this.RaiseAndSetIfChanged(ref _selectedMove, value); }

		public static IReadOnlyList<Move> Moves => Database.Moves;
		public static IReadOnlyList<Monster> Monsters => Database.Monsters;

		public FlatTreeDataGridSource<Monster> Results { get; }
		public ReadOnlyObservableCollection<Move> MovesFilter { get; }
		public ReactiveCommand<Move?, Unit> AddMoveCommand { get; }

		private readonly ObservableCollection<Move> movesFilter = [];
		private readonly ObservableCollection<Monster> results = [];

		private int count = 0;
		private readonly int[] moveIndexes = new int[8]; // A monster can't have more than 8 moves

		public MonsterSearchViewModel() : this(1) { }

		public MonsterSearchViewModel(int number)
		{
			Number = number;

			TextColumnOptions<Monster> numberOptions = new() { CompareAscending = Monster.CompareIndexAsc, CompareDescending = Monster.CompareIndexDes };
			TemplateColumnOptions<Monster> nameOptions = new() { CompareAscending = Monster.CompareNameAsc, CompareDescending = Monster.CompareNameDes };
			TemplateColumnOptions<Monster> typeOptions = new() { CompareAscending = Monster.CompareTypeAsc, CompareDescending = Monster.CompareTypeDes, MinWidth = new GridLength(110) };
			Results = new(results)
			{
				Columns = {
					new TemplateColumn<Monster>("", new FuncDataTemplate<Monster>(ImageTemplate, supportsRecycling: true)),
					new TemplateColumn<Monster>("Name", new FuncDataTemplate<Monster>(NameTemplate, supportsRecycling: true), options: nameOptions),
					new TextColumn<Monster, string>("Number", m => m.DisplayNumber, options: numberOptions),
					new TemplateColumn<Monster>("Type", new FuncDataTemplate<Monster>(TypeTemplate, supportsRecycling: false), options: typeOptions),
					new TextColumn<Monster, int>("HP", m => m.HP),
					new TextColumn<Monster, int>("M. Attack", m => m.MeleeAttack),
					new TextColumn<Monster, int>("M. Defense", m => m.MeleeDefense),
					new TextColumn<Monster, int>("R. Attack", m => m.RangedAttack),
					new TextColumn<Monster, int>("R. Defense", m => m.RangedDefense),
					new TextColumn<Monster, int>("Speed", m => m.Speed),
				},
			};
			Results.DisableSelection();
			MovesFilter = new(movesFilter);

			RecalculateResults();

			AddMoveCommand = ReactiveCommand.Create<Move?>(AddMove, this.WhenAnyValue(x => x.SelectedMove, CanAddMove));
		}

		private static Button? NameTemplate(Monster monster, INameScope scope)
		{
			Button button = new();
			button.Classes.Add("hyperlink");
			button.MinWidth = 110;
			button.Margin = new Thickness(5, 0, 0, 0);
			button[!Button.ContentProperty] = Bind<Monster>.Property(nameof(Monster.Name), m => m.Name);
			button[!Button.CommandProperty] = Bind.Command<string?, bool>(nameof(Features.UrlOpener.OpenUrl), Features.UrlOpener.OpenUrl);
			button[!Button.CommandParameterProperty] = Bind<Monster>.Property(nameof(Monster.WikiLink), m => m.WikiLink);
			button[!ToolTip.TipProperty] = Bind<Monster>.Property(nameof(Monster.WikiLink), m => m.WikiLink);
			return button;
		}

		private static Image? ImageTemplate(Monster monster, INameScope scope)
		{
			return new Image() { Height = 40, Width = 40, [!Image.SourceProperty] = Bind<Monster>.Property(nameof(Monster.Image), m => m.Image) };
		}

		private static StackPanel? TypeTemplate(Monster monster, INameScope scope)
		{
			return new StackPanel()
			{
				Orientation = Orientation.Horizontal,
				Children = {
					new Image() {
						VerticalAlignment = VerticalAlignment.Center,
						Height = 25, Width = 25, Margin = new Thickness(5, 0),
						[!Image.SourceProperty] = Bind<Monster>.Property(nameof(Monster.TypeImage), m => m.TypeImage)
					},
					new TextBlock() {
						VerticalAlignment = VerticalAlignment.Center,
						[!TextBlock.TextProperty] = Bind<Monster>.Property(nameof(Monster.Type), m => m.Type)
					},
				}
			};
		}

		public bool CanAddMove(string? moveName)
		{
			if (count >= moveIndexes.Length || moveName == null)
				return false;
			if (!MoveFinder.TryGetMoveIndex(moveName, out _))
				return false;
			return true;
		}

		public bool CanAddMove(Move? move)
		{
			return count < moveIndexes.Length && move != null && move.Index >= 0;
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