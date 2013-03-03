using System;
using System.Collections.Generic;
using System.Threading;

namespace SimonWaite.Network.Protocols.Irc
{
	public class ChannelMonitorAgent : ISubscriptionHandler
	{
		private static readonly char[] splitOnSpace = new char[] { ' ' };
		Dictionary<string,Channel> channels = new Dictionary<string,Channel>();
		Dictionary<string,User> users = new Dictionary<string,User > ();

		Rfc1459 irc;

		public ChannelMonitorAgent (Rfc1459 irc)
		{
			this.irc = irc;
			// we probably don't want this at the moment. Ping ought to suffice.
			//t = new Timer (new TimerCallback (Tick), this, 120000, 1200000);
			irc.InputQueue.Register (this, "PING"); // we want pings to kick off occasional manual updates.
			irc.InputQueue.Register (this, "JOIN"); // we get a join message from the server for both ourselves and others.
			irc.InputQueue.Register (this, "352"); // handle answers to 'who #channel' 
		}

		void Tick (object state)
		{
			HandlePing (null);
		}

		#region ISubscriptionHandler implementation
		public void SubscriptionNotification (Rfc1459 context, IrcMessage message)
		{
			if (message.Command.CompareTo ("PING") == 0) {
				HandlePing(message);
			}

			if (message.Command.CompareTo ("JOIN") == 0) {
				HandleJoin(message);
			}

			if (message.Command.CompareTo ("PART") == 0 || message.Command.CompareTo ("KICK") == 0) {
				HandlePart(message);
			}

			if (message.Command.CompareTo ("MODE") == 0) {
				HandleMode(message);
			}

			if (message.Command.CompareTo ("352") == 0) {
				HandleWhoReply(message);
			}
		}
		#endregion

		void HandlePing (IrcMessage message)
		{
			// this may be null! Warning!
		}

		void HandleJoin(IrcMessage message)
		{
			// make sure the channel exists.
			// InputPump SMesg: 'nick!~ident@example.com JOIN #zzyyxx'

			string channel = irc.ToLowerCase(message.CommandArguments [0]);

			string nick = irc.ToLowerCase (message.Nick);
			User u = GetUser ( nick );
			GetChannel(channel).AddUser (u);
		}
		Channel GetChannel(string channel)
		{
			Channel c;
			if (!channels.ContainsKey (channel)) {
				c = new Channel (channel);
				channels.Add (channel, c);
				NewChannelDetected (channel);
			} else {
				c= channels[channel];
			}
			return c;
		}
		User GetUser(string nick)
		{
			return GetUser (nick, null);
		}
		User GetUser(string nick, IrcMessage message)
		{
			User u;
			if (!users.ContainsKey (nick)) {

				u = message != null ? new User (irc, message) : new User(irc);
				// but we don't know what the description is.
				users.Add (nick, u);
				// so we ask the server about them
				NewUserDetected (nick);
			} else {
				u = users [u];
			}

			return u;
		}

		void HandlePart(IrcMessage message)
		{
			// who part channel :text
			string nick = irc.ToLowerCase (message.Nick);
			string chan = irc.ToLowerCase(message.CommandArguments [0]);

			GetChannel(chan).RemoveUser(nick);

		}

		void HandleMode (IrcMessage message)
		{
			string target = message.CommandArguments [0];

		}
		//:port80b.se.quakenet.org 352 storpung #botty TheQBot CServe.quakenet.org *.quakenet.org Q H*@d :3 The Q Bot

		void HandleWhoReply (IrcMessage message)
		{
			string channel = message.CommandArguments [0];
			string ident = message.CommandArguments [1];
			string host = message.CommandArguments [2];
			string nick = irc.ToLowerCase(message.CommandArguments [3]);
			string modes = message.CommandArguments [4];
			string[] data = message.Data.Split (splitOnSpace, 2);
			string hops = data [0];
			string description = data [1];

			// find user.
			User u = GetUser(nick, new IrcMessage(":"+nick+"!"+ident+"@"+host+" * :*")); // dummy message
			u.Description = description;

			if (!channel.StartsWith ("*")) {
				Channel c = GetChannel(channel);
				c.GetUser(u);
			}
		}

		// :port80b.se.quakenet.org 352 storpung #botty TheQBot CServe.quakenet.org *.quakenet.org Q H*@d :3 The Q Bot
		void NewUserDetected (string nick)
		{
		}

		void NewChannelDetected (string channel)
		{
			irc.OutputQueue.Enqueue (new IrcMessage (null, "WHO " + channel));
		}
	}
}

