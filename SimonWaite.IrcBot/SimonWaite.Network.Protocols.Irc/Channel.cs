using System;
using System.Collections.Generic;
using System.Text;

namespace SimonWaite.Network.Protocols.Irc
{
	public class Channel : BasicModes
	{
		string name;


		Dictionary<User,BasicModes> userCollection = new Dictionary<User, BasicModes> ();
		ReadOnlyDictionary<User,BasicModes> roUserCollection = new ReadOnlyDictionary<User, BasicModes> ();


		public Channel (string channel)// : this(channel,null)
		{
			Name = channel;
			roUserCollection = new ReadOnlyDictionary<User, BasicModes> (userCollection);
		}

		public Channel(string channel, string key)
		{
			throw new NotImplementedException (); // not sure if this is the right thing to do.
		}

		static readonly string argumentModes = "bkov"; 
		static readonly string multipleArgumentModes = "bov"; 

		#region implemented abstract members of BasicModes
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

		public string Name {
			get {
				return name;
			}
			set {
				name = value;
				Changed("Name");
			}
		}

		string topic;
		public string Topic {
			get {
				return topic;
			}
			set {
				topic = value;
				Changed("Topic");
			}
		}

		string setBy;
		public string SetBy {
			get {
				return setBy;
			}
			set {
				setBy = value;
				Changed("SetBy");
			}
		}
		DateTime setAt;
		public DateTime SetAt {
			get {
				return setAt;
			}
			set {
				setAt = value;
				Changed("SetAt");
			}
		}

		public IDictionary<User,BasicModes> Users { get { return roUserCollection; } }

		public void RemoveUser (string nick)
		{
			User foundUser = null;
			// find it.

			foreach(User c in userCollection.Keys)
			{
				if(c.Nick.CompareTo(nick)==0)
				{
					foundUser=c;
				}
			}
			// remove it
			if (null != foundUser)
				userCollection.Remove (foundUser);
		}

		public BasicModes GetUserModes (User u)
		{
			if (!userCollection.ContainsKey (u))
				AddUser (u);
			return userCollection [u];
		}

		public void AddUser(User u)
		{
			if (userCollection.ContainsKey (u))
				return;
			userCollection.Add (u, new UserChannelModes());
		}

		public string Dump(int indent)
		{
			string prefix = new string (' ', indent);
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("{0}Channel:\n", prefix);
			sb.AppendFormat ("{0}     Name.: '{1}'\n", prefix,Name);
			sb.AppendFormat ("{0}     Topic: '{1}'\n", prefix,Topic); 
			sb.AppendFormat ("{0}     SetBy: '{1}'\n", prefix,SetBy); 
			sb.AppendFormat ("{0}     SetAt: '{1}'\n", prefix,SetAt); 
			sb.AppendFormat ("{0}     Users: '{1}'\n", prefix, userCollection.Count);
			List<User> keys = new List<User> (userCollection.Keys);
			for (int i=0; i<keys.Count; i++) {
				sb.AppendFormat ("{0}       user:{1}\n", prefix, i);
				sb.AppendLine(keys[i].Dump(indent+10));
				sb.AppendFormat ("{0}       modes:{1}\n", prefix, i);
				sb.AppendLine(userCollection[keys[i]].Dump(indent+10));
			}
			sb.AppendFormat ("{0}     ChannelModes:\n",prefix);
			sb.Append (base.Dump (indent+10));
			return sb.ToString ();
		}
	}
}

