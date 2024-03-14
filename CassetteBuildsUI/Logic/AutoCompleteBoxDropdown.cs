using System;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Controls;

namespace CassetteBuildsUI.Logic
{
	public static class AutoCompleteBoxDropdown
	{
		private static readonly PropertyInfo? textBoxProperty = typeof(AutoCompleteBox).GetProperty("TextBox", BindingFlags.Instance | BindingFlags.NonPublic);
		private static readonly MethodInfo? populateDropDownMethod = typeof(AutoCompleteBox).GetMethod("PopulateDropDown", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly MethodInfo? openingDropDownMethod = typeof(AutoCompleteBox).GetMethod("OpeningDropDown", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly FieldInfo? ignorePropertyChangeField = typeof(AutoCompleteBox).GetField("_ignorePropertyChange", BindingFlags.NonPublic | BindingFlags.Instance);
		private static readonly object[] populateDropDownArgs = [null!, EventArgs.Empty];
		private static readonly object[] openingDropDownArgs = [false];

		public static void AddDropdownInteraction(this AutoCompleteBox autoCompleteBox)
		{
			Task.Delay(10).ContinueWith(_ => Avalonia.Threading.Dispatcher.UIThread.Invoke(autoCompleteBox.AddDropdownButton));
		}

		private static void AddDropdownButton(this AutoCompleteBox autoCompleteBox)
		{
			if (textBoxProperty?.GetValue(autoCompleteBox) is not TextBox tb)
				return;

			autoCompleteBox.KeyUp += (sender, args) =>
			{
				if (args.Key == Avalonia.Input.Key.Down || args.Key == Avalonia.Input.Key.F4)
				{
					if (string.IsNullOrEmpty(autoCompleteBox.Text))
					{
						autoCompleteBox.ShowDropdown();
					}
				}
			};

			Button button = new()
			{
				Content = "⮟",
				Margin = new Avalonia.Thickness(0,3,3,3),
				ClickMode = ClickMode.Press
			};
			button.Click += (s, e) => autoCompleteBox.ShowDropdown();
			tb.InnerRightContent = button;
		}

		private static void ShowDropdown(this AutoCompleteBox autoCompleteBox)
		{
			if (!autoCompleteBox.IsDropDownOpen)
			{
				populateDropDownArgs[0] = autoCompleteBox;
				populateDropDownMethod?.Invoke(autoCompleteBox, populateDropDownArgs);
				openingDropDownMethod?.Invoke(autoCompleteBox, openingDropDownArgs);

				if (!autoCompleteBox.IsDropDownOpen)
				{
					//We *must* set the field and not the property as we need to avoid the changed event being raised (which prevents the dropdown opening).
					if (ignorePropertyChangeField?.GetValue(autoCompleteBox) is bool val && !val)
						ignorePropertyChangeField?.SetValue(autoCompleteBox, true);

					autoCompleteBox.SetCurrentValue(AutoCompleteBox.IsDropDownOpenProperty, true);
				}
			}
		}
	}
}