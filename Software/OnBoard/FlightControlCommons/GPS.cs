/*
    UAV.NET: a UAV / Robotic control framework for .net
    Copyright (C) 2012  Peter Siegel

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
using SharpGis.SharpGps;
using SharpGis.SharpGps.NMEA;
using SharpGis.SharpGps.NTRIP;

namespace FlightControlCommons
{
    /// <summary>
    /// A GPS Sensor Object connected via USB
    /// 
    ///    GPGGA - Global Positioning System Fix Data
    ///    GPGLL - Geographic position, Latitude and Longitude
    ///    GPGSA - GPS DOP and active satellites
    ///    GPGSV - Satellites in view
    ///    GPRMC - Recommended minimum specific GPS/Transit data
    ///    PGRME - Estimated Position Error (Garmin proprietary)
    ///
    /// Contains following UAV Parameters 
    ///lbRMCPosition
    ///lbRMCPositionLongitude
    ///lbRMCPositionLatitude
    ///lbRMCPositionUTM
    ///lbRMCCourse
    ///lbRMCSpeed
    ///lbRMCTimeOfFix
    ///lbRMCMagneticVariation
    ///lbGGAPosition
    ///lbGGATimeOfFix
    ///lbGGAFixQuality
    ///lbGGANoOfSats
    ///lbGGAAltitude
    ///lbGGAAltitudeUnit
    ///lbGGAHDOP
    ///lbGGAGeoidHeight
    ///lbGGADGPSupdate
    ///lbGGADGPSID
    ///lbGLLPosition
    ///lbGLLTimeOfSolution
    ///lbGLLDataValid
    ///lbGSAMode
    ///lbGLLPosition
    ///lbGSAFixMode
    ///lbGSAPRNs
    ///lbGSAPDOP
    ///lbGSAHDOP
    ///lbGSAVDOP
    ///lbRMEHorError
    ///lbRMEVerError
    ///lbRMESphericalError
    ///lbRMEVerError
    ///lbRMEVerError
    /// </summary>
    [Serializable]
    public class GPS : UAVCommons.UAVStructure, IHardwareConnectable
    {
        /// <summary>
        /// Baud Rate for Communication with the GPS 
        /// 57600 BPS for 10 HZ
        /// 9600 BPS for 1 HZ
        /// </summary>
        public int baud;

        /// <summary>
        /// Comm Port for the GPS /dev/ttyUSB0
        /// or for example COM12
        /// </summary>
        public string port;
        [NonSerialized]
        private GpsDevice device;

        /// <summary>
        /// Contructor, Creates a new instance of the Gps Object according to the given Parameters
        /// </summary>
        /// <param name="name">Name of the Object</param>
        /// <param name="port">Serial port e.g. Com1 or /dev/ttyUSB0</param>
        /// <param name="baud">Speed in baud, eg. 56700</param>
        public GPS(string name, string port, int baud)
            : base(name, port)
        {
            this.baud = baud;
            this.port = port;
            device = new GpsDevice(this);
            values = device.values;
            values.ValueChanged += new MonitoredDictionary<string, UAVSingleParameter>.ValueChangedHandler(gpsValues_ValueChanged);
        }
        /// <summary>
        /// Connect to the GPS via Serial port and Init the Main Loop
        /// </summary>
        public void ConnectHardware()
        {

            device.initgps(port, baud);
           
        }

        /// <summary>
        /// Update the Values in the Monitored Dictionary 
        /// </summary>
        /// <param name="newvalues"></param>
        void device_ValuesChanged(Dictionary<string, string> newvalues)
        {

        }
    }
}
