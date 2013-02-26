using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;

namespace FlightControlCommons.components
{
          [Serializable]
    public class BaseNavigation : UAVCommons.UAVStructure
    {
        public BaseAutoPilot ap = null;
        public UAVCommons.UAVStructure performanceData = null;
        public UAVCommons.UAVStructure gps = null;
        public UAVCommons.Navigation.WayPoint currentWaypoint = null; 
        public delegate void PassingWayPointHandler(UAVCommons.Navigation.WayPoint waypoint);
        public event PassingWayPointHandler PassingWayPoint;

        /// <summary>
        /// Initialise Navigation
        /// Baue Navigations Objekt auf
        /// </summary>
        /// <param name="name"></param>
        /// <param name="ap"></param>
        /// <param name="gps"></param>
        public BaseNavigation(string name, UAVCommons.UAVStructure gps, UAVCommons.UAVStructure Performancedata)
            : base(name, null)
        {
          
            this.gps = gps;
            this.performanceData = Performancedata;
            gps.ValueChanged += new ValueChangedHandler(gps_ValueChanged);
            //Config Values
            values.Add(new UAVParameter("TargetPsi",0,360,0));
            values.Add(new UAVParameter("TargetAltitude", 1000, 999999, 0));
           }


        /// <summary>
        ///  Calculate Distance to current Waypoint and if overhead call event
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
       void gps_ValueChanged(UAVCommons.UAVParameter param, bool isremote)
        {
            if (currentWaypoint != null)
            {
                if (Distance((UAVStructure)this.gps, currentWaypoint) < Convert.ToDouble(performanceData["WayPointThreshold"].Value))
                {
                    if (PassingWayPoint != null) PassingWayPoint(currentWaypoint);
                }

                values["TargetPsi"].Value = this.GetDirection((FlightControlCommons.GPS)gps, currentWaypoint);

                values["TargetAltitude"].Value = currentWaypoint.Altitude;
            }
            else { 
            // What todo if there is no current WayPoint Set?
            
            }
            }

        /// <summary>
        /// Calculates the Distance from the Current Position to the given Waypoint in ?? NM ??
        /// </summary>
        /// <param name="gps">A object representing the onboard GPS</param>
        /// <param name="currentWaypoint">A Waypoint representing the Target for the Distance Calc</param>
        /// <returns>The Distance in ?? NM?? as a double value</returns>
        public double Distance(UAVStructure gps, UAVCommons.Navigation.WayPoint currentWaypoint)
        {
            if ((gps.values["lbRMCPositionLongitude"] != null) && (gps.values["lbRMCPositionLatitude"] != null))
            {
                SharpGis.SharpGps.Coordinate gpslocation = new SharpGis.SharpGps.Coordinate(Convert.ToDouble(gps.values["lbRMCPositionLongitude"].Value), Convert.ToDouble(gps.values["lbRMCPositionLatitude"].Value));
                double greatCircleDistance = gpslocation.Distance(new SharpGis.SharpGps.Coordinate(currentWaypoint.Longitude, currentWaypoint.Latitude));
                double altDistance = Math.Max(Convert.ToDouble(gps.values["lbGGAAltitude"].Value), currentWaypoint.Altitude) - Math.Min(Convert.ToDouble(gps.values["lbGGAAltitude"].Value), currentWaypoint.Altitude);
                return Math.Sqrt(greatCircleDistance * greatCircleDistance + altDistance * altDistance);

            }
            else {
                // Log error
            }
            return 99999999;
        }

        /// <summary>
        /// Returns the Direction from the Current Position to the given Waypoint
        /// </summary>
        /// <param name="gps">The Current GPS</param>
        /// <param name="myWaypoint">A Waypoint Object containing the position of the Target Waypoint</param>
        /// <returns>The Bearing from the Current Position to the Waypoint in degrees (0..360)</returns>
        public double GetDirection(GPS gps, UAVCommons.Navigation.WayPoint myWaypoint)
        {
            double lat1 = Convert.ToDouble(gps["lbRMCPositionLatitude"].Value);
            double lat2 = myWaypoint.Latitude;
            double lon1 = Convert.ToDouble(gps["lbRMCPositionLongitude"].Value);
            double lon2 = myWaypoint.Longitude;
           
            // Calculate Bearing in Radians
            double resultRad = (Math.Atan2((Math.Cos(lat1)*Math.Sin(lat2)) - (Math.Sin(lat1)*Math.Cos(lat2)*Math.Cos(lon2 - lon1)),
                               Math.Sin(lon2 - lon1)*Math.Cos(lat2)) % (2*Math.PI));

            return resultRad*(180/Math.PI);
        } // Convert Radians to Degrees

    }
}
