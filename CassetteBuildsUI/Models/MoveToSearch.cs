using System.Collections.ObjectModel;

namespace CassetteBuildsUI.Models
{
	public class MoveToSearch
	{
		public readonly int Index;

		public int Number => Index + 1;
		public string? Move { get; set; }
		public ReadOnlyCollection<string> MoveNames { get; init; }

		public MoveToSearch(int index, ReadOnlyCollection<string> moveNames)
		{
			Index = index;
			MoveNames = moveNames;
		}
	}
}