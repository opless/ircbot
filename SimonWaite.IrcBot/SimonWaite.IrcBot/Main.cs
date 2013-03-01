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
using SimonWaite.Network.Protocols.Irc;

namespace SimonWaite.IrcBot
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Log.D ("Starting up...");
			Rfc1459 context = new Rfc1459 ();
			context.Nick = "oplessBot";
			context.User = "ircbot";
			context.Description="irc bot";
			context.Server = "irc.quakenet.org";
			context.Port = 6667;
			
			Log.D ("Plugins...");
			var channels = new AutoJoinChannelsAgent(context,"#zzyyxx");

			Log.D ("Connecting...");
			context.Connect ();
			
			
			
			while(true)
			{
				Log.D ("Idle.");
				System.Threading.Thread.Sleep(15000);
			}
			
		}
	}
}
