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
  public interface ISubscriptionHandler
  {
    void SubscriptionNotification(Rfc1459 context, IrcMessage message);
  }
}

