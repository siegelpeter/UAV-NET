using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace UAVCommons {
    [Serializable]
    public class TcpBaseClient
    {
        public TcpClient client = null;
        public NetworkStream clientStream = null;
        public IPEndPoint serverEndPoint = null;
        protected object SendSync = new object();
        public CommunicationEndpoint.Communicationtype mode = CommunicationEndpoint.Communicationtype.Send;
     
       
        public delegate void ConnectionStateChangeHandler(TcpBaseClient source, string state);
        public event ConnectionStateChangeHandler StateChanged;


        public TcpBaseClient(string adr, int port)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Parse(adr), port);
        }
        public virtual bool Connect()
        {

            client = new TcpClient();
            client.Connect(serverEndPoint);
            clientStream = client.GetStream();
            if (StateChanged != null) StateChanged(this, "Connected");
            return true;
        }

        public virtual void Connected() {
            if (StateChanged != null) StateChanged(this, "Connected");
        }
        public virtual void Disconnect()
        {
            if (client.Connected) client.Close();
            if (StateChanged != null) StateChanged(this, "Disconnected");
        }

    }
}
