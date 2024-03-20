using System;
using System.Buffers;

namespace Kryz.Pools
{
	public abstract class GenericPool<T>
	{
		private T[] array = ArrayPool<T>.Shared.Rent(10);
		private int count = 0;

		public int Count => count;

		public GenericPool(int initialSize = 0)
		{
			EnsureAmount(initialSize);
		}

		public T Get()
		{
			T item;
			if (count > 0)
			{
				item = array[count - 1];
				array[count - 1] = default!;
				count--;
			}
			else
			{
				item = Instantiate();
			}
			OnGet(ref item);
			return item;
		}

		public void Return(T item)
		{
			count++;
			EnsureCapacity(count);
			array[count - 1] = item;
			OnReturn(ref array[count - 1]);
		}

		public void Expand(int amount = 1)
		{
			EnsureCapacity(count + amount);

			for (int i = 0; i < amount; i++)
			{
				Return(Instantiate());
			}
		}

		public void EnsureAmount(int amount)
		{
			if (amount > count)
			{
				Expand(amount - count);
			}
		}

		private void EnsureCapacity(int capacity)
		{
			if (array.Length < capacity)
			{
				int newCapacity = Math.Max(capacity, array.Length * 2);
				T[] newArray = ArrayPool<T>.Shared.Rent(newCapacity);
				Array.Copy(array, newArray, count);
				Array.Clear(newArray, count, newArray.Length - count);
				ArrayPool<T>.Shared.Return(array, clearArray: true);
				array = newArray;
			}
		}

		protected abstract T Instantiate();

		protected virtual void OnGet(ref T obj) { }
		protected virtual void OnReturn(ref T obj) { }
	}
}