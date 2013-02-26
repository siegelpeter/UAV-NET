using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Runtime;
using System.IO;
using System.Diagnostics;
namespace UAVCommons
{
    /// <summary>
    /// A TCP Communication channel
    /// </summary>
    [Serializable]
    public class TCPCommunicationEndPoint : IPCommunicationEndPoint
    {

     
        private object SyncConnect = new object();
         
        public delegate void ConnectionStateChangeHandler(CommunicationEndpoint source, string state);
        public event ConnectionStateChangeHandler StateChanged;

       

     
    
     
        /// <summary>
        /// Starte Verbindung / Server 
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {
            base.Connect();
            try
            {
                lock (SyncConnect)
                {
                    System.Net.IPAddress ip = System.Net.IPAddress.Any;
                    if ((endpointAdress != null)&&(endpointAdress != String.Empty)) ip = System.Net.IPAddress.Parse(endpointAdress);
                    
                    if (this.commType == Communicationtype.Command)
                    {
                        if (listen)
                        {
                            server = new TCPServer.CommandTcpServer(cmdHandler, ip, endpointPort);
                             ((TCPServer.CommandTcpServer)server).StateChanged += new TCPServer.TCPServer.ConnectionStateChangeHandler(TCPCommunicationEndPoint_StateChanged);
                        }
                        else
                        {
                            client = new TcpCommandClient(endpointAdress, endpointPort, cmdHandler);
                            client.StateChanged += new TcpBaseClient.ConnectionStateChangeHandler(client_StateChanged);
                            client.Connect();
                        }
                    }
                    else if (commType == Communicationtype.Recieve)
                    {
                        if (listen)
                        {
                            server = new TCPServer.DataTcpServer(ip, endpointPort);
                            ((TCPServer.DataTcpServer)server).DataRecieved += new TCPServer.DataTcpServer.RecieveHandler(server_DataRecieved);
                             ((TCPServer.DataTcpServer)server).StateChanged += new TCPServer.TCPServer.ConnectionStateChangeHandler(TCPCommunicationEndPoint_StateChanged);
                        }
                        else
                        {
                            // if ((client != null) && (client.client.Connected)) client.client.Client.Disconnect();

                            client = new TcpDataClient(endpointAdress, endpointPort,commType);
                            ((TcpDataClient)client).DataRecieved += new TcpDataClient.RecieveHandler(server_DataRecieved);
                            ((TcpDataClient)client).StateChanged += new TcpBaseClient.ConnectionStateChangeHandler(client_StateChanged);

                            client.Connect();
                        }
                    }
                    else if (commType == Communicationtype.Send)
                    {
                        if (listen)
                        {
                            server = new TCPServer.DataTcpServer(ip, endpointPort);
                             ((TCPServer.DataTcpServer)server).StateChanged += new TCPServer.TCPServer.ConnectionStateChangeHandler(TCPCommunicationEndPoint_StateChanged);
                        }
                        else
                        {
                            client = new TcpDataClient(endpointAdress, endpointPort,commType);
                            ((TcpDataClient)client).StateChanged += new TcpBaseClient.ConnectionStateChangeHandler(client_StateChanged);
                            client.Connect();
                        }

                    }
                    else
                    {
                        //should never be the case
                        return false;
                    }
                }
                }
		   catch (Exception ex)
            {
			//	Console.WriteLine("Exception in TCPCommunication Endpoint"+ex.Message+ex.StackTrace.ToString());
            }
            finally {
                if (this.reconnecter == null) Reconnect();
   
            }
            
            return true;
        }




        public override void SendData(UAVSingleParameter param)
        {
            if (param.updateRate == Int32.MaxValue) return;
            try
            {
                lock (SendSync)
                {
                    if (commType == Communicationtype.Send)
                    {
                        if (server != null)
                        {
                            if (server is TCPServer.DataTcpServer)
                            {
                                ((TCPServer.DataTcpServer)server).Send(param);
                            }
                        }
                        if (client != null)
                        {
                            if (client is TcpDataClient) ((TcpDataClient)(client)).Send(param);
                        }
                    }
                }
            }
            catch (Exception ex) {
          
            }
        }


        public override void server_DataRecieved(DateTime TimeStamp, List<UAVParameter> data)
        {
            this.FireRecievedEvent(TimeStamp, data);
        
        }



        public override void Sendall(UAVSingleParameter[] param)
        {
            try
            {
                lock (SendSync)
                {
                    if (commType == Communicationtype.Send)
                    {
                        if (server != null)
                        {
                            if (server is TCPServer.DataTcpServer)
                            {
                                foreach (UAVSingleParameter value in param)
                                {
                                    ((TCPServer.DataTcpServer)server).Send(value);
                                }
                            }
                        }
                        if (client != null)
                        {
                            foreach (UAVSingleParameter value in param)
                            {
                                if (client is TcpDataClient) ((TcpDataClient)(client)).Send(value);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally {
                     }
        }

        public override int ConnectionCount { 
        get {
            if (listen)
            {
                return server.clients.Count;
            }
            else {
               return 1;
            }
        }
        }

   
        public override bool Connected()
        {
            lock (SyncConnect)
            {
                if (server != null)
                {
                    foreach (TcpClient myclient in server.clients)
                    {
                        if (myclient.Connected) return true;
                    }
                }else if (client != null)
                {
                    if (client.client.Connected) return true;
                }
            }
            return false;
        }

        protected override void FireStateChanged(TCPServer.TCPServer source, string state)
        {
            if (StateChanged != null) StateChanged(this, state);
        }


        public override void SendCommand(Commands.BaseCommand cmd) {
            if (this.Connected()) {
                if (this.commType == Communicationtype.Command) {
                    if (listen)
                    {
                        ((TCPServer.CommandTcpServer)server).SendCommand(cmd);
                    }
                    else {
                        ((TcpCommandClient)client).Send(cmd);
                    }
                }
            }     
        }


    }
}
