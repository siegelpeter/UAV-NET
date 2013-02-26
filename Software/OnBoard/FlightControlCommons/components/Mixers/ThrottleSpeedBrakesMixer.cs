using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
namespace FlightControlCommons.components
{
    public class ThrottleSpeedBrakesMixer:BaseMixer
    {
        /// <summary>
        /// Output
        /// </summary>
        UAVParameter throttle = null;
        UAVParameter brakes = null;

        /// <summary>
        /// Input
        /// </summary>
        UAVParameter powerlever = null;

        /// <summary>
        /// Initialises all inputs and outputs
        /// </summary>
        /// <param name="uavdata"></param>
        /// <param name="powerlever"></param>
        /// <param name="throttle"></param>
        /// <param name="brakes"></param>
        public ThrottleSpeedBrakesMixer(string name, UAVCommons.MonitoredDictionary<string, object> uavdata,UAVParameter powerlever,UAVParameter throttle,UAVParameter brakes) : base(name,uavdata){
            this.throttle = null;
            this.brakes = null;
            this.powerlever = powerlever;
        }

       
        /// <summary>
        /// 
        /// </summary>
        public override void Compute()
        {            
            if (Convert.ToDouble(powerlever.Value) > (Convert.ToDouble(powerlever.Min) + (Convert.ToDouble(powerlever.Max) - Convert.ToDouble(powerlever.Min)) / 2))
            {
                //Wenn über der Mittelstellung 
                brakes.Value = ValueControl.TransformToRange(powerlever,brakes);
                if (Convert.ToDouble(throttle.Value) != 0) throttle.Value = 0;
            }
            else {
                //Wenn unter der Mittelstellung 
                throttle.Value = ValueControl.TransformToRange(powerlever, throttle);
                if (Convert.ToDouble(brakes.Value) != 0) brakes.Value = 0;

            }
        }
    }
}
