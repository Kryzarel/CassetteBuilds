using Avalonia.Controls;
using Avalonia.Input;
using CassetteBuildsUI.Logic;

namespace CassetteBuildsUI.Views;

public partial class MonsterSearchView : UserControl
{
	public MonsterSearchView()
	{
		InitializeComponent();

		MoveSearchBox.AddDropdownInteraction();
		MoveSearchBox.KeyUp += (sender, args) =>
		{
			if (args.Key is Key.Enter or Key.Return)
			{
				AddButton.Command?.Execute(AddButton.CommandParameter);
			}
		};
	}
}