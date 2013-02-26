/*
    UAV.NET: a UAV / Robotic control framework for .net
    Copyright (C) 2012  Peter Siegel

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
using Pololu.Usc;
using Pololu.UsbWrapper;

namespace FlightControlCommons
{
    [Serializable]
    public class Analog : UAVParameter, IHardwareConnectable
    {
        /// <summary>
        /// Servo Channel on Pololu PWM Board
        /// </summary>
        public Byte channel;

        /// <summary>
        /// Device designator
        /// </summary>
        public string device = null;
        [NonSerialized]
        static Usc usbdevice = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="servoname"></param>
        /// <param name="servovalue"></param>
        /// <param name="device"></param>
        /// <param name="Channelnr"></param>
        public Analog(string servoname, int servovalue, string device, Byte Channelnr)
            : base(servoname, servovalue)
        {
            channel = Channelnr;
            this.device = device;
            this.Min = 0;
            this.Max = 2000;
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

                ServoStatus[] servos;
                usbdevice.getVariables(out servos);
                ushort newvalue = servos[channel].position;
                if (Convert.ToUInt16(base.Value) != newvalue) {
                    Value = newvalue;
                    
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, false);
                    }
                }
                return Value;
            }
            set
            {
              
                base.Value = value;
                if (ValueChanged != null)
                {
                    ValueChanged(this, false);
                }
            }
        }

        public override UAVParameter SilentUpdate(string key,object value,bool isremote)
        {
         

            return base.SilentUpdate(key,value,isremote);
        }
    }

}
