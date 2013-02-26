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
using JoystickInterface;
using UAVCommons;
using System.Collections.Generic;

namespace FlightControlCommons
{
	public class JoyStickParam:UAVCommons.UAVParameter
	{
		public JoystickInterface.BaseJoystick joy = null;
		private int channel = 0;
        private double lastValue = 0;
        public bool meanvalues = false;
        public int AverageValuesCount = 20;
        public int RasterIntervall = 100;
        private DateTime LastTimestamp = DateTime.MinValue;
        private List<double> oldvalues = new List<double>();
        private object syncobj = new object();
		public delegate void ValueChangedHandler(UAVParameter param,bool isremote);

        /// <summary>
        /// The Event ValueChanged is fired when this Parameter has been Updates
        /// </summary>
        public event ValueChangedHandler ValueChanged;

		
		public JoyStickParam (JoystickInterface.BaseJoystick joystick,int channel,string name,double value,double max,double min, int urate):base(name,value,min,max, urate)
		{
			joy = joystick;
			this.channel = channel;
		}
		
		public override object Value {
			get {
                if (joy == null) return 0;
			    if (lastValue != joy.Axis[channel]) lastValue = joy.Axis[channel];
                if (meanvalues) {
                    return GetMean(oldvalues);
                }
                return joy.Axis[channel] / Int16.MaxValue * 100-100;	
			}
			set {
				
			}
		}

        private object GetMean(List<Double> oldvalues)
        {
            Double sum = 0;
            lock (syncobj)
            {
                foreach (Double val in oldvalues)
                {
                    sum += val;
                }
            }
            return sum / oldvalues.Count;
        }

		public void RegisterJoyStick (JoystickInterface.BaseJoystick joystick)
		{
			joy=joystick;
			joy.ValueChanged+= HandleJoyValueChanged;
		}

		void HandleJoyValueChanged (int nr)
		{
            if (nr == channel)
            {
                Changed();
            }
		}
		
		public void Changed(){
		
            if ((lastValue != joy.Axis[channel]))
            {
                if (meanvalues)
                {
                    DateTime nowtime = DateTime.Now;
                    lock (syncobj)
                    {
                        if (LastTimestamp.AddMilliseconds(RasterIntervall) < nowtime)
                        {


                            oldvalues.Add(joy.Axis[channel]);
                            LastTimestamp = nowtime;
                        }
                        if (oldvalues.Count > (AverageValuesCount - 1))
                        {
                            oldvalues.RemoveAt(0);
                        }
                    }
                }
				   FireChangedEvent (false);
            }
			
		}
		
		
	}
}

