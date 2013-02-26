using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
using Pololu.Usc;
using Pololu.UsbWrapper;

namespace FlightControlCommons
{
    /// <summary>
    /// PWM Signal Output on the Polulo Mini Maestro Board (e.g: Servo Output)
    /// </summary>
    [Serializable]
    public class PWM : UAVStructure, IHardwareConnectable
    {
        /// <summary>
        /// PWM Channel on the Pololu Mini Maestro
        /// </summary>
        public Byte channel;
        public static ushort[] channels;

		public static bool ImmediateMode = false;

        public UAVSingleParameter internalvalue = null;


        /// <summary>
        /// Device Designator 
        /// </summary>
        public string device = null;
        [NonSerialized]
        static Usc usbdevice = null;

        /// <summary>
        /// Builds a new PWM Object
        /// </summary>
        /// <param name="servoname">Name of the Object to use with uavdata["Name of Object"]</param>
        /// <param name="servovalue">the Value to set on Initialisation</param>
        /// <param name="device">The Hardware Device to use</param>
        /// <param name="Channelnr">The Channel Number from 0 to x</param>
        public PWM(string servoname, int servovalue, string device, Byte Channelnr)
            : base(servoname, "")
        {
            if (channels == null) {
                channels = new ushort[12];
                for (int i = 0; i < channels.Length; i++ )
                {
                    channels[i] = 2000;
                }
            }
            values.Add(new UAVParameter("LowLimit", -100, -100, 100));
            values.Add(new UAVParameter("HighLimit", 100, -100, 100));
            values.Add(new UAVParameter("Neutral", 0, -100, 100));
            values.Add(new UAVParameter("Invert", 0, 0, 1));
            values.Add(new UAVParameter("Value", servovalue, -100, 100,100));
            internalvalue = values["Value"];
			values.Add(new UAVParameter("Output", 0, -100, 100,100));
            values["LowLimit"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["HighLimit"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["Neutral"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["Invert"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["Output"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            internalvalue.ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            //this.Value = servovalue;
			
            channel = Channelnr;
            this.device = device;
            this.Min = -100;
            this.Max = 100;
		
		
		

		}

        /// <summary>
        /// Update Output
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        void PWM_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            //Update Servo Value
			Console.WriteLine("Changed");
			if (param.Name != "Output"){
            Value = Value;
			}
        }
		 public Double CalculateOutputValue()
        {
		return CalculateOutputValue(double.NaN);
		}
		
        /// <summary>
        /// Calculates Value according to Netral Point and Limits and Invert
        /// </summary>
        /// <returns>A double Value containing the Result</returns>
        public Double CalculateOutputValue(double startvalue)
        {
		//        Double InputValue = Convert.ToDouble(decimal.Parse(values["Value"].Value.ToString()));
         //   Double InputValue = Convert.ToDouble(Int32.Parse(values["Value"].Value.ToString()));
            Double InputValue = internalvalue.DoubleValue;
			if (!double.IsNaN(startvalue)) 
			{
		//		Console.WriteLine("Calc Startposition Value");
				InputValue = startvalue;
			}
			Double result = 0;
            Double minvalue = Convert.ToDouble(Min);

			Double maxvalue = Convert.ToDouble(Max);

            Double neutral = values["Neutral"].DoubleValue;
            Double lowerlimit = values["LowLimit"].DoubleValue;
            Double highlimit = values["HighLimit"].DoubleValue;

            try
            { 
                // Invert if necessary
//                if (Convert.ToInt32(values["Invert"].Value) == 1)
//                {
//                    InputValue = (maxvalue - InputValue) + minvalue;
//                }
            }
            catch (Exception ex) {
                System.Console.WriteLine("Ungültiger Wert für Invert!");
            }

            // appy linear functions
            if (InputValue < 0)
            {
                Double lowersteping = (neutral - lowerlimit) / 100;
                result = lowerlimit + lowersteping * (InputValue - minvalue);
            }
            else
            {
                Double highsteping = (highlimit - neutral) / 100;
                result = highlimit - highsteping * (maxvalue - InputValue);
            }
		
			values["Output"].DoubleValue = result;
			Double res = ((result+100)*5)+1000;
		//	Console.WriteLine("CalcTarget:" + res + "InputValue"+InputValue);
			return res;
        }

        /// <summary>
        /// Connect to Device
        /// </summary>
        public void ConnectHardware()
        {
            try
            {
                connectToDevice();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Fehler beim Verbinden mit dem Mini Maestro");
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace.ToString());

            }
        }

        public delegate void ValueChangedHandler(UAVSingleParameter param, bool isremote);
        public event ValueChangedHandler ValueChanged;

        /// <summary>
        /// Connects to a Maestro using native USB and returns the Usc object
        /// representing that connection.  When you are done with the
        /// connection, you should close it using the Dispose() method so that
        /// other processes or functions can connect to the device later.  The
        /// "using" statement can do this automatically for you.
        /// </summary>
        void  connectToDevice()
        {
			
			try{
            // Get a list of all connected devices of this type.
				if ( usbdevice == null){
            List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();
		    foreach (DeviceListItem dli in connectedDevices)
            {
                if ((device == null))//|| (this.device == dli.serialNumber)
                {
                    // If you have multiple devices connected and want to select a particular
                    // device by serial number, you could simply add a line like this:
                    // if (dli.serialNumber != "00012345"){ continue; }
                    Usc uscdevice = new Usc(dli); // Connect to the device.
                   
					usbdevice = uscdevice;
                               // Return the device.
                }
            }
				}
				 if (usbdevice != null){
					usbdevice.clearErrors();
                    var settings = usbdevice.getUscSettings();
						settings.channelSettings[channel].mode = ChannelMode.Servo;
					
						Console.WriteLine("Set Startup Position Servo "+channel+" to "+settings.channelSettings[channel].home);
					settings.servoPeriod = 20;
				
						
					usbdevice.setUscSettings(settings,false);
				  //  usbdevice.setAcceleration(channel, 0);
                    usbdevice.setSpeed(channel, 0);
				  
						
					}else{
					   throw new Exception("Could not find device.  Make sure it is plugged in to USB " +
                "and check your Device Manager (Windows) or run lsusb (Linux).");
						
							
						}
				
         
			}catch (Exception ex){
				Console.WriteLine("Fehler beim Verbinden mit dem Servo "+ex.Message+ex.StackTrace.ToString());
				
			}
		

        }

		/// <summary>
		/// Sets the home position of the PWM Channel.
		/// </summary>
		/// <param name='Position'>
		/// Value of the Position from -100 to 100 
		/// Position will be calculated according to Limits and Neutral and Invert settings
		/// Position.
		/// </param>
		public void SetHomePosition (double Position)
		{
			if (usbdevice == null)
				connectToDevice ();
			if (usbdevice != null) {
				var settings = usbdevice.getUscSettings ();
				
				settings.channelSettings [channel].homeMode = HomeMode.Goto;
					
		
				settings.channelSettings [channel].home = Convert.ToUInt16 ((int)CalculateOutputValue (Position));
				Console.WriteLine ("Set Startup Position Servo " + channel + " to " + settings.channelSettings [channel].home);

						
				usbdevice.setUscSettings (settings, false);
			
			
			}
		}

        /// <summary>
        /// Attempts to set the target (width of pulses sent) of a channel.
        /// </summary>
        /// <param name="channel">Channel number from 0 to 23.</param>
        /// <param name="target">
        ///   Target, in units of quarter microseconds.  For typical servos,
        ///   6000 is neutral and the acceptable range is 4000-8000.
        /// </param>
        void TrySetTarget(Byte channel, int target)
        {
            try
            {
                //    using ()  // Find a device and temporarily connect.
                {
					try{
                  	//Console.WriteLine(this.Name+" Target :"+target*4);
					if ((target*4) < UInt16.MaxValue) {
					//Console.WriteLine("Servo:"+channel+" target:"+target)
							
                        channels[channel] = Convert.ToUInt16(target * 4);
                        //usbdevice.SetAllChannels(channels);
						if (ImmediateMode) {
								if (usbdevice == null) connectToDevice();
								Console.WriteLine("immediate Set");
								usbdevice.setTarget(channel, Convert.ToUInt16(target * 4));
							}
						}else{
				     		Console.WriteLine("Invalid Target: "+target+" for Channel: "+this.channel);
				
							
						}
					}catch (Exception ex){
						Console.WriteLine("Invalid Target "+target+ ex.Message+ex.StackTrace.ToString());
						usbdevice.disconnect();
						usbdevice = null;
						connectToDevice();
					}
				//	Console.WriteLine("Servo: "+channel+" mit wert "+target);
                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                Console.WriteLine(exception.Message + "\n" + exception.StackTrace.ToString());
                // displayException(exception);
            }
        }

        /// <summary>
        /// Name of the PWM Output e.g. Höhenruder
        /// </summary>
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }


        /// <summary>
        /// The Value contains the Value of the PWM Output, for a Servo it is the Position
        /// Setting this Value will set the Output on the Mini Maestro
        /// </summary>
        public override object Value
        {
            get
            {
                return internalvalue.Value;
                
            }
            set
            {
               
				try{
				    if (UAVBase.KeepInRange == true){
				    value = Tools.Limit(Convert.ToDouble(value),this.MinDoubleValue,this.MaxDoubleValue);
				}
                    internalvalue.Value = value;
			
				
					
			 //  Console.WriteLine("MyTarget: orignal:" +value +" typ: "+ value.GetType().ToString()+" Convert"+Convert.ToDouble(value));
               TrySetTarget(Convert.ToByte(channel), (int)CalculateOutputValue());
				    FireChangedEvent(false);
				}catch(Exception ex){
			
					
					Console.WriteLine("LowLimit:"+values["LowLimit"].Value);
					Console.WriteLine("HighLimit:"+values["HighLimit"].Value);
					Console.WriteLine("Neutral:"+values["Neutral"].Value);
					Console.WriteLine("Invert:"+values["Invert"].Value);
					
					
				Console.WriteLine("Fehler beim setzten des Wertes:"+value+" Channel"+this.channel+ex.Message+ex.StackTrace.ToString());
					
				}
            }
        }

        /// <summary>
        /// Update all Servo Positions
        /// </summary>
        public static void UpdateServos() {

			if (!PWM.ImmediateMode) Usc.SetAllChannels(channels);
        }

        /// <summary>
        /// sets the Value of the PWM without firing the ValueChanged Event
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="isremote"></param>
        /// <returns></returns>
        public override UAVSingleParameter SilentUpdate(string key, object value, bool isremote)
        {
			
            if (key == Name)
            {
				  
                    TrySetTarget(Convert.ToByte(channel), Convert.ToInt32(CalculateOutputValue()));
                
                    return base.SilentUpdate(key, value, isremote);
            }
            else
            {
                return values.SilentUpdate(key, value, isremote);
            }
        }


    }

}
