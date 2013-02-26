using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons.Commands;
using UAVCommons;
namespace FlightControlCommons.components
{
    public class UpdateWayPoints:BaseCommand
    {
        List<UAVCommons.Navigation.WayPoint> waypoints = null;
        string MissionControlName = null;
        int currentWayPointIndex = 0;
        public UpdateWayPoints(List<UAVCommons.Navigation.WayPoint> waypoints,string MissionControlName, int CurrentWayPointIndex)
            : base("UpdateWayPoints")
        {
        //    MissionControlName = MissionControlName;
            this.waypoints = waypoints;
           
        }

        public override void Send(UAVBase core)
        {
            base.Send(core);
            
        }
        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
          // ((FlightControlCommons.components.WaypointMissionControl)core.uavData[MissionControlName]).Waypoints = waypoints;
           
           
        }
    
    }
}
