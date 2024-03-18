using System;
using System.Windows.Input;

namespace CassetteBuilds.Logic
{
	public class Command : ICommand
	{
		public event EventHandler? CanExecuteChanged;

		private readonly Action<object?> execute;
		private readonly Func<object?, bool>? canExecute;

		public Command(Action<object?> execute, Func<object?, bool>? canExecute = null, EventHandler? canExecuteChanged = null)
		{
			this.execute = execute;
			this.canExecute = canExecute;
			canExecuteChanged += InvokeCanExecuteChanged;
		}

		public bool CanExecute(object? parameter)
		{
			return canExecute?.Invoke(parameter) ?? true;
		}

		public void Execute(object? parameter)
		{
			execute?.Invoke(parameter);
		}

		private void InvokeCanExecuteChanged(object? sender, EventArgs args)
		{
			CanExecuteChanged?.Invoke(sender, args);
		}
	}
}