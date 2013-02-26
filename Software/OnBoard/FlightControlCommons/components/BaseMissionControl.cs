using System;
using System.Collections.Generic;

using System.Text;

namespace FlightControlCommons.components
{
          [Serializable]
    public class BaseMissionControl : UAVCommons.UAVStructure
    {
      

        public BaseNavigation navigation = null;

        public BaseMissionControl(BaseNavigation navi, string name) : base(name, null) {
            this.navigation = navi;
        }

    }
}
