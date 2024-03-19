using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using CassetteBuilds.Code.Models;

namespace CassetteBuilds.Code.Misc
{
	public static class TreeDataGridDisableSelection
	{
		// UnsafeAccessor does not support generics https://github.com/dotnet/runtime/issues/89439
		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_selection")]
		private extern static ref ITreeDataGridSelection? GetSet_selection(this FlatTreeDataGridSource<Monster> source);
		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_isSelectionSet")]
		private extern static ref bool GetSet_isSelectionSet(this FlatTreeDataGridSource<Monster> source);

		public static void DisableSelection(this FlatTreeDataGridSource<Monster> source)
		{
			source.GetSet_selection() = null;
			source.GetSet_isSelectionSet() = true;
		}
	}
}