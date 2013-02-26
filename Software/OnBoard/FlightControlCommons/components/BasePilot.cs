using System;
using System.Collections.Generic;

using System.Text;

using UAVCommons;

namespace FlightControlCommons.components
{
          [Serializable]
    public class BasePilot : UAVCommons.UAVStructure
    {
        public BasePilot(string name) : base(name, null) {

            //TODO: Werte sind Winkel
            this.values.Add(new UAVCommons.UAVParameter("phi_rollrateout", 0));
            this.values.Add(new UAVCommons.UAVParameter("psi_rollrateout", 0));
            this.values.Add(new UAVCommons.UAVParameter("theta_rollrateout", 0));
        }

    }
}
