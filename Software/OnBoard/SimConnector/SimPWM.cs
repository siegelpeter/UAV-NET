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


namespace FlightControlCommons
{
    /// <summary>
    /// PWM Signal Output on the Polulo Mini Maestro Board (e.g: Servo Output)
    /// </summary>
    [Serializable]
    public class SimPWM : UAVStructure, IHardwareConnectable
    {
        /// <summary>
        /// PWM Channel on the Pololu Mini Maestro
        /// </summary>
        public Byte channel;

        /// <summary>
        /// Sim Connector to set values in the Sim
        /// </summary>
        public SimConnector.simConnector connector;


        /// <summary>
        /// Device Designator 
        /// </summary>
        public string device = null;
   

        /// <summary>
        /// Builds a new PWM Object
        /// </summary>
        /// <param name="servoname">Name of the Object to use with uavdata["Name of Object"]</param>
        /// <param name="servovalue">the Value to set on Initialisation</param>
        /// <param name="device">The Hardware Device to use</param>
        /// <param name="Channelnr">The Channel Number from 0 to x</param>
        public SimPWM(string servoname, int servovalue, string device, Byte Channelnr,SimConnector.simConnector connector)
            : base(servoname, "")
        {
            this.connector = connector;
            values.Add(new UAVParameter("LowLimit", 1000, 1000, 2000));
            values.Add(new UAVParameter("HighLimit", 2000, 1000, 2000));
            values.Add(new UAVParameter("Neutral", 1500, 1000, 2000));
            values.Add(new UAVParameter("Invert", 0, 0, 1));
            values.Add(new UAVParameter("Value", 1000, 1000, 2000));

            values["LowLimit"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["HighLimit"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["Neutral"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);
            values["Invert"].ValueChanged += new UAVSingleParameter.ValueChangedHandler(PWM_ValueChanged);

            //this.Value = servovalue;
            channel = Channelnr;
            this.device = device;
            this.Min = 1000;
            this.Max = 2000;
        }

        /// <summary>
        /// Update Output
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        void PWM_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            //Update Servo Value
            Value = Value;
        }

        /// <summary>
        /// Calculates Value according to Netral Point and Limits and Invert
        /// </summary>
        /// <returns>A double Value containing the Result</returns>
        public Double CalculateOutputValue()
        {
            Double InputValue = (Convert.ToDouble(values["Value"].Value));
            Double result = 0;
            Double minvalue = Convert.ToDouble(Min);
            Double maxvalue = Convert.ToDouble(Max);
            Double neutral = Convert.ToDouble(values["Neutral"].Value);
            Double lowerlimit = Convert.ToDouble(values["LowLimit"].Value);
            Double highlimit = Convert.ToDouble(values["HighLimit"].Value);

            try
            {
                // Invert if necessary
                if (Convert.ToInt32(values["Invert"].Value) == 1)
                {
                    InputValue = (maxvalue - InputValue) + minvalue;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Ungültiger Wert für Invert!");
            }

            // appy linear functions
            if (InputValue < 1500)
            {
                Double lowersteping = (neutral - lowerlimit) / 500;
                result = lowerlimit + lowersteping * (InputValue - minvalue);
            }
            else
            {
                Double highsteping = (highlimit - neutral) / 500;
                result = highlimit - highsteping * (maxvalue - InputValue);
            }

            return result;
        }

        /// <summary>
        /// Connect to Device
        /// </summary>
        public void ConnectHardware()
        {
          
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
            connector.UpdateSimValue(ChanneltoName(channel).ToString(), Convert.ToInt32(target));
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
                return values["Value"].Value;

            }
            set
            {
                if (!double.IsNaN(Convert.ToDouble((value))))
                {

                    //  Console.WriteLine("Target: "+value);
                    TrySetTarget(Convert.ToByte(channel), Convert.ToUInt16(CalculateOutputValue()));
                    values["Value"].Value = value;
                    if (ValueChanged != null)
                    {
                        ValueChanged(this, false);
                    }
                }else
                {
                }
            }
        }


     
        public string ChanneltoName(Byte Channel)
        {
            switch (channel)
            {
                case 1:
                    {
                        return "roll_out";
                    }
                case 2:
                    {
                        return "pitch_out";
                    }
                case 3:
                    {
                        return "throttle_out";
                    }
                case 4:
                    {
                        return "rudder_out";
                    }

            }

            return "";
        }
        public float ConvertToSimValue(object value)
        {
            return (Convert.ToInt32(value) - 1500) / 500;
        }

        public override UAVSingleParameter SilentUpdate(string key, object value, bool isremote)
        {
            TrySetTarget(Convert.ToByte(channel), Convert.ToUInt16(CalculateOutputValue()));
              

            return base.SilentUpdate(key, value, isremote);
        }

      


    }

}

    