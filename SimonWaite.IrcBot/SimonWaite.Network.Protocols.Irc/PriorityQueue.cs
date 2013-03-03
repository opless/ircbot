using System;
using System.ComponentModel;

namespace SimonWaite.Network.Protocols.Irc
{
	public class PriorityQueue<T> : INotifyPropertyChanged where T: class
	{
		object q = new object ();
		SynchronizedQueue<T> low = new SynchronizedQueue<T> ();
		SynchronizedQueue<T> medium = new SynchronizedQueue<T> ();
		SynchronizedQueue<T> high = new SynchronizedQueue<T> ();
		SynchronizedQueue<T> urgent = new SynchronizedQueue<T> ();

		public PriorityQueue ()
		{
		}
		
		public void Clear ()
		{
			lock (q) {
				low.Clear ();
				medium.Clear ();
				high.Clear ();
				urgent.Clear ();

				NotifyPropertyChanged ("Clear");
				// clear out subscribers.
				PropertyChanged = null;
			}
		}

		public void Enqueue (T element)
		{
			EnqueueMedium (element);
		}
		
		public T Dequeue ()
		{
			if (urgent.HasMore)
				return urgent.Dequeue ();
			if (high.HasMore)
				return high.Dequeue ();
			if (medium.HasMore)
				return medium.Dequeue ();
			if (low.HasMore)
				return low.Dequeue ();
			return null;
		}
		
		public bool HasMore {
			get {
				lock (q) {
					return (low.HasMore || medium.HasMore || high.HasMore || urgent.HasMore);
				}
			}
		}

		public T DequeueLow() {
			return low.Dequeue ();
		}
		public T DequeueMedium() {
			return medium.Dequeue ();
		}
		public T DequeueHigh() {
			return high.Dequeue ();
		}
		public T DequeueUrgent() {
			return urgent.Dequeue ();
		}

		
		public void EnqueueLow(T element){
			low.Enqueue (element);
			NotifyPropertyChanged ("Low");
		}
		
		public void EnqueueMedium(T element){
			medium.Enqueue (element);
			NotifyPropertyChanged ("Medium");
		}
		
		public void EnqueueHigh(T element){
			high.Enqueue (element);
			NotifyPropertyChanged ("High");
		}
		
		public void EnqueueUrgent(T element){
			urgent.Enqueue (element);
			NotifyPropertyChanged ("Urgent");
		}

		
		public event PropertyChangedEventHandler PropertyChanged;
		
		private void NotifyPropertyChanged (String info)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this, new PropertyChangedEventArgs (info));
			}
		}
	}
}

