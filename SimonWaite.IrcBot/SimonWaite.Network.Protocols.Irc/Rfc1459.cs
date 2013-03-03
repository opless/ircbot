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
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Timers;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace SimonWaite.Network.Protocols.Irc
{
	public class Rfc1459
	{
		TcpClient connection;

		ChannelMonitorAgent channelAgent;

		public Rfc1459 ()
		{
			IsConnected = false;
			inputQueue = new IrcSubscriptionQueue (this);

			Log.D ("Initialising Default Agents");
			channelAgent = new ChannelMonitorAgent (this);
			IsRfc1459CaseMapping = true;
		}

		public string Server { get; set; }

		public int Port { get; set; }

		public string Nick { get; set; }

		public string User { get; set; }

		public string Password { get; set; }

		public string Description { get; set; }

		public bool IsConnected { get; private set; }

		public bool IsConnecting { get; private set; }

		Stream stream;
		BufferedInputPump readPump;
		IrcSubscriptionQueue inputQueue;
		PriorityQueue<IrcMessage> outputQueue = new PriorityQueue<IrcMessage> ();

		public IrcSubscriptionQueue InputQueue { get { return inputQueue; } }

		public PriorityQueue<IrcMessage> OutputQueue { get { return outputQueue; } }

		public void Connect ()
		{
			Log.D ("Connect");
			if (IsConnected || IsConnecting)
				return;


			Log.D ("Connecting to {0}:{1}", Server, Port);
			var client = new TcpClient ();
			client.BeginConnect (Server, Port, new AsyncCallback (ClientConnected), client);
		}

		internal void Output (IrcMessage m)
		{
			Output (m.ToString ());
		}

		internal void Output (string s)
		{
			Log.D ("OUTPUT: {0}", s);
			if (!s.EndsWith ("\r\n"))
				s += "\r\n";
			byte[] bBuff = Encoding.UTF8.GetBytes (s);
			stream.Write (bBuff, 0, bBuff.Length);
		}

		void ClientConnected (IAsyncResult ar)
		{
			Log.D ("ClientConnected");

			IsConnected = false;

			var client = ar.AsyncState as TcpClient;
			if (null == client) {
				return; // don't know how we got here, but it's best just to ignore it.
			}
			try {
				client.EndConnect (ar);
				IsConnected = true;
				IsConnecting = false;

				connection = client;
				stream = connection.GetStream ();

				readPump = new BufferedInputPump (stream, inputQueue, this);
				readPump.Kick ();
				
				// set up reading for Pong
				var pingPong = new ServerPingHandler(this);
				
				// set up Writer
				outputQueue.PropertyChanged += HandleWriteNotify;
				Log.D ("Logging in...");

				// we need to send a couple of important commands to initialise the connection
				// PASS <password>
				// NICK <nickname>
				// USER <username> <hostname> <servername> <realname>
				if (null != Password)
					Output ("PASS " + Password);
				Output ("NICK " + Nick);
				Output ("USER " + User + " 1 1 " + Description); // hostname and servername have been ignored for decades.


			} catch(Exception e) {
				TearDownConnection (e);
			}
		}

		long nextWriteSchedule = 0;
		readonly DateTime epoch = new DateTime (1970, 1, 1, 0, 0, 0, 0);

		void HandleWriteNotify (object sender, PropertyChangedEventArgs e)
		{
			var queue = sender as SynchronizedQueue<IrcMessage>;
			Log.D ("HandleWriteNotify: {0}", e.PropertyName);
			// if we're being torn down, don't write anything
			if (e.PropertyName.CompareTo ("Clear") == 0)
				return;

			//TODO: write better anti flood mechanism!
			//TODO: maybe a high and low priority queue?
			while (queue.HasMore) {
				long now = (long)(DateTime.UtcNow - epoch).TotalSeconds;

				if (nextWriteSchedule > now) {
					Thread.Sleep (500);
					continue;
				}
				nextWriteSchedule = now + 1;

				try { 
					Output (queue.Dequeue ().ToString ());
				} catch(Exception ex) {
					TearDownConnection (ex);
				}
			}
		}

		
		
		public void TearDownConnection(bool f)
		{
			TearDownConnection(null);
		}

		public void TearDownConnection (Exception reason)
		{
			Log.D ("Tearing Down connection.");
			if(null != reason)
				Log.D ("Reason: {0}",reason);

			IsRfc1459CaseMapping = true;

			this.inputQueue.Clear ();
			this.outputQueue.Clear ();
			try {
				this.connection.Close ();
			} catch {
			}
			this.stream = null;
			this.connection = null;
			IsConnected = false;
			IsConnecting = false;
		}

		public string ToLowerCase (string str)
		{
			/*
			 * the characters {}| are considered to be the 
			 * lower case equivalents of the characters []\,
			 * respectively
			 * 
			 * Unless, of course the server indicates:
			 * CASEMAPPING=ascii - then we may use normal rules. (yay for 005)
			 * 
			 * TODO: IMPLEMENT CASEMAPPING DETECTION
			 */
			string toLower = str.ToLowerInvariant ();
			if(IsRfc1459CaseMapping)
				toLower = toLower.Replace ('[', '{').Replace (']', '}').Replace ('\\', '|');
			return toLower;
		}

		public bool IsRfc1459CaseMapping { get; set; }
	}
}

