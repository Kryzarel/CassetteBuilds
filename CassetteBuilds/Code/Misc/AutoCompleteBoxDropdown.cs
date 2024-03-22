using System;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace CassetteBuilds.Code.Misc
{
	public static class AutoCompleteBoxDropdown
	{
		// OP way to access private members without reflection. Available only in .NET 8 and above.
		// Way faster than reflection and compatible with AOT(?)
		// https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafeaccessorattribute
		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_textBox")]
		private extern static ref TextBox GetTextBox(this AutoCompleteBox autoCompleteBox);
		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "PopulateDropDown")]
		private extern static void PopulateDropDown(this AutoCompleteBox autoCompleteBox, object? sender, EventArgs args);
		[UnsafeAccessor(UnsafeAccessorKind.Method, Name = "OpeningDropDown")]
		private extern static void OpeningDropDown(this AutoCompleteBox autoCompleteBox, bool value);
		[UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_ignorePropertyChange")]
		private extern static ref bool RefGet_ignorePropertyChange(this AutoCompleteBox autoCompleteBox);

		// Cache delegates to reduce allocations
		private static readonly EventHandler addDropdownHandler = AddDropdown;
		private static readonly EventHandler<KeyEventArgs> showDropdownOnKeyUpHandler = ShowDropdownOnKeyUp;

		public static void AddDropdown(this AutoCompleteBox autoCompleteBox)
		{
			if (!autoCompleteBox.HasDropdown() && !autoCompleteBox.TryAddDropdown())
			{
				autoCompleteBox.LayoutUpdated += addDropdownHandler;
			}
		}

		private static void AddDropdown(object? sender, EventArgs args)
		{
			if (sender is AutoCompleteBox autoCompleteBox && autoCompleteBox.TryAddDropdown())
			{
				autoCompleteBox.LayoutUpdated -= addDropdownHandler;
			}
		}

		private static bool HasDropdown(this AutoCompleteBox autoCompleteBox)
		{
			return autoCompleteBox.GetTextBox()?.InnerRightContent is Button;
		}

		private static bool TryAddDropdown(this AutoCompleteBox autoCompleteBox)
		{
			TextBox textBox = autoCompleteBox.GetTextBox();
			if (textBox is null || textBox.InnerRightContent is Button) return false;

			// Use KeyUp because AutoCompleteBox sometimes eats KeyDown events
			autoCompleteBox.KeyUp += showDropdownOnKeyUpHandler;

			Button button = new()
			{
				Content = "▼",
				Margin = new Thickness(0, 3, 3, 3),
				VerticalAlignment = VerticalAlignment.Stretch,
				FontSize = (int)(autoCompleteBox.FontSize * 0.6),
				ClickMode = ClickMode.Press
			};
			button.Click += (s, e) => autoCompleteBox.ShowDropdown();
			textBox.InnerRightContent = button;
			autoCompleteBox.UpdateLayout(); // Force the control to update, otherwise it might not show the button
			return true;
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