using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;

namespace JoystickInterface
{
	public class LinuxJoyStick:BaseJoystick
	{
		private FileStream devicefile = null;
		BackgroundWorker bgworker = new BackgroundWorker ();
		
		public delegate void ValueChangedHandler (int nr);

		/// <summary>
		/// The Event ValueChanged is fired when an Axis has been updated
		/// </summary>
		public event ValueChangedHandler ValueChanged;

		
        
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        struct js_event
		{
			[MarshalAs(UnmanagedType.U4, SizeConst = 32)]
			public UInt32 time;
			[MarshalAs(UnmanagedType.I2, SizeConst = 16)]
			public Int16 value;
			[MarshalAs(UnmanagedType.U1, SizeConst = 8)]
			public byte type;
			[MarshalAs(UnmanagedType.U1, SizeConst = 8)]
			public byte number;

		}

		private js_event data = new js_event ();
		private byte JS_EVENT_BUTTON = 0x01;
		private byte JS_EVENT_AXIS = 0x02;
		private byte JS_EVENT_INIT = 0x80;

		public LinuxJoyStick ()
		{
			Process calibratejoy = Process.Start ("/usr/bin/jscal","-s 5,1,1,136,137,14127751,13765501,1,1,129,130,14127751,14127751,1,1,137,138,13094013,15789839,1,0,128,128,2147483647,2147483647,1,1,132,133,14509582,15789839 /dev/input/js0");
//			calibratejoy.StartInfo.RedirectStandardError = true;
//			calibratejoy.StartInfo.RedirectStandardOutput = true;
//			calibratejoy.StartInfo.RedirectStandardInput = true;
//			calibratejoy.StartInfo.UseShellExecute = true;
		//	Console.WriteLine (calibratejoy.StandardOutput.ReadToEnd () + calibratejoy.StandardError.ReadToEnd ());
			
			calibratejoy.Start ();
			
			calibratejoy.WaitForExit ();

			Open ("/dev/input/js0");
			bgworker.RunWorkerAsync ();
			for (int i = 0; i < 8; i++) {
				Axis.Add (0);
			}
			
		}

		public T ReadStruct<T> (
    FileStream fs)
		{
			byte[] buffer = new
                byte[Marshal.SizeOf (typeof(
                T))];
			fs.Read (buffer, 0,
                Marshal.SizeOf (typeof(T)));
			GCHandle handle =
                GCHandle.Alloc (buffer,
                GCHandleType.Pinned);
			T temp = (T)
                Marshal.PtrToStructure (
                handle.AddrOfPinnedObject (),
                typeof(T));
			handle.Free ();
			return temp;
		}

		public void Open (string device)
		{
			
			devicefile = new FileStream (device, FileMode.Open, FileAccess.Read, FileShare.Read);
			bgworker.DoWork += new DoWorkEventHandler (bgworker_DoWork);
            
		}

		void bgworker_DoWork (object sender, DoWorkEventArgs e)
		{
			do {
				js_event myevent =
                    ReadStruct
                        <js_event> (devicefile);
				if (myevent.type == this.JS_EVENT_AXIS) {
					SetAxisValue (myevent.number, myevent.value);					
				} else if (myevent.type == this.JS_EVENT_BUTTON) {
					this.buttons [myevent.number] = Convert.ToBoolean (myevent.value);					
				}
			
			} while (!bgworker.CancellationPending);

		}

		~LinuxJoyStick ()
		{
			if (devicefile != null)
				devicefile.Close ();

		}

		public void SetAxisValue (byte nr, short value)
		{
		
			if (Axis [nr] != ((double)value) / 32767 * 100){
			Axis [nr] = ((double)value) / 32767 * 100;
				
				this.UpdateStatus (nr);
				
			}
			
			
		}


	}
}
