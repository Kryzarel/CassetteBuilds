using System.Collections.ObjectModel;

namespace CassetteBuildsUI.ViewModels;

public class MainViewModel : ViewModelBase
{
	public ObservableCollection<MonstersSearchViewModel> Searches { get; }

	public MainViewModel()
	{
		Searches = [new MonstersSearchViewModel()];
	}
}