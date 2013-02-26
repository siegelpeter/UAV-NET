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
    [Serializable]
    public class UDPCommunicationEndPoint : IPCommunicationEndPoint
    {

        private object SyncConnect = new object();

        public delegate void ConnectionStateChangeHandler(CommunicationEndpoint source, string state);
        public event ConnectionStateChangeHandler StateChanged;


        [NonSerialized]
        public UDPBaseClient client = null;

        /// <summary>
        /// Starte Verbindung / Server 
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {

            running = true;
            try
            {
                lock (SyncConnect)
                {
                    System.Net.IPAddress ip = System.Net.IPAddress.Any;
                    if ((endpointAdress != null) && (endpointAdress != String.Empty)) ip = System.Net.IPAddress.Parse(endpointAdress);

                    if ((commType == Communicationtype.Recieve) || (commType == Communicationtype.Send))
                    {

                        client = new UDPDataClient(endpointAdress, endpointPort,commType);
                        
                        ((UDPDataClient)client).DataRecieved += new UDPDataClient.RecieveHandler(server_DataRecieved);
                      
                        client.Connect();

                    }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
   
            }

            return true;
        }


        public override void server_DataRecieved(DateTime TimeStamp, List<UAVParameter> data)
        {
           this.FireRecievedEvent(TimeStamp, data);
            
        }

        public override void Disconnect()
        {
            this.running = false;
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
                            if ((client is UDPDataClient) &&(client != null)) ((UDPDataClient)(client)).Send(param);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        public override void Sendall(UAVSingleParameter[] param)
        {
            try
            {
                lock (SendSync)
                {
                    if (commType == Communicationtype.Send)
                    {
                            foreach (UAVParameter value in param)
                            {
                                if (client is UDPDataClient) ((UDPDataClient)(client)).Send(value);
                            }
                      }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
            }
        }

        public override int ConnectionCount
        {
            get
            {
                    return 1;
            }
        }


        public override bool Connected()
        {
                  return true;
            }

        /// <summary>
        /// No Commands via UDP for now
        /// </summary>
        /// <param name="cmd"></param>
        public override void SendCommand(Commands.BaseCommand cmd)
        {
        }



        protected override void FireStateChanged(TCPServer.TCPServer source, string state)
        {
            if (StateChanged != null) StateChanged(this, state);
        }
    }
}
