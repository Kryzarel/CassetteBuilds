using System.Collections.Generic;
using System.ComponentModel;

namespace CassetteBuildsUI.Models
{
	public class MoveToSearch : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public readonly int Index;

		public int Number => Index + 1;
		public IReadOnlyList<string> MoveNames { get; }

		private string? move;
		public string? Move
		{
			get => move;
			set
			{
				move = value;
				PropertyChanged?.Invoke(this, moveChangedArgs);
			}
		}

		// Reduce allocations by having a single instance of this. It's essentially read-only, so it should be ok.
		private static readonly PropertyChangedEventArgs moveChangedArgs = new(nameof(Move));

		public MoveToSearch(int index, IReadOnlyList<string> moves)
		{
			Index = index;
			MoveNames = moves;
		}
	}
}