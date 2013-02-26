// Copyright 2007 - Morten Nielsen
//
// This file is part of SharpGps.
// SharpGps is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpGps is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpGps; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//
// 2005-05-22: Added checksum checking
//
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Globalization;
using System.Diagnostics;

namespace FlightControlCommons
{
	/// <summary>
	/// Class for handling serial communications with the GPS receiver
	/// </summary>
	[Serializable]
	internal class SerialPort : IDisposable
	{
		private int _TimeOut = 5; //Set default timeout to 5 seconds
		private long TimeSinceLastEvent;
		private bool HasTimedOut;
        public List<string> list = new List<string>();

        public List<string> currentsettings = new List<string>();

		private const int _receivedBytesThreshold = 200;
		[NonSerialized]
		private System.IO.Ports.SerialPort
			com;
		private bool disposed = false;
		
		/// <summary>
		/// Initilializes the serialport
		/// </summary>
		public SerialPort ()
		{
			disposed = false;
		}

		public SerialPort (string SerialPort, int BaudRate)
		{
			Port = SerialPort;
			this.BaudRate = BaudRate;
			/*      com.DataBits = 8;
            com.StopBits = StopBits.One;
            com.Parity = Parity.None;
            com.RtsEnable = true;
            com.DtrEnable = false;*/
			disposed = false;
		}

		public void Dispose ()
		{
			if (!this.disposed) {
				this.Stop ();
				com = null;
			}
			disposed = true;

			// Take yourself off the Finalization queue 
			// to prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize (this);
		}
		/// <summary>
		/// Finalizer
		/// </summary>
		~SerialPort ()
		{
			Dispose ();
		}


        private void ForceSetBaudRate(string portName, int baudRate)
        {
            if (Type.GetType("Mono.Runtime") == null) return; //It is not mono === not linux! 
            string arg = String.Format("-F {0} speed {1}", portName, baudRate);
            var proc = new Process
            {
                EnableRaisingEvents = false,
                StartInfo = { FileName = @"stty", Arguments = arg }
            };
            proc.Start();
            proc.WaitForExit();
        }


		/// <summary>
		/// Gets or sets the timeout in seconds.
		/// <remarks>5 second default</remarks>
		/// </summary>
		internal int TimeOut {
			get { return _TimeOut; }
			set { _TimeOut = value; }
		}

		/// <summary>
		/// Serial port
		/// </summary>
		internal string Port {
			get ;
			set ;
		}

		/// <summary>
		/// BaudRate
		/// </summary>
		internal int BaudRate {
			get ;
			set ;
		}

		/// <summary>
		/// Opens the GPS port ans starts parsing data
		/// </summary>
		private void OpenPort ()
		{
			TimeSinceLastEvent = DateTime.Now.Ticks;
			HasTimedOut = false;
			//com.DataReceived += new SerialDataReceivedEventHandler(this.Read);
			try {
				com = new System.IO.Ports.SerialPort (Port, BaudRate);
                com.DtrEnable = true;
				com.Open ();
                ForceSetBaudRate(Port, BaudRate);
			} catch (System.Exception ex) {
				GC.SuppressFinalize (com);
				Console.WriteLine ("Cannot OPEN AHRS PORT: " + ex.Message);
				//Some devices (like iPAQ H4100 among others) throws an IOException for no reason
				//Let's just ignore it and run along
				//Thanks to Shaun O'Callaghan for pointing this out
			}
		
		}

		/// <summary>
		/// Species whether the serialport is open or not
		/// </summary>
		internal bool IsPortOpen {
			get { 
				if (com == null)
					return false;
				return com.IsOpen;
			}
		}

		/// <summary>
		/// Opens the serial port and starts parsing NMEA data. Returns when the port is closed.
		/// </summary>
		internal void Start ()
		{
			Read ();
		}

		/// <summary>
		/// Check the serial port for data. If data is available, data is read and parsed.
		/// This is a loop the keeps running until the port is closed.
		/// </summary>
		private void Read ()
		{
			System.Threading.Thread.CurrentThread.CurrentCulture = new
System.Globalization.CultureInfo ("en-US");
			int MilliSecondsWait = 10;
			string strData = "";
		
			do {
				OpenPort ();
				Console.WriteLine ("Connected AHRS now " + Port);
				if (((com != null)&&(com.IsOpen))){
                    string headline = "";
                    do
                    {
                         headline = com.ReadLine(); // sparkfun headline
                    } while (!headline.Contains("Sparkfun 9DOF Razor AHRS\r"));
                
					list.Add(com.ReadLine().Replace ("\r", ""));
                list.Add(com.ReadLine().Replace ("\r", ""));
                list.Add(com.ReadLine().Replace ("\r", ""));
                list.Add(com.ReadLine().Replace ("\r", ""));
                list.Add(com.ReadLine().Replace ("\r", ""));
                list.Add(com.ReadLine().Replace ("\r", ""));
                }
                FireEvent(null);
                if ((currentsettings != null) && (currentsettings.Count > 0))
                {
                    Console.WriteLine("Applying Razor Settings");

                    com.Write("#");
                    foreach (string val in this.currentsettings)
                    {
                        foreach (char c in val.ToCharArray()){
                        com.Write(Convert.ToString(c));
                        }
                        com.Write("\r");
                    }

                    com.BaseStream.Flush();
                    Console.WriteLine("AHRS settings sent");

                }
                else {
                    com.Write("!");
                    com.BaseStream.Flush();
                }
				// Repeated Values 
                string templine = "";
                do
                {
                    templine = com.ReadLine();
                } while (!templine.StartsWith("!ANG:"));

				Console.WriteLine("AHRS initialized");

                List<UAVCommons.UAVParameter> param = new List<UAVCommons.UAVParameter> ();
				while ((com != null)&&(com.IsOpen)) {        
					try {
        			
						string line = com.ReadLine ();
						Console.WriteLine(line);

						if (line.StartsWith ("!")) {
							line = line.Replace ("!ANG:", "");
							line = line.Replace ("AN:", "");
							
							line = line.Replace ("\r", "");
							string[] values = line.Split (',');
							int i = 0;
							foreach (string value in values) {
								string cleanValue = value.Trim();
                       
				                   
                        
								double dval = 0;
								try {
                                    dval = double.Parse (cleanValue,NumberStyles.Float);
								//	dval = Convert.ToDouble (decimal.Parse (cleanValue));


								//	dval = double.Parse(cleanValue);
								//	Console.WriteLine("convert: "+cleanValue+" "+dval.ToString());
									if ((i >= param.Count)&&((param.Count < 12))) {
										param.Add (new UAVCommons.UAVParameter (i.ToString (), dval));
									} else {
										param [i].DoubleValue = dval;
									}

								} catch (Exception ex) {
									Console.WriteLine (ex.Message + ex.StackTrace.ToString ());
									Console.WriteLine (cleanValue);
								}
								i++;
							}

           
							FireEvent (param);
						}
                    
                
					} catch (Exception ex) {

						Console.WriteLine ("Fehler beim Lesen vom Serial Port für AHRS:" + ex.Message + ex.StackTrace.ToString ());
					}
				}
				Thread.Sleep (100);
				try {
					com = new System.IO.Ports.SerialPort (Port, BaudRate);

					com.Open ();
				} catch (Exception ex) {
					Console.WriteLine ("AHRS not found");
				}
			} while (true);
          
		
		}
		
		public static double GetDouble (string value)
		{
			double result = double.NaN;

			//Try parsing in the current culture
			if (!double.TryParse (value, System.Globalization.NumberStyles.Any, CultureInfo.CurrentCulture, out result) &&
			//Then try in US english
				!double.TryParse (value, System.Globalization.NumberStyles.Any, CultureInfo.GetCultureInfo ("en-US"), out result) &&
			//Then in neutral language
				!double.TryParse (value, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out result)) {
  
			}
			return result;
		}


		/// <summary>
		/// Fires a NewGPSFix event
		/// </summary>
		/// <param name="type">Type of GPS event (GPGGA, GPGSA, etx...)</param>
		/// <param name="sentence">NMEA Sentence</param>
		private void FireEvent (List<UAVCommons.UAVParameter> values)
		{
			NewData (this, values);
			//NewGPSData(this, e);
		}
		/// <summary>
		/// Delegate type for hooking up change notifications.
		/// </summary>
		public delegate void NewDataRowHandler (object sender, List<UAVCommons.UAVParameter> e);
		/// <summary>
		/// Event fired whenever new GPS data is acquired from the receiver.
		/// </summary>
		public event NewDataRowHandler NewData;

		/// <summary>
		/// Eventtype parsed when GPS sends a sentence
		/// </summary>
		public class DataEventArgs : EventArgs
		{
			/// <summary>
			/// Type of event that occured, specied by NMEA type (GPRMC, GPGGA, GPGSA, GPGLL, GPGSV or PGRME)
			/// </summary>
			public List<UAVCommons.UAVParameter> channelValues;

			public DataEventArgs (List<UAVCommons.UAVParameter> content)
			{
				this.channelValues = content;
			}

		}

		/// <summary>
		/// Closes the port and ends the thread.
		/// </summary>
		internal void Stop ()
		{
			if (com != null)
			if (com.IsOpen)
				com.Close ();
		}

		

	}
}
