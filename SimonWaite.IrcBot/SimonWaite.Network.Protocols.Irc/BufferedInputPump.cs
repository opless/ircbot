using System;
using System.IO;
using System.Text;

namespace SimonWaite.Network.Protocols.Irc
{
	internal class BufferedInputPump
	{
		Rfc1459 parent;
		Stream stream;
		IrcSubscriptionQueue queue;
		byte[] lineBuffer = new byte[1024]; // servers are usually limited to 512 bytes ;)
		byte[] readBuffer = new byte[0x8000]; // 32K read buffer

		int ptr = 0;
		bool needsKick;
		bool closed;

		public BufferedInputPump (Stream stream, IrcSubscriptionQueue queue, Rfc1459 parent)
		{
			this.stream = stream;
			this.queue = queue;
			this.parent = parent;
			needsKick = true;
			closed = false;
		}

		public void Kick ()
		{
				
			Log.D ("KICK InputPump needs this: {0}", needsKick);
			if (!needsKick)
				return;
				
			needsKick = false;
			stream.BeginRead (readBuffer, 0, readBuffer.Length, new AsyncCallback (Pump), this);
		}

		void Pump (IAsyncResult ar)
		{
			Log.D ("InputPump - Pump!");
			int amountRead = 0;
			try { 
				amountRead = stream.EndRead (ar);

			} catch (Exception e) {
				parent.TearDownConnection (e);
				return;
			}

			Log.D ("Read {0} bytes", amountRead);
			if (amountRead == 0) {
				// this'll mean the link has closed.
				Log.D ("Closed Link Detected");
				closed = true;
				parent.TearDownConnection(null);
				return;
			}
			// read into linebuffer, then parse into queue
			for (int i=0; i < amountRead; i++) {
				byte c = readBuffer [i];
				if (c == '\r' || c == '\n') {
					// skip blank lines, or ultra long ones
					if (ptr > 0) {
						// parse into queue
						string line = Encoding.UTF8.GetString (lineBuffer, 0, ptr);
						Log.D ("InputPump Line: '{0}'", line);

						IrcMessage msg = new IrcMessage (line);
						//Log.D ("InputPump SMesg: '{0}'", msg);
						//Log.D ("InputPump DMesg: '{0}'", msg.ToDebugString());
						queue.Enqueue (msg);
						ptr = 0;
					}
					continue; //skip
				}
				if (ptr < lineBuffer.Length) {
					lineBuffer [ptr] = c;
					ptr ++;
				}

			}
			stream.BeginRead (readBuffer, 0, readBuffer.Length, new AsyncCallback (Pump), this);

		}

	}
		
}

