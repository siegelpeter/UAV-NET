using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace UAVCommons {
    [Serializable]
    public class UDPBaseClient
    {
      
        public NetworkStream clientStream = null;
        public IPEndPoint serverEndPoint = null;
        public UdpClient client;
        protected object SendSync = new object();
        public CommunicationEndpoint.Communicationtype mode = CommunicationEndpoint.Communicationtype.Send;
       
       
        public delegate void ConnectionStateChangeHandler(UDPBaseClient source, string state);
        public event ConnectionStateChangeHandler StateChanged;
        public bool listen;


        public UDPBaseClient(string adr, int port)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(adr), port);
          
        }
        public virtual bool Connect()
        {
            if (this.mode == CommunicationEndpoint.Communicationtype.Recieve)
            {
                client = new UdpClient(serverEndPoint.Port, AddressFamily.InterNetwork);
                
            }
            else {
                client = new UdpClient();
                client.Connect(serverEndPoint);
           
            }
          
            
           
            if (StateChanged != null) StateChanged(this, "Connected");
            return true;
        }

        public virtual void Connected() {
            if (StateChanged != null) StateChanged(this, "Connected");
        }
        public virtual void Disconnect()
        {
            client.Close();
            client = null;
            if (StateChanged != null) StateChanged(this, "Disconnected");
        }

    }
}
