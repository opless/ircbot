using System;
using System.Collections.Generic;

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
			userCollection.Add (u, new BasicModes());
		}
	}
}

