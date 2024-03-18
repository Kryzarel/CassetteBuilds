using System;
using Avalonia.Controls;
using Avalonia.Input;
using CassetteBuilds.Code.Models;
using CassetteBuilds.Logic;

namespace CassetteBuilds.Views;

public partial class MonsterSearchView : UserControl
{
	public MonsterSearchView()
	{
		InitializeComponent();

		MoveSearchBox.AddDropdown();
		MoveSearchBox.KeyUp += (sender, args) =>
		{
			if (args.Key is Key.Enter or Key.Return)
			{
				AddButton.Command?.Execute(AddButton.CommandParameter);
			}
		};

		// Can't bind CustomSortComparer via XAML for some reason, gotta do it in code...
		FindColumn(ResultsGrid, "Number")!.CustomSortComparer = Monster.NumberComparer;
	}

	public static DataGridColumn? FindColumn(DataGrid dataGrid, string header)
	{
		foreach (DataGridColumn? item in dataGrid.Columns)
		{
			if (item?.Header is string h && h.Equals(header, StringComparison.OrdinalIgnoreCase))
			{
				return item;
			}
		}
		return null;
	}
}