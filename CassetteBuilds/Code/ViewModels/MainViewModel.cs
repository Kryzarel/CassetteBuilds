using System.Collections.ObjectModel;
using Kryz.Pools;
using ReactiveUI;

namespace CassetteBuilds.ViewModels;

public class MainViewModel : ViewModelBase
{
	private class MonsterSearchViewModelPool(MainViewModel mainVM) : GenericPool<MonsterSearchViewModel>
	{
		private readonly MainViewModel mainVM = mainVM;
		private int NextNum => mainVM.Searches == null || mainVM.Searches.Count == 0 ? 1 : mainVM.Searches[^1].Number + 1;
		protected override MonsterSearchViewModel Instantiate() => new();
		protected override void OnGet(ref MonsterSearchViewModel searchVM) => searchVM.Number = NextNum;
		protected override void OnReturn(ref MonsterSearchViewModel searchVM) => searchVM.Clear();
	}

	private readonly MonsterSearchViewModelPool viewModelPool;

	public ObservableCollection<MonsterSearchViewModel> Searches { get; }

	private int _selectedIndex;
	public int SelectedIndex { get => _selectedIndex; set { BugWorkaround(_selectedIndex); this.RaiseAndSetIfChanged(ref _selectedIndex, value); } }

	public MainViewModel()
	{
		viewModelPool = new MonsterSearchViewModelPool(this);
		Searches = [viewModelPool.Get()];
	}

	public void AddSearch()
	{
		Searches.Add(viewModelPool.Get());
		SelectedIndex = Searches.Count - 1;
	}

	public void RemoveSearch(MonsterSearchViewModel viewModel)
	{
		int index = Searches.IndexOf(viewModel);
		if (index >= 0)
		{
			if (SelectedIndex == index)
			{
				SelectedIndex = index >= Searches.Count - 1 ? index - 1 : index + 1;
			}
			Searches.RemoveAt(index);
		}

		if (Searches.Count == 0)
		{
			AddSearch();
		}
	}

	// Workaround for AutoCompleteBox bug where it stops working when switching views if SearchText is not empty
	private void BugWorkaround(int index)
	{
		if (index >= 0 && index < Searches.Count)
		{
			MonsterSearchViewModel viewModel = Searches[index];
			if (viewModel.SearchText != null)
			{
				viewModel.SearchText = null;
			}
		}
	}
}