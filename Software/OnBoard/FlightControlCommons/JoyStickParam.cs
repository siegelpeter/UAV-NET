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

