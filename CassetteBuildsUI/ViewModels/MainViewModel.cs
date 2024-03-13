using System.Collections.ObjectModel;

namespace CassetteBuildsUI.ViewModels;

public class MainViewModel : ViewModelBase
{
	public ObservableCollection<MonsterSearchViewModel> Searches { get; }

	public MainViewModel()
	{
		Searches = [new MonsterSearchViewModel()];
	}
}