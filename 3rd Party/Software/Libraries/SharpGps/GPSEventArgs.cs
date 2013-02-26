using System;
using System.Collections.Generic;

using System.Text;

namespace SharpGis.SharpGps
{
    /// <summary>
    /// Eventtype invoked when a new message is received from the GPS.
    /// String GPSEventArgs.TypeOfEvent specifies eventtype.
    /// </summary>
    public class GPSEventArgs : EventArgs
    {
        /// <summary>
        /// Type of event
        /// </summary>
        public GPSEventType TypeOfEvent;
        /// <summary>
        /// Full NMEA sentence
        /// </summary>
        public string Sentence;
    }
}
