using System;
using System.Collections.Generic;
using System.Text;

namespace SimonWaite.Network.Protocols.Irc
{
	public class Mode
	{
		List<string> args = new List<string> ();

		public string Name { get; set; } // ie o, k, v, n etc
		public List<string> Arguments { get { return args; } }


		public string Dump(int indent)
		{
			string prefix = new string (' ', indent);
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("{0}Mode\n{0}* Name: '{1}'\n* Args: {2}",
			                prefix, Name, args.Count);
			for (int i=0; i<args.Count; i++) {
				sb.AppendFormat("{0}* * {1}: '{2}'",prefix,i,args[i]);
				sb.AppendLine();
			}
			return sb.ToString ();
		}
	}

}