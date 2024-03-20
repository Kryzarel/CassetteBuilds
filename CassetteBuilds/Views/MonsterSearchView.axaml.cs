using System;
using Avalonia.Controls;
using Avalonia.Input;
using CassetteBuilds.Code.Misc;

namespace CassetteBuilds.Views;

public partial class MonsterSearchView : UserControl
{
	public MonsterSearchView()
	{
		InitializeComponent();

		LayoutUpdated += ChangeView;
	}

	private void ChangeView(object? sender, EventArgs args)
	{
		LayoutUpdated -= ChangeView; // We must unsubscribe from the event to prevent infinite LayoutUpdate loop

		MoveSearchBox.AddDropdown();
		MoveSearchBox.KeyUp += (sender, args) =>
		{
			if (args.Key is Key.Enter or Key.Return)
			{
				AddButton.Command?.Execute(AddButton.CommandParameter);
			}
		};
	}
}