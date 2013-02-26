﻿using System;
using System.Collections.Generic;
using System.Text;
using UAVCommons;
using System.Threading;
namespace FlightControlCommons
{
        [Serializable]
    public class SimAHRS : UAVCommons.UAVStructure
    {
        
        SimConnector.simConnector connector = null;
        
        public SimAHRS(string name,SimConnector.simConnector connector)
            : base(name, "")
        {
            this.connector = connector;
            values.Add(new UAVParameter("phi", 0,-180,180, 0));
            values.Add(new UAVParameter("theta", 0180, 0, -180));
            values.Add(new UAVParameter("psi", 0180, 0, -180));

            values.Add(new UAVParameter("accx", 0, -1000, 1000, 0));
            values.Add(new UAVParameter("accy", 0, -1000, 1000, 0));
            values.Add(new UAVParameter("accz", 0, -1000, 1000, 0));

            values.Add(new UAVParameter("magx", 0, -1000, 1000, 0));
            values.Add(new UAVParameter("magy", 0, -1000, 1000, 0));
            values.Add(new UAVParameter("magz", 0, -1000, 1000, 0));

            values.Add(new UAVParameter("gyrogammastrich", 0, -1000, 1000, 0));
            values.Add(new UAVParameter("gyrbetastrich", 0, -1000, 1000, 0));
            values.Add(new UAVParameter("gyralphastrich", 0, -1000, 1000, 0));

            connector.NewValues += new SimConnector.simConnector.NewValuesHandler(connector_NewValues);
        }

        void connector_NewValues(object sender, Dictionary<string, object> values)
        {

            //X Längsachse Roll phi
            //Y Querachse Pitch theta
            //Z Hochachse Heading psi
           ((UAVParameter)this.values["phi"]).Value = TryGet(values,"roll"); //Winkel phi
           ((UAVParameter)this.values["theta"]).Value = TryGet(values,"pitch"); //Winkel
           ((UAVParameter)this.values["psi"]).Value = TryGet(values,"heading"); //Winkel

            // 3 yaw
            // 4 roll
           ((UAVParameter)this.values["gyrogammastrich"]).Value = TryGet(values,"gyrogammastrich");
           ((UAVParameter)this.values["gyrbetastrich"]).Value = TryGet(values,"gyrbetastrich");
           ((UAVParameter)this.values["gyralphastrich"]).Value = TryGet(values,"gyralphastrich");


           ((UAVParameter)this.values["accx"]).Value = TryGet(values,"accx");
           ((UAVParameter)this.values["accy"]).Value = TryGet(values,"accy");
           ((UAVParameter)this.values["accz"]).Value = TryGet(values,"accz");

           ((UAVParameter)this.values["magx"]).Value = 0;
           ((UAVParameter)this.values["magy"]).Value = 0;
           ((UAVParameter)this.values["magz"]).Value = 0;

            }

        public object TryGet(Dictionary<string, object> values,string key) {
            if (values.ContainsKey(key)) {
                return values[key];
            }
            return 0;
        }
            
    }
}