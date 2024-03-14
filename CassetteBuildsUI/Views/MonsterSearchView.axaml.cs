using Avalonia.Controls;
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
			if (args.Key == Avalonia.Input.Key.Enter || args.Key == Avalonia.Input.Key.Return)
			{
				AddButton.Command?.Execute(AddButton.CommandParameter);
			}
		};
	}
}