using System;
using System.Collections.Generic;

namespace SimonWaite.Network.Protocols.Irc
{
	public class ReadOnlyList<T> : IList<T>
	{
		IList<T> w;
		public ReadOnlyList ()
		{
			w = new List<T> (0);
		}

		public ReadOnlyList(IList<T> toWrap)
		{
			w = toWrap;
		}

		#region IList implementation

		public int IndexOf (T item)
		{
			return w.IndexOf (item);
		}

		public void Insert (int index, T item)
		{
			throw new NotSupportedException ();
		}

		public void RemoveAt (int index)
		{
			throw new NotSupportedException ();
		}

		public T this [int index] {
			get {
				return w[index];
			}
			set {
				throw new NotSupportedException ();
			}
		}

		#endregion

		#region ICollection implementation

		public void Add (T item)
		{
			throw new NotSupportedException ();
		}

		public void Clear ()
		{
			throw new NotSupportedException ();
		}

		public bool Contains (T item)
		{
			return w.Contains (item);
		}

		public void CopyTo (T[] array, int arrayIndex)
		{
			w.CopyTo (array, arrayIndex);
		}

		public bool Remove (T item)
		{
			throw new NotSupportedException ();
		}

		public int Count {
			get {
				return w.Count;
			}
		}

		public bool IsReadOnly {
			get {
				return true;
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<T> GetEnumerator ()
		{
			return w.GetEnumerator ();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			return w.GetEnumerator ();
		}

		#endregion
	}
}

