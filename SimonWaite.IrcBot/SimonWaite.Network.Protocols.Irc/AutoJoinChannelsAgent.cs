using System;
using System.Collections.Generic;
using System.Threading;

namespace SimonWaite.Network.Protocols.Irc
{
	public class AutoJoinChannelsAgent :ISubscriptionHandler
	{
		object lockObj = new object();
		Rfc1459 irc;
		List<string> channels = new List<string> ();
		Timer t;

		public AutoJoinChannelsAgent (Rfc1459 irc, params string[] channels)
		{
			this.irc = irc;
			this.channels.AddRange (channels);
			t = new Timer (new TimerCallback (Tick), this, 1000, 60000);
			irc.InputQueue.Register(this,"PING"); // server ping
			irc.InputQueue.Register(this,"376");  // motd end
			irc.InputQueue.Register(this,"422"); // motd doesn't exist
		}
		
		private void Tick (object state)
		{
			foreach (var channel in channels) {
				// be thick and just join them (NB. No key support)
				// TODO Key Support
				// TODO Priority OutputQueue Support
				irc.OutputQueue.Enqueue(new IrcMessage(null,"JOIN",channel));
			}
		}
		
		public void Add (string channel)
		{
			lock (lockObj) {
				if (channels.Contains (channel))
					return;
				channels.Add (channel);
			}
		}
		
		public void Add (IEnumerable<string> channels)
		{
			lock (lockObj) {

				foreach (var channel in channels) {	
					Add (channel); 
				}
			}
		}
		
		public void Remove (string channel)
		{
			lock (lockObj) {

				if (channels.Contains (channel))
					channels.Remove (channel);
			}
		}

		public void Remove (IEnumerable<string> channels)
		{
			lock (lockObj) {

				foreach (var channel in channels) {	
					Remove (channel); 
				}
			}
		}
		
		public void SubscriptionNotification (Rfc1459 context, IrcMessage message)
		{
			// blindly join.
			this.Tick(this);
		}
	}
}

