using System;
using System.Collections.Generic;

namespace SimonWaite.Network.Protocols.Irc
{
	public class Mode
	{
		List<string> args = new List<string> ();

		public string Name { get; set; } // ie o, k, v, n etc
		public List<string> Arguments { get { return args; } }

	}

}