using System;

namespace SimonWaite.Network.Protocols.Irc
{
	public class Log
	{
		static public void D(string msg) {
			Console.WriteLine(msg);
		}
		static public void D(string fmt, params object[] args)
		{
			D (string.Format(fmt,args));
		}
	}
}

