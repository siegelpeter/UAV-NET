using System;
using System.Collections.Generic;

using System.Text;
using FlightControlCommons;
using UAVCommons;
namespace FlightControl
{
    public class VTOLFlightStabilizer:FlightControlCommons.components.BaseFlightStabilizer
    {
        public PWM power_out = null;

        public PIDLibrary.ParameterPID powerPid = null;

        public VTOLFlightStabilizer(string name, UAVStructure myAhrs, FlightControlCommons.components.BaseAutoPilot ap)
            : base(name, myAhrs, ap)
        {
    
    }
      

    }
}
