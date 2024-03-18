using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;

namespace CassetteBuilds.Logic
{
	public static class AutoCompleteBoxDropdown
	{
		// OP way to access private members without reflection. Available only in .NET 8 and above.
		// Way faster than reflection and compatible with AOT(?)
		// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafeaccessorattribute
		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "get_TextBox")]
		extern static TextBox GetTextBox(this AutoCompleteBox autoCompleteBox);
		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "PopulateDropDown")]
		extern static void PopulateDropDown(this AutoCompleteBox autoCompleteBox, object? sender, EventArgs args);
		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "OpeningDropDown")]
		extern static void OpeningDropDown(this AutoCompleteBox autoCompleteBox, bool value);
		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_ignorePropertyChange")]
		extern static ref bool RefGet_ignorePropertyChange(this AutoCompleteBox autoCompleteBox);

		// Cache this delegate just because we can. No need to allocate it each time.
		private static readonly EventHandler<KeyEventArgs> showDropdownOnKeyUpAction = ShowDropdownOnKeyUp;

		public static void AddDropdownInteraction(this AutoCompleteBox autoCompleteBox)
		{
			Task.Delay(10).ContinueWith(_ => Dispatcher.UIThread.Invoke(autoCompleteBox.AddDropdownButton));
		}

		private static void AddDropdownButton(this AutoCompleteBox autoCompleteBox)
		{
			// Use KeyUp because AutoCompleteBox sometimes eats KeyDown events
			autoCompleteBox.KeyUp += showDropdownOnKeyUpAction;

			TextBox tb = autoCompleteBox.GetTextBox();
			Button button = new()
			{
				Content = "⮟",
				Margin = new Thickness(0, 3, 3, 3),
				ClickMode = ClickMode.Press
			};
			button.Click += (s, e) => autoCompleteBox.ShowDropdown();
			tb.InnerRightContent = button;
		}

		private static void ShowDropdownOnKeyUp(object? sender, KeyEventArgs args)
		{
			if (args.Key is Key.Down or Key.F4 && sender is AutoCompleteBox box && string.IsNullOrEmpty(box.Text))
			{
				box.ShowDropdown();
			}
		}

		private static void ShowDropdown(this AutoCompleteBox autoCompleteBox)
		{
			if (!autoCompleteBox.IsDropDownOpen)
			{
				autoCompleteBox.PopulateDropDown(autoCompleteBox, EventArgs.Empty);
				autoCompleteBox.OpeningDropDown(false);

				if (!autoCompleteBox.IsDropDownOpen)
				{
					//We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
					autoCompleteBox.RefGet_ignorePropertyChange() = true;
					autoCompleteBox.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
				}
			}
		}
	}
}