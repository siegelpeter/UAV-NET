using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
using Pololu.Usc;
using Pololu.UsbWrapper;

namespace FlightControlCommons
{
    [Serializable]
    public class DigitalOut : UAVParameter, IHardwareConnectable
    {
        public Byte channel;
        public string device = null;
        [NonSerialized]
        static Usc usbdevice = null;


        public DigitalOut(string servoname, int servovalue, string device, Byte Channelnr)
            : base(servoname, servovalue)
        {
            channel = Channelnr;
            this.device = device;
            this.Min = 0;
            this.Max = 1;
        }

        public void ConnectHardware()
        {
            if (usbdevice == null) usbdevice = connectToDevice();
        }
        public delegate void ValueChangedHandler(UAVParameter param, bool isremote);
        public event ValueChangedHandler ValueChanged;

        /// <summary>
        /// Connects to a Maestro using native USB and returns the Usc object
        /// representing that connection.  When you are done with the
        /// connection, you should close it using the Dispose() method so that
        /// other processes or functions can connect to the device later.  The
        /// "using" statement can do this automatically for you.
        /// </summary>
        Usc connectToDevice()
        {
            // Get a list of all connected devices of this type.
            List<DeviceListItem> connectedDevices = Usc.getConnectedDevices();

            foreach (DeviceListItem dli in connectedDevices)
            {
                if ((device == null) || (this.device == dli.serialNumber))
                {
                    // If you have multiple devices connected and want to select a particular
                    // device by serial number, you could simply add a line like this:
                    //   if (dli.serialNumber != "00012345"){ continue; }

                    Usc uscdevice = new Usc(dli); // Connect to the device.
                    uscdevice.getUscSettings().channelSettings[channel].mode = ChannelMode.Output;
                    
                    return uscdevice;             // Return the device.
                }
            }
            throw new Exception("Could not find device.  Make sure it is plugged in to USB " +
                "and check your Device Manager (Windows) or run lsusb (Linux).");

        }


        /// <summary>
        /// Attempts to set the target (width of pulses sent) of a channel.
        /// </summary>
        /// <param name="channel">Channel number from 0 to 23.</param>
        /// <param name="target">
        ///   Target, in units of quarter microseconds.  For typical servos,
        ///   6000 is neutral and the acceptable range is 4000-8000.
        /// </param>
        void TrySetTarget(Byte channel, UInt16 target)
        {
            try
            {
                //    using ()  // Find a device and temporarily connect.
                {
                 
                    // device.Dispose() is called automatically when the "using" block ends,
                    // allowing other functions and processes to use the device.
                }
            }
            catch (Exception exception)  // Handle exceptions by displaying them to the user.
            {
                // displayException(exception);
            }
        }

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
        public override object Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                  
                TrySetTarget(Convert.ToByte(channel), Convert.ToUInt16(value));
                base.Value = value;
                if (ValueChanged != null)
                {
                    ValueChanged(this, false);
                }
            }
        }
        public override UAVParameter SilentUpdate(string key, object value,bool isremote)
        {
            TrySetTarget(Convert.ToByte(channel), Convert.ToUInt16(value));

            return base.SilentUpdate(key,value,isremote);
        }
    }

}
