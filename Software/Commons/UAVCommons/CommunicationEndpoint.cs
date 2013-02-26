using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons
{
   [Serializable]
    public abstract class CommunicationEndpoint
    {
        public SpeedType endpointSpeedType = SpeedType.Wifi;
        public Communicationtype commType = Communicationtype.Command;
        protected object SendSync = new object();
        private int priority = 0;
        public int Latency = 0;
       
        /// <summary>
        /// gives the Endpoint where the Status changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="?"></param>

        public delegate void RecieveHandler(DateTime timestamp, List<UAVParameter> data);
        public event RecieveHandler DataRecieved;

        public delegate void LatencyHandler(CommunicationEndpoint source,int ms);
        public event LatencyHandler LatencyUpdated;

        public delegate void ConnectionStateChangeHandler(CommunicationEndpoint source, string state);
        public event ConnectionStateChangeHandler StateChanged;


       protected void FireRecievedEvent(DateTime timestamp, List<UAVParameter> data){
           
           if (DataRecieved != null) DataRecieved(timestamp, data);
       }

        /// <summary>
        /// Notiz für den Benutzer wie schnell die Verbindung dieses Endpunkts ist
    /// </summary>
        public enum SpeedType
        {
            Wifi,
            Gprs3G,
            RC
        }

        /// <summary>
        /// Gibt die Kommunikationsrichtung(type) an!
        /// </summary>
        public enum Communicationtype
        {
            Command,
            Recieve,
            Send,
        }

        public abstract bool Connect();
        public abstract void Disconnect();
        public abstract void SendData(UAVSingleParameter param);
        public abstract bool Connected();

        public abstract void Sendall(UAVSingleParameter[] param);
      
        /// <summary>
        /// Sendet ein Kommando an alle offenen Verbindungen dieses Endpunkts
        /// </summary>
        /// <param name="cmd"></param>
        public abstract void SendCommand(Commands.BaseCommand cmd);


        /// <summary>
        /// Gibt die Anzahl der offenen Verbindungen dieses Endpunkts zurrück
        /// </summary>
        public abstract int ConnectionCount
        {
            get;
           
        }
        /// <summary>
        /// Enthält eine Priorität wobei höher wichtiger bedeutet
        /// </summary>
        public int Priority
        {
            get
            {
                return this.priority;
            }
            set
            {
                this.priority = value;
            }
        }

        public void server_LatencyUpdated(int ms)
        {

            Latency = ms;
            if (LatencyUpdated != null) LatencyUpdated(this, ms);
        }
    }
}
