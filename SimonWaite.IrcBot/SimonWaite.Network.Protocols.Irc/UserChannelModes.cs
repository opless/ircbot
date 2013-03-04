using System;
using System.Collections.Generic;
using System.Text;

namespace SimonWaite.Network.Protocols.Irc
{
	class UserChannelModes : BasicModes
	{
		static readonly string argumentModes = string.Empty; //
		static readonly string multipleArgumentModes = string.Empty; 

		
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
	}

}

