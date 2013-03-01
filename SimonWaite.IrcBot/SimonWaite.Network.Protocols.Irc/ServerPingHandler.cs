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

namespace SimonWaite.Network.Protocols.Irc
{
	public class ServerPingHandler : ISubscriptionHandler
	{
		Rfc1459 irc;

		public ServerPingHandler (Rfc1459 irc)
		{
			this.irc = irc;
			irc.InputQueue.Register (this, "PING");
		}




		#region ISubscriptionHandler implementation
		public void SubscriptionNotification (Rfc1459 context, IrcMessage message)
		{
			Log.D("PINGPONG IN.: {0}",message.ToDebugString());
			var reply = new IrcMessage(message.Data,"PONG");
			
			Log.D("PINGPONG OUT: {0}",reply.ToDebugString());
			Log.D("PINGPONG str: {0}",reply.ToString());
			context.OutputQueue.Enqueue( reply );
		}
		#endregion
	}
}

