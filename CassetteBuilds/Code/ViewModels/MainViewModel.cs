using System.Collections.ObjectModel;
using ReactiveUI;

namespace CassetteBuilds.ViewModels;

public class MainViewModel : ViewModelBase
{
	private int _selectedIndex;
	public int SelectedIndex
	{
		get => _selectedIndex;
		set
		{
			AutoCompleteBoxBugWorkaround(SelectedIndex);
			this.RaiseAndSetIfChanged(ref _selectedIndex, value);
		}
	}

	public ObservableCollection<MonsterSearchViewModel> Searches { get; }

	public MainViewModel()
	{
		Searches = [new MonsterSearchViewModel()];
	}

	public void AddSearch()
	{
		int nextNum = Searches.Count <= 0 ? 1 : Searches[^1].Number + 1;
		Searches.Add(new MonsterSearchViewModel(nextNum));
		SelectedIndex = Searches.Count - 1;
	}

	public void RemoveSearch(MonsterSearchViewModel viewModel)
	{
		int index = Searches.IndexOf(viewModel);
		if (index >= 0)
		{
			if (SelectedIndex == index && index > 0)
			{
				SelectedIndex = index - 1;
			}
			Searches.RemoveAt(index);
		}
	}

	// Workaround for AutoCompleteBox bug where it stops working when switching views if SearchText is not empty
	private void AutoCompleteBoxBugWorkaround(int index)
	{
		if (index > 0 && index < Searches.Count)
		{
			MonsterSearchViewModel viewModel = Searches[index];
			if (viewModel.SearchText != null)
			{
				viewModel.SearchText = null;
			}
		}
	}
}