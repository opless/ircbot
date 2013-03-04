//
//  Author:
//    simon simon@simonwaite.com
//
//  Copyright (c) 2013, Simon Waite
//
//  All rights reserved.
//
//
using System;
using System.Collections.Generic;
using System.Threading;

namespace SimonWaite.Network.Protocols.Irc
{
	public class IrcSubscriptionQueue
	{
		SynchronizedQueue<IrcMessage> queue = new SynchronizedQueue<IrcMessage> ();
		Dictionary<string, List<ISubscriptionHandler>> subscribers = new Dictionary<string, List<ISubscriptionHandler>> ();
		Timer timer;
		Rfc1459 irc;
		int score;

		public IrcSubscriptionQueue (Rfc1459 irc)
		{
			this.irc = irc;
			queue.PropertyChanged += QueueChanged;
			// timer = new Timer(new TimerCallback(Tick),this,100,Timeout.Infinite);
		}
		/*
     * This doesn't go here!
     * 
    void Tick(object state)
    {
      if (queue.HasMore)
      {
        irc.Output(queue.Dequeue);
        if(score < 8)
          score++;

      } else
      {
        score --;
      }

      int dueTime = 150 * (1 << score);
      timer.Change(dueTime,Timeout.Infinite);
    }
    */

		void QueueChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// abort on cleardown.
			if (e.PropertyName.CompareTo ("Clear") == 0)
				return;
			while (queue.HasMore) {
				var message = queue.Dequeue ();
				var interest = message.Command;

				// TODO: make interest more pattern-matchy, and allow whole message matching
				if (subscribers.ContainsKey (interest)) {
					foreach (var subscriber in subscribers[interest]) {
						try {
							subscriber.SubscriptionNotification (irc, message);
						} catch(Exception ex) {
							Log.D ("*** Exception: interest: '{0}' by {1}",interest,subscriber.GetType().FullName);
							Log.D (ex.ToString());
							Log.D("*** End Exception ***");
						}
					}
				}

			}
		}

		public void Register (ISubscriptionHandler who, string interest)
		{
			Log.D ("Registering interest '{0}' for {1}#{2:X}", interest, who.GetType ().FullName, who.GetHashCode ());
			if (!subscribers.ContainsKey (interest)) {
				subscribers.Add (interest, new List<ISubscriptionHandler> ());
			}
			if (subscribers [interest].Contains (who)) {
				throw new IndexOutOfRangeException ("Duplicate Interest Registered: Interest='" + interest + "', by:  " + who.GetType ().FullName + ", #" + who.GetHashCode ().ToString ("X"));
			}

			// so if we're here, we can safely add.
			subscribers [interest].Add (who);

		}

		public bool DeRegister (ISubscriptionHandler who, string interest)
		{
			if (!subscribers.ContainsKey (interest))
				return false;
			if (subscribers [interest].Contains (who))
				return false;

			subscribers [interest].Remove (who);
			return true;
		}

		public void Enqueue (IrcMessage message)
		{
			queue.Enqueue (message);
		}

		public void Clear ()
		{
			queue.Clear ();
			// put this back
			queue.PropertyChanged += QueueChanged;

		}

	}
}

