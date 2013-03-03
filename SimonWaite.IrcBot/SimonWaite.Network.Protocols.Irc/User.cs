using System;
using System.Collections.Generic;

namespace SimonWaite.Network.Protocols.Irc
{
	public class User : BasicModes
	{
		string nick;
		string ident;
		string host;
		string description;

		public User()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SimonWaite.Network.Protocols.Irc.User"/> class.
		/// Uses the details in the 'From' portion of the message to create it.
		/// Doesn't set description though. You'll have to acquire it yourself.
		/// </summary>
		/// <param name="irc">Irc.</param>
		/// <param name="message">Message.</param>
		public User(Rfc1459 irc, IrcMessage message)
		{
			Nick = irc.ToLowerCase (message.Nick);
			Ident = message.Ident;
			Host = message.Host;

		}

		public string Nick {
			get {
				return nick;
			}
			set {
				nick = value;
				Changed("Nick");
			}
		}
		public string Ident {
			get {
				return ident;
			}
			set {
				ident = value;
				Changed("Ident");
			}
		}
		public string Host {
			get {
				return host;
			}
			set {
				host = value;
				Changed("Host");
			}
		}
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
				Changed("Description");
			}
		}

	}


}

