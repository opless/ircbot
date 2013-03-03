using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SimonWaite.Network.Protocols.Irc
{
	public class ReadOnlyDictionary<K,V> : INotifyPropertyChanged, IDictionary<K,V>
	{
		IDictionary<K,V> w;
		public ReadOnlyDictionary ()
		{
			w = new Dictionary<K,V> (0);
		}

		public ReadOnlyDictionary(Dictionary<K,V> toWrap)
		{
			w = toWrap;
		}

		#region IDictionary implementation
		public void Add (K key, V value)
		{
			throw new NotSupportedException ();
		}
		public bool ContainsKey (K key)
		{
			return w.ContainsKey (key);
		}
		public bool Remove (K key)
		{
			throw new NotSupportedException ();
		}
		public bool TryGetValue (K key, out V value)
		{
			return w.TryGetValue (key, out value);
		}
		public V this [K key] {
			get {
				return w[key];
			}
			set {
				throw new NotSupportedException ();
			}
		}
		public ICollection<K> Keys {
			get {
				return w.Keys;
			}
		}
		public ICollection<V> Values {
			get {
				return w.Values;
			}
		}
		#endregion
		#region ICollection implementation
		public void Add (KeyValuePair<K, V> item)
		{
			throw new NotSupportedException ();
		}
		public void Clear ()
		{
			throw new NotSupportedException ();
		}
		public bool Contains (KeyValuePair<K, V> item)
		{
			return w.Contains (item);
		}
		public void CopyTo (KeyValuePair<K, V>[] array, int arrayIndex)
		{
			w.CopyTo (array, arrayIndex);
		}
		public bool Remove (KeyValuePair<K, V> item)
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
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator ()
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


		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(String info)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}		
		#endregion
	}
}

