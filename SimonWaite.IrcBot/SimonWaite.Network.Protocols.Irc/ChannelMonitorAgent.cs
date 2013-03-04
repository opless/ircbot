using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

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
			Log.D ("ChannelMonitorAgent::ctor");

			// we probably don't want this at the moment. Ping ought to suffice.
			//t = new Timer (new TimerCallback (Tick), this, 120000, 1200000);
			irc.InputQueue.Register (this, "PING"); // we want pings to kick off occasional manual updates.
			irc.InputQueue.Register (this, "JOIN"); // we get a join message from the server for both ourselves and others.
			irc.InputQueue.Register (this, "352"); // handle answers to 'who #channel' 
			irc.InputQueue.Register (this, "315"); // handle  end answers to 'who #channel' 
		}

		void Tick (object state)
		{
			HandlePing (null);
		}

		#region ISubscriptionHandler implementation
		public void SubscriptionNotification (Rfc1459 context, IrcMessage message)
		{
			Log.D ("ChannelMonitorAgent::SubscriptionNotification");
			if (message.Command.CompareTo ("PING") == 0) {
				Log.D ("** PING");
				HandlePing(message);
				return;
			}

			if (message.Command.CompareTo ("JOIN") == 0) {
				Log.D ("** JOIN");
				HandleJoin(message);
				return;
			}

			if (message.Command.CompareTo ("PART") == 0 || message.Command.CompareTo ("KICK") == 0) {
				Log.D ("** PART");
				HandlePart(message);
				return;
			}

			if (message.Command.CompareTo ("MODE") == 0) {
				Log.D ("** MODE");
				HandleMode(message);
				return;
			}

			if (message.Command.CompareTo ("352") == 0) {
				Log.D ("** 352");
				HandleWhoReply(message);
				return;
			}

			if (message.Command.CompareTo ("315") == 0) {
				Log.D ("** 315");
				string str = Dump(0);
				Log.D ("dumped ["+Dump (0)+"]");
				return;
			}

			Log.D ("** OOPS **");
		}
		#endregion

		void HandlePing (IrcMessage message)
		{
			// this may be null! Warning!
			Log.D ("PING:\n\n",Dump (0));
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

				u = message != null ? new User (irc, message) : new User() { Nick = nick };
				// but we don't know what the description is.
				users.Add (nick, u);
				// so we ask the server about them
				NewUserDetected (nick);
			} else {
				u = users[nick];
				if(!u.IsInitialised)
				{
					User q = message != null ? new User (irc, message) : null;
					if(null != q)
					{
						u.Host = q.Host;
						u.Ident = q.Ident;
					}
				}
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
		//                               0        1      2          3                 4           5  6    
		void HandleWhoReply (IrcMessage message)
		{
			string channel = message.CommandArguments [1];
			string ident = message.CommandArguments [2];
			string host = message.CommandArguments [3];
			string nick = irc.ToLowerCase(message.CommandArguments [5]);
			string modes = message.CommandArguments [6];
			string[] data = message.Data.Split (splitOnSpace, 2);
			string hops = data [0];
			string description = data [1];

			// find user.
			User u = GetUser(nick, new IrcMessage(":"+nick+"!"+ident+"@"+host+" * :*")); // dummy message
			u.Description = description;

			if (!channel.StartsWith ("*")) {
				Channel c = GetChannel(channel);
				BasicModes ucmodes= c.GetUserModes(u);
				ucmodes.ParseObservedModes(modes,null);
			}
		}

		// :port80b.se.quakenet.org 352 storpung #botty TheQBot CServe.quakenet.org *.quakenet.org Q H*@d :3 The Q Bot
		void NewUserDetected (string nick)
		{
			// TODO
		}

		void NewChannelDetected (string channel)
		{
			irc.OutputQueue.Enqueue (new IrcMessage (null, "WHO " + channel));
		}

		public string Dump(int indent)
		{
			string prefix = new string (' ', indent);
			string ret = string.Format ("{0}ChannelMonitorAgent:\n{0}  Channels:\n{1}\n{0}  Users:\n{2}\n\n", prefix, ChannelsDump (indent+2), UsersDump (indent+2));
			return ret;
		}

		string ChannelsDump (int indent)
		{
			string prefix = new string (' ', indent);
			StringBuilder sb = new StringBuilder ();
			List<string> keys = new List<string> (channels.Keys);
			for (int i=0; i<keys.Count; i++) {
				sb.AppendFormat ("{0}Channel:{1}\n", prefix, i);
				sb.AppendLine(channels[keys[i]].Dump(indent+4));
			}
			return sb.ToString ();		}

		string UsersDump (int indent)
		{
			string prefix = new string (' ', indent);
			StringBuilder sb = new StringBuilder ();
			List<string> keys = new List<string> (users.Keys);
			for (int i=0; i<keys.Count; i++) {
				sb.AppendFormat ("{0}User:{1}\n", prefix, i);
				sb.AppendLine(users[keys[i]].Dump(indent+4));
			}
			return sb.ToString ();
		}
	}
}

