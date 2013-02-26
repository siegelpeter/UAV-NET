using System;
using System.Collections.Generic;

using System.Text;

namespace FlightControlCommons.components
{
          [Serializable]
    public class WaypointMissionControl:BaseMissionControl
    {
        public List<UAVCommons.Navigation.WayPoint> Waypoints = new List<UAVCommons.Navigation.WayPoint>();
    

        public WaypointMissionControl(BaseNavigation navi, string name)
            : base(navi, name)
        {
            this.values.Add(new UAVCommons.UAVParameter("WaypointIndex", 1)); // Active Waypoint
            navi.PassingWayPoint += new BaseNavigation.PassingWayPointHandler(navi_PassingWayPoint);
        }

        /// <summary>
        /// Passing old Waypoint select new Waypoint
        /// </summary>
        /// <param name="waypoint"></param>
        void navi_PassingWayPoint(UAVCommons.Navigation.WayPoint waypoint)
        {
            if (values["WaypointIndex"].IntValue < Waypoints.Count) values["WaypointIndex"].IntValue++;
            navigation.currentWaypoint = Waypoints[values["WaypointIndex"].IntValue];
        }
       

    }
}
