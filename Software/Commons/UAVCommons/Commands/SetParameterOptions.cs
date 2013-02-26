using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Commands
{
    /// <summary>
    /// Sets the Options of a Parameter in a remote uavdata Store
    /// e.g.: Sets UpdateRate for a AHRS\phi Value 
    /// </summary>
       [Serializable]
    public class SetParameterOptions : BaseCommand
    {
        UAVParameter parameter = null;

        public SetParameterOptions(UAVParameter param) {
            parameter = param;
        }

        public SetParameterOptions() { }

        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
          
            core.uavData[parameter.Name].Min = parameter.Min;
            core.uavData[parameter.Name].Max = parameter.Max;
            core.uavData[parameter.Name].updateRate = parameter.updateRate;
        }
    }
}
