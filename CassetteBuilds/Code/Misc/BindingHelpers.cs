using System;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

namespace CassetteBuilds.Code.Misc
{
	public static class Bind
	{
		public static CompiledBindingExtension Command<TValue>(string name, Action<TValue?> execute, Func<TValue?, bool>? canExecute = null)
		{
			Action<object, object?> executeHelper = (o, p) => execute((TValue?)p);
			Func<object, object?, bool>? canExecuteHelper = canExecute == null ? null : (o, p) => canExecute((TValue?)p);

			CompiledBindingPathBuilder builder = new();
			CompiledBindingPath path = builder.Command(name, executeHelper, canExecuteHelper, null).Build();
			return new CompiledBindingExtension(path);
		}

		public static CompiledBindingExtension Command<TValue, TReturn>(string name, Func<TValue?, TReturn> execute, Func<TValue?, bool>? canExecute = null)
		{
			Action<object, object?> executeHelper = (o, p) => execute((TValue?)p);
			Func<object, object?, bool>? canExecuteHelper = canExecute == null ? null : (o, p) => canExecute((TValue?)p);

			CompiledBindingPathBuilder builder = new();
			CompiledBindingPath path = builder.Command(name, executeHelper, canExecuteHelper, null).Build();
			return new CompiledBindingExtension(path);
		}
	}

	public static class Bind<TObj>
	{
		public static CompiledBindingExtension Property<TValue>(string name, Func<TObj, TValue?> getter, Action<TObj, TValue?>? setter = null)
		{
			Func<object, object?>? get = o => getter((TObj)o);
			Action<object, object?>? set = setter == null ? null : (o, v) => setter((TObj)o, (TValue?)v);

			ClrPropertyInfo propertyInfo = new(name, get, set, typeof(TValue));
			CompiledBindingPathBuilder builder = new();
			CompiledBindingPath path = builder.Property(propertyInfo, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build();
			return new CompiledBindingExtension(path);
		}

		public static CompiledBindingExtension Command<TValue>(string name, Action<TObj, TValue?> execute, Func<TObj, TValue?, bool>? canExecute = null)
		{
			Action<object, object?> executeHelper = (o, p) => execute((TObj)o, (TValue?)p);
			Func<object, object?, bool>? canExecuteHelper = canExecute == null ? null : (o, p) => canExecute((TObj)o, (TValue?)p);

			CompiledBindingPathBuilder builder = new();
			CompiledBindingPath path = builder.Command(name, executeHelper, canExecuteHelper, null).Build();
			return new CompiledBindingExtension(path);
		}
	}
}