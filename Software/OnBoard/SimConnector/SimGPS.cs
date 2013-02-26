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

namespace FlightControlCommons
{
    [Serializable]
    public class SimGPS : UAVCommons.UAVStructure
    {
        SimConnector.simConnector connector = null;

        public SimGPS(string name, SimConnector.simConnector connector)
            : base(name, "")
        {
            this.connector = connector;
            if (!values.ContainsKey("lbRMCPosition")) values.Add(new UAVParameter("lbRMCPosition",0, null, null));
            if (!values.ContainsKey("lbRMCPositionLongitude")) values.Add(new UAVParameter("lbRMCPositionLongitude", 0, null, null));
            if (!values.ContainsKey("lbRMCPositionLatitude")) values.Add(new UAVParameter("lbRMCPositionLatitude", 0, null, null));
            if (!values.ContainsKey("lbRMCPositionUTM")) values.Add(new UAVParameter("lbRMCPositionUTM", 0, null, null));
            if (!values.ContainsKey("lbRMCCourse")) values.Add(new UAVParameter("lbRMCCourse", 0, null, null));
            if (!values.ContainsKey("lbRMCSpeed")) values.Add(new UAVParameter("lbRMCSpeed", 0, null, null));
            if (!values.ContainsKey("lbRMCTimeOfFix")) values.Add(new UAVParameter("lbRMCTimeOfFix", 0, null, null));
            if (!values.ContainsKey("lbRMCMagneticVariation")) values.Add(new UAVParameter("lbRMCMagneticVariation", 0, null, null));
            if (!values.ContainsKey("lbGGAPosition")) values.Add(new UAVParameter("lbGGAPosition", 0, null, null));
            if (!values.ContainsKey("lbGGATimeOfFix")) values.Add(new UAVParameter("lbGGATimeOfFix", 0, null, null));
            if (!values.ContainsKey("lbGGAFixQuality")) values.Add(new UAVParameter("lbGGAFixQuality", 0, null, null));
            if (!values.ContainsKey("lbGGANoOfSats")) values.Add(new UAVParameter("lbGGANoOfSats", 0, null, null));
            if (!values.ContainsKey("lbGGAAltitude")) values.Add(new UAVParameter("lbGGAAltitude", 0, null, null));
            if (!values.ContainsKey("lbGGAAltitudeUnit")) values.Add(new UAVParameter("lbGGAAltitudeUnit", 0, null, null));

            if (!values.ContainsKey("lbGGAHDOP")) values.Add(new UAVParameter("lbGGAHDOP", 0, null, null));
            if (!values.ContainsKey("lbGGAGeoidHeight")) values.Add(new UAVParameter("lbGGAGeoidHeight", 0, null, null));
            if (!values.ContainsKey("lbGGADGPSupdate")) values.Add(new UAVParameter("lbGGADGPSupdate", 0, null, null));
            if (!values.ContainsKey("lbGGADGPSID")) values.Add(new UAVParameter("lbGGADGPSID", 0, null, null));
            if (!values.ContainsKey("lbGLLPosition")) values.Add(new UAVParameter("lbGLLPosition", 0, null, null));
            if (!values.ContainsKey("lbGLLTimeOfSolution")) values.Add(new UAVParameter("lbGLLTimeOfSolution", 0, null, null));
            if (!values.ContainsKey("lbGLLDataValid")) values.Add(new UAVParameter("lbGLLDataValid", 0, null, null));
            if (!values.ContainsKey("lbGSAMode")) values.Add(new UAVParameter("lbGSAMode", 0, null, null));
            if (!values.ContainsKey("lbGLLPosition")) values.Add(new UAVParameter("lbGLLPosition", 0, null, null));
            if (!values.ContainsKey("lbGSAFixMode")) values.Add(new UAVParameter("lbGSAFixMode", 0, null, null));
            if (!values.ContainsKey("lbGSAPRNs")) values.Add(new UAVParameter("lbGSAPRNs", 0, null, null));
            if (!values.ContainsKey("lbGSAPDOP")) values.Add(new UAVParameter("lbGSAPDOP", 0, null, null));
            if (!values.ContainsKey("lbGSAHDOP")) values.Add(new UAVParameter("lbGSAHDOP", 0, null, null));
            if (!values.ContainsKey("lbGSAVDOP")) values.Add(new UAVParameter("lbGSAVDOP", 0, null, null));
            if (!values.ContainsKey("lbRMEHorError")) values.Add(new UAVParameter("lbRMEHorError", 0, null, null));
            if (!values.ContainsKey("lbRMEVerError")) values.Add(new UAVParameter("lbRMEVerError", 0, null, null));
            if (!values.ContainsKey("lbRMESphericalError")) values.Add(new UAVParameter("lbRMESphericalError", 0, null, null));
            if (!values.ContainsKey("lbRMEVerError")) values.Add(new UAVParameter("lbRMEVerError", 0, null, null));
            if (!values.ContainsKey("lbRMEVerError")) values.Add(new UAVParameter("lbRMEVerError", 0, null, null));

            connector.NewValues += new SimConnector.simConnector.NewValuesHandler(connector_NewValues);
            /* 
          uavData.Add("course",new UAVParameter("course",0));
          */

        }

        private void UpdateValue(string p, object p_2)
        {
            if (values.ContainsKey(p))
            {
                values[p].Value = p_2;
            }
            else
            {
                values.Add(p, new UAVParameter(p, p_2));
            }
        }


        void connector_NewValues(object sender, Dictionary<string, object> values)
        {

            foreach (KeyValuePair<string, object> myvalue in values)
            {
                if (myvalue.Key == "lat") UpdateValue("lbRMCPositionLatitude", myvalue.Value);
                if (myvalue.Key == "lng") UpdateValue("lbRMCPositionLongitude", myvalue.Value);
                if (myvalue.Key == "altitude") UpdateValue("lbGGAAltitude", myvalue.Value);

            }

        }



    }
}

