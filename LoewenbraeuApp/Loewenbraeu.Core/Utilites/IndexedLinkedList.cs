using System.Collections.Generic;

namespace Loewenbraeu.Core
{
	public class IndexedLinkedList<T>
	{

		LinkedList<T> data = new LinkedList<T>();
		Dictionary<T, LinkedListNode<T>> index = new Dictionary<T, LinkedListNode<T>>();

		public void Add(T value)
		{
			index[value] = data.AddLast(value);
		}

		public void RemoveFirst ()
		{
			if (data != null && data.Count > 0) {
				index.Remove (data.First.Value);
				data.RemoveFirst ();	
			}
			
		}

		public void Remove(T value)
		{
			LinkedListNode<T> node;
			if (index.TryGetValue(value, out node))
			{
				data.Remove(node);
				index.Remove(value);
			}
		}

		public int Count
		{
			get
			{
				return data.Count;
			}
		}

		public void Clear()
		{
			data.Clear();
			index.Clear();
		}

		public T First
		{
			get
			{
				return data.First.Value;
			}
		}
	}
}
