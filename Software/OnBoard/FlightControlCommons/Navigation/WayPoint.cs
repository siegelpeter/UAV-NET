using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAVCommons.Navigation
{
    [Serializable]
    public class WayPoint
    {
        public Double Longitude = 0;
        public Double Latitude = 0;
        public Double Altitude = 0;
        public bool IsAbsolute = true;
        /// <summary>
        /// Höhe Über Grund
        /// </summary>
        public Double AltitudeAGL = 0;
    }
}
