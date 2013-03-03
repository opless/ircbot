using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace SimonWaite.Network.Protocols.Irc
{
	public class BasicModes : INotifyPropertyChanged
	{

		protected BasicModes()
		{
			roObservedModes = new ReadOnlyDictionary<string, Mode> (observedModes);
			roDesiredModes = new ReadOnlyDictionary<string, Mode> (desiredModes);
			roInternalModes = new ReadOnlyDictionary<string, Mode> (internalModes);
		}

		DateTime lastChanged;

		Dictionary<string,Mode> observedModes = new Dictionary<string, Mode> ();
		Dictionary<string,Mode> desiredModes = new Dictionary<string, Mode> ();
		Dictionary<string,Mode> internalModes = new Dictionary<string, Mode> ();

		//new ReadOnlyDictionary<string,Mode>(observedModes);
		readonly ReadOnlyDictionary<string,Mode> roObservedModes;
		readonly ReadOnlyDictionary<string,Mode> roDesiredModes;
		readonly ReadOnlyDictionary<string,Mode> roInternalModes;

		public IDictionary<string,Mode> ObservedModes { get { return roObservedModes; } }
		public IDictionary<string,Mode> DesiredModes  { get { return roDesiredModes; } }
		public IDictionary<string,Mode> InternalModes { get { return roInternalModes; } }


		public DateTime LastChanged { get { return lastChanged; } }

		public virtual void ParseObservedModes (string modes, string[] args)
		{

		}
		public virtual void ParseDesiredModes (string modes, string[] args)
		{

		}
		public virtual void ParseInternalModes (string modes, string[] args)
		{

		}
		private virtual string GetNeedArgs()
		{
			return string.Empty;
		}
		private virtual string GetMultpleArgs()
		{
			return string.Empty;
		}
		protected virtual void Parse(Dictionary<string, mode> modeCollection,string modes, string[] args)
		{
			bool add=true;
			int argPtr = 0;

			foreach(char c in modes)
			{
				string cstr = c.ToString();
				string currentArg = args[argPtr];

				if(c == '+') add = true;
				if(c == '-') add = false;
				if(add)
				{
					Mode mode = modeCollection.ContainsKey(cstr) ? modeCollection[cstr] : new Mode();
					if(GetNeedArgs().Contains(cstr))
					{
						if(GetMultpleArgs().Contains(cstr) && !mode.Arguments.Contains(currentArg))
						{
							mode.Arguments.Add (currentArg );
						}
						argPtr++;
					}
				}
				else {
					if(modeCollection.ContainsKey(cstr))
					{
						if(GetNeedArgs()

						   /// ARGH this is wrong!
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
	}
}

