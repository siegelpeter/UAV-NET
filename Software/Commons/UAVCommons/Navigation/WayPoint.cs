using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Navigation
{
    /// <summary>
    /// This Class represents a Waypoint
    /// Can hold the GPS position and the Altitude in AGL and Absolute
    /// </summary>
    [Serializable]
    public class WayPoint
    {
        /// <summary>
        /// The Longitudinal Position e.g. 14
        /// </summary>
        public Double Longitude = 0;
        /// <summary>
        /// The Latitude e.g. 40 N is +
        /// </summary>
        public Double Latitude = 0;

        /// <summary>
        /// The Altitude if IsAbsolute == true
        /// </summary>
        public Double Altitude = 0;
        /// <summary>
        /// If Altitude AGL is used set this to true
        /// </summary>
        public bool IsAbsolute = true;
        /// <summary>
        /// Höhe Über Grund
        /// </summary>
        public Double AltitudeAGL = 0;
    }
}
