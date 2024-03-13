using System.Collections.ObjectModel;
using CassetteBuildsUI.Models;

namespace CassetteBuildsUI.ViewModels;

public class MainViewModel : ViewModelBase
{
	public ObservableCollection<MonsterSearch> Searches { get; }

	public MainViewModel()
	{
		Searches = [new MonsterSearch()];
	}
}