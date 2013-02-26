using System;
using System.Collections.Generic;

using System.Text;

namespace FlightControlCommons.components
{
    public abstract class BaseMixer : UAVCommons.UAVStructure
    {
        public UAVCommons.MonitoredDictionary<string,object> uavdata = null;
        public BaseMixer(string name,UAVCommons.MonitoredDictionary<string, object> uavdata) :base(name,null){
            this.uavdata = uavdata;
        }
        
        public abstract void Compute();

        
    }
}
