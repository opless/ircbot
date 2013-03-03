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
using System.Text;

namespace SimonWaite.Network.Protocols.Irc
{
	public class IrcMessage
	{
		static readonly char[] splitOnColon = new char[]{ ':' };
		static readonly char[] splitOnSpace = new char[]{ ' ' };
		static readonly char[] splitOnBang = new char[]{ '!' };
		static readonly char[] splitOnAt = new char[]{ '@' };

		public IrcMessage ()
		{
		}
		
		
		public IrcMessage (string data, string cmd, params string[] args)
		{
			Command = cmd;
			CommandArguments = args;
			Data = data; // arg organisation is a bit mental here because I'm using params :/
		}

		public IrcMessage (string rawIrc)
		{
			bool serverMsg = rawIrc.StartsWith (":");
			string[] components = rawIrc.Split (splitOnColon, serverMsg ? 2 : 2, StringSplitOptions.RemoveEmptyEntries);
//			foreach (var x in components) {
//				Console.WriteLine (": >> [" + x + "]");
//			}
			// server message.
			if (serverMsg) {
				string[] fromAndCommand = components [0].Split (splitOnSpace, StringSplitOptions.RemoveEmptyEntries);
///				foreach (var x in fromAndCommand) {
//					Console.WriteLine ("$ >> [" + x + "]");
//				}
				From = fromAndCommand [0]	;
				Command = fromAndCommand [1];
				if (fromAndCommand.Length > 2) {
					CommandArguments = new string[fromAndCommand.Length - 2];
					Array.ConstrainedCopy (fromAndCommand, 2, CommandArguments, 0, CommandArguments.Length);
				}
			} else {
				string[] cmdAndArgs = components[0].Split (splitOnSpace, 2, StringSplitOptions.RemoveEmptyEntries);
//				foreach (var x in cmdAndArgs) {
//					Console.WriteLine ("# >> [" + x + "]");
//				}
				From = null;
				Command = cmdAndArgs [0];
				if (cmdAndArgs.Length > 1) {
					CommandArguments = new string[cmdAndArgs.Length - 1];
					Array.ConstrainedCopy (cmdAndArgs, 1, CommandArguments, 0, CommandArguments.Length);
				}
			}
			if (components.Length > 1) {
				Data = components [1];
			}
		}
		// :User!~ident@host.example.com
		public string Nick { get { return From.Split(splitOnBang)[0]; } }
		public string Ident { get { return From.Split(splitOnAt)[0].Split(splitOnBang)[1]; } }
		public string Host { get { return From.Split (splitOnAt)[1]; } }

		// :User!~ident@host.example.com PRIVMSG #channel :thought that may surface
		//  <-------------------From---> <Cmnd-> <--arg->  <----- data ----------->
		public string From { get; private set; }

		public string Command { get; private set; }

		public string[] CommandArguments { get; set; }

		public string Data { get; set; }

		public override string ToString ()
		{
			string ret = String.Empty;
			if (null != From) {
				ret += From;
			}
			if (null != Command) {
				ret += " " + Command;
			}
			if (null != CommandArguments && CommandArguments.Length > 0) {
				foreach (string arg in CommandArguments) {
					ret += " " + arg;
				}
			}
			if (null != Data) {
				ret += " :" + Data;
			}
			return ret;
		}
	
		public  string ToDebugString ()
		{
			return string.Format ("[IrcMessage: From={0}, Command={1}, CommandArguments={2}, Data={3}]", From, Command, CommandArguments, Data);
		}
	}
}

