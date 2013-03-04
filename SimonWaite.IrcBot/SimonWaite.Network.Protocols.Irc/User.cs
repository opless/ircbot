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
		public bool IsInitialised
		{
			get { 
				return !(Nick == null || Ident == null || Host == null); // || Description == null); // 
			}
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

		public override string Dump (int i)
		{
			string prefix = new string (' ',i);
			string ret= string.Format("{0} User: {1} ({2}@{3})\n",prefix, Nick, Ident, Host );
			ret+=string.Format("{0} Desc: '{1}'\n",prefix,Description);
			ret+=base.Dump(i+2);
			return ret;
		}

		#region implemented abstract members of BasicModes

		static readonly string argumentModes = string.Empty; //
		static readonly string multipleArgumentModes = string.Empty; 
		
		
		public override void ParseObservedModes (string modes, string[] args)
		{
			Parse (observedModes, modes, args, argumentModes, multipleArgumentModes);
		}
		
		public override void ParseDesiredModes (string modes, string[] args)
		{
			Parse (desiredModes,modes, args, argumentModes, multipleArgumentModes);
		}
		
		public override void ParseInternalModes (string modes, string[] args)
		{
			throw new NotImplementedException ();
		}

		#endregion
	}


}

