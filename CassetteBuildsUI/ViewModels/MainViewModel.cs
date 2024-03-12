using System.Collections.Generic;
using System.Collections.ObjectModel;
using CassetteBuilds.Code.Core;
using CassetteBuilds.Code.Data;
using CassetteBuildsUI.Models;

namespace CassetteBuildsUI.ViewModels;

public class MainViewModel : ViewModelBase
{
	public ObservableCollection<MoveToSearch> MovesToSearch { get; }
	public ReadOnlyCollection<string> MoveNames { get; }

	public MainViewModel()
	{
		List<string> moveNames = new(Database.Moves.Length);
		foreach (Move move in Database.Moves.Span)
		{
			moveNames.Add(move.Name);
		}
		MoveNames = new ReadOnlyCollection<string>(moveNames);

		List<MoveToSearch> movesToSearch = new(8);
		for (int i = 0; i < 8; i++)
		{
			movesToSearch.Add(new(i, MoveNames));
		}
		MovesToSearch = new ObservableCollection<MoveToSearch>(movesToSearch);
	}
}