using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons;
using GroundControlCore;
using UAVCommons.Commands;
using System.Configuration;
namespace GroundControlCore
{
    public class UAV : UAVBase
    {

        /// <summary>
        /// gives the Endpoint where the Status changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="?"></param>
        public delegate void NewFlightLogMessageHandler( string Message);
        public event NewFlightLogMessageHandler NewFlightLogMessage;
        public event NewGroundLogMessageHandler NewGroundLogMessage;
        public delegate void NewGroundLogMessageHandler(string Message);

        public override void run() { 
        
        }


        /// <summary>
        /// Initialise Object
        /// </summary>
        public UAV()
        {
            if (Convert.ToBoolean(ConfigurationSettings.AppSettings["WLAN"]))
            {
                TCPCommunicationEndPoint controlconnection = new TCPCommunicationEndPoint();
                controlconnection.commType = CommunicationEndpoint.Communicationtype.Command;
                controlconnection.endpointAdress = ConfigurationSettings.AppSettings["GroundIP"];
                controlconnection.endpointPort = Convert.ToInt32(ConfigurationSettings.AppSettings["GroundCommandPort"]);
                controlconnection.endpointSpeedType = CommunicationEndpoint.SpeedType.Wifi;
                controlconnection.listen = true;
                controlconnection.cmdHandler = this;
                this.knownEndpoints.Add(controlconnection);
               if (NewGroundLogMessage != null) NewGroundLogMessage("GroundCommandEndpoint: "+controlconnection.endpointAdress+":"+controlconnection.endpointPort);
            }
            if (Convert.ToBoolean(ConfigurationSettings.AppSettings["VPN"]))
            {
                TCPCommunicationEndPoint recieveConnection = new TCPCommunicationEndPoint();
                recieveConnection.commType = CommunicationEndpoint.Communicationtype.Send;
                recieveConnection.endpointAdress = ConfigurationSettings.AppSettings["VPNGroundIP"];
                recieveConnection.endpointPort = Convert.ToInt32(ConfigurationSettings.AppSettings["VPNGroundSendPort"]);
                recieveConnection.endpointSpeedType = CommunicationEndpoint.SpeedType.Gprs3G;
                recieveConnection.listen = true;
                if (NewGroundLogMessage != null) NewGroundLogMessage("VPNCommandEndpoint: " + recieveConnection.endpointAdress + ":" + recieveConnection.endpointPort);

                this.knownEndpoints.Add(recieveConnection);


                TCPCommunicationEndPoint recieveConnection1 = new TCPCommunicationEndPoint();
                recieveConnection1.commType = CommunicationEndpoint.Communicationtype.Recieve;
                recieveConnection1.endpointAdress = ConfigurationSettings.AppSettings["VPNGroundIP"];
                recieveConnection1.endpointPort = Convert.ToInt32(ConfigurationSettings.AppSettings["VPNGroundRecievePort"]);
                recieveConnection1.endpointSpeedType = CommunicationEndpoint.SpeedType.Gprs3G;
                recieveConnection1.listen = true;
                if (NewGroundLogMessage != null) NewGroundLogMessage("VPNGroundRecievePort: " + recieveConnection1.endpointAdress + ":" + recieveConnection1.endpointPort);

                this.knownEndpoints.Add(recieveConnection1);
            }
            if (Convert.ToBoolean(ConfigurationSettings.AppSettings["WLAN"]))
            {
                TCPCommunicationEndPoint sendConnection1 = new TCPCommunicationEndPoint();
                sendConnection1.commType = CommunicationEndpoint.Communicationtype.Send;
                sendConnection1.endpointAdress = ConfigurationSettings.AppSettings["GroundIP"];
                sendConnection1.endpointPort = Convert.ToInt32(ConfigurationSettings.AppSettings["GroundSendPort"]);
                sendConnection1.endpointSpeedType = CommunicationEndpoint.SpeedType.Wifi;
                sendConnection1.listen = true;
                if (NewGroundLogMessage != null) NewGroundLogMessage("GroundSend: " + sendConnection1.endpointAdress + ":" + sendConnection1.endpointPort);

                this.knownEndpoints.Add(sendConnection1);


                TCPCommunicationEndPoint sendConnection = new TCPCommunicationEndPoint();
                sendConnection.commType = CommunicationEndpoint.Communicationtype.Recieve;
                sendConnection.endpointAdress = ConfigurationSettings.AppSettings["GroundIP"];
                sendConnection.endpointPort = Convert.ToInt32(ConfigurationSettings.AppSettings["GroundRecievePort"]);
                sendConnection.endpointSpeedType = CommunicationEndpoint.SpeedType.Wifi;
                sendConnection.listen = true;
                if (NewGroundLogMessage != null) NewGroundLogMessage("GroundRecieve: " + sendConnection.endpointAdress + ":" + sendConnection.endpointPort);
                this.knownEndpoints.Add(sendConnection);
            }
            NewGroundLogMessage += new NewGroundLogMessageHandler(UAV_NewGroundLogMessage);
        }

        void UAV_NewGroundLogMessage(string Message)
        {
            WriteToLog(this, "GroundLog", Message);  
        }

        public override void UAV_CommunicationStatusChanged(CommunicationEndpoint source, string state)
        {
            base.UAV_CommunicationStatusChanged(source, state);
            string msg = "";
            string target = "";
            if (source is TCPCommunicationEndPoint)
            {
                target = ((TCPCommunicationEndPoint)source).endpointAdress + ":" + ((TCPCommunicationEndPoint)source).endpointPort;
            }
            msg = source.commType.ToString() + "for " + target + "has new state " + state;

     

            if (NewGroundLogMessage != null) NewGroundLogMessage(msg);
        }

        void controlconnection_StateChanged(CommunicationEndpoint source, string state)
        {
            if (source is TCPCommunicationEndPoint)
            {
                if (source.commType == CommunicationEndpoint.Communicationtype.Command)
                {
                    if (state == "Connected"){ 
                    UAVCommons.Commands.GetParameters getcmd = new UAVCommons.Commands.GetParameters();
                    SendCommand(getcmd);
                    }
                }
            }
        }

       
     

    
    }
}
