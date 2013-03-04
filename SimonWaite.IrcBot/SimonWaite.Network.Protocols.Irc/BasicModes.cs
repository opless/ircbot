using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace SimonWaite.Network.Protocols.Irc
{
	public abstract class BasicModes : INotifyPropertyChanged
	{

		protected BasicModes()
		{
			roObservedModes = new ReadOnlyDictionary<string, Mode> (observedModes);
			roDesiredModes = new ReadOnlyDictionary<string, Mode> (desiredModes);
			roInternalModes = new ReadOnlyDictionary<string, Mode> (internalModes);
		}

		DateTime lastChanged;

		protected Dictionary<string,Mode> observedModes = new Dictionary<string, Mode> ();
		protected Dictionary<string,Mode> desiredModes = new Dictionary<string, Mode> ();
		protected Dictionary<string,Mode> internalModes = new Dictionary<string, Mode> ();

		//new ReadOnlyDictionary<string,Mode>(observedModes);
		readonly ReadOnlyDictionary<string,Mode> roObservedModes;
		readonly ReadOnlyDictionary<string,Mode> roDesiredModes;
		readonly ReadOnlyDictionary<string,Mode> roInternalModes;

		public IDictionary<string,Mode> ObservedModes { get { return roObservedModes; } }
		public IDictionary<string,Mode> DesiredModes  { get { return roDesiredModes; } }
		public IDictionary<string,Mode> InternalModes { get { return roInternalModes; } }


		public DateTime LastChanged { get { return lastChanged; } }

		public abstract void ParseObservedModes (string modes, string[] args);
		public abstract void ParseDesiredModes (string modes, string[] args);
		public abstract void ParseInternalModes (string modes, string[] args);



		protected virtual void Parse(Dictionary<string, Mode> modeCollection,string modes, string[] args, string argModes, string multipleArgModes)
		{
			bool add=true;
			int argPtr = 0;

			foreach(char c in modes)
			{
				string cstr = c.ToString();
				string currentArg = args == null ? string.Empty : args[argPtr];

				if(c == '+') add = true;
				if(c == '-') add = false;
				Mode mode = modeCollection.ContainsKey(cstr) ? modeCollection[cstr] : new Mode();
				if(add)
				{
					if(argModes.Contains(cstr))
					{
						if(multipleArgModes.Contains(cstr) && !mode.Arguments.Contains(currentArg))
						{
							mode.Arguments.Add (currentArg );
						}
						argPtr++;
					}
				}
				else {
					if(modeCollection.ContainsKey(cstr))
					{
						if(argModes.Contains(cstr) && mode.Arguments.Contains(currentArg))
						{
							mode.Arguments.Remove(currentArg);
						}
						argPtr++;
					}
				}
			}
		}
		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected void Changed(string changedProperty)
		{
			lastChanged = DateTime.UtcNow;

			if (PropertyChanged != null) {
				PropertyChanged (this, new PropertyChangedEventArgs (changedProperty));
			}
		}

		public virtual string Dump(int indent)
		{
			string prefix = new string(' ',indent);
			string ret = string.Format ("{0}Modes\n",prefix);
			ret += Dump (indent + 2, "Observed", observedModes);
			ret += Dump (indent + 2, "Desired", desiredModes);
			ret += Dump (indent + 2, "Internal", internalModes);
			return ret;
		}
		private string Dump(int indent,string tag, Dictionary<string,Mode> modes)
		{
			string prefix = new string(' ',indent);
			StringBuilder sb = new StringBuilder ();
			sb.AppendFormat ("{0}{1}:\n",prefix, tag);
			List<string> keys = new List<string> (modes.Keys);
			keys.Sort();
			for(int i=0;i<keys.Count;i++)
			{
				sb.AppendFormat("{0}  Mode: {1}\n",prefix,keys[i]);
				sb.AppendLine(modes[keys[i]].Dump(indent+2));
			}
			return sb.ToString ();
		}
	}

}

