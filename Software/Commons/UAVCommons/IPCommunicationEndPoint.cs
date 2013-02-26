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
    public abstract class  IPCommunicationEndPoint : CommunicationEndpoint
    {

        public string endpointAdress = null;
        public int endpointPort = 0;
        public bool listen = false;
        public Commands.CommandHandler cmdHandler = null;
        private object SyncConnect = new object();
         
        public delegate void ConnectionStateChangeHandler(CommunicationEndpoint source, string state);
        public event ConnectionStateChangeHandler StateChanged;

        public delegate void RecieveHandler(DateTime timestamp,List<UAVParameter> data);
        public event RecieveHandler DataRecieved;
        [NonSerialized]
        Dictionary<string, Timer> pingThreads = new Dictionary<string, Timer>();
        
        [NonSerialized]
        public TCPServer.TCPServer server = null;
        [NonSerialized]
        public TcpBaseClient client = null;
        [NonSerialized]
        public Thread reconnecter = null;
        public bool running = true;
        /// <summary>
        /// Starte Verbindung / Server 
        /// </summary>
        /// <returns></returns>
        public override bool Connect()
        {/*
            if (pingThreads == null) {
                pingThreads = new Dictionary<string, Timer>();
            }
            if (pingThreads.ContainsKey(this.endpointAdress))
            {

            }
            else {
                Timer pingtimer = new Timer(new TimerCallback(PingEndpoint), this.endpointAdress, 0, 10000);
                pingThreads.Add(this.endpointAdress,pingtimer);
            }
            */
            running = true;
          
          
            
            return true;
        }

        public void client_StateChanged(TcpBaseClient source, string state)
        {
            if (this.StateChanged != null)
            {
                StateChanged(this, state);
            }
        }

        public void TCPCommunicationEndPoint_StateChanged(TCPServer.TCPServer source, string state)
        {
            FireStateChanged(source, state);
        }

        protected abstract void FireStateChanged(TCPServer.TCPServer source, string state);
      

        public void Reconnect()
        {
            reconnecter = new Thread(new ThreadStart(ReconnectWorker));
            reconnecter.Start();
        }
        public void PingEndpoint(object state)
        { 
            //Process excute ping
            Process p = new Process();
            p.StartInfo.FileName= "ping";
            p.StartInfo.Arguments = state.ToString();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();

           
            string lines = p.StandardOutput.ReadToEnd();
            StringReader reader = new StringReader(lines);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if ((line == String.Empty) || (! (line.ToLower().Contains("bytes from") || (line.ToLower().Contains("antwort"))))) continue;
                if (line.ToLower().Contains("nicht") || line.ToLower().Contains("unreachable")) break;
                if (line.ToLower().Contains("antwort"))
                {
                    string[] tokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string resultstr = tokens[4].Replace("Zeit", "").Replace("ms", "").Replace("<", "");
                    server_LatencyUpdated(Convert.ToInt32(resultstr.Replace("=","")));
                }
                else
                {
                    string[] tokens = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    string resultstr = tokens[7].Replace("Time", "").Replace("ms", "").Replace("=", "").Replace(" ", "") ;
                    server_LatencyUpdated(Convert.ToInt32(resultstr));
                }
              
            }    
        }



        public virtual void server_DataRecieved(DateTime TimeStamp, List<UAVParameter> data)
        {
            if (DataRecieved != null)
            {
                DataRecieved(TimeStamp, data);
            }
        }

        public override void Disconnect()
        {
            this.running = false;
            if (server != null) server.Stop();
        }

     

        public void ReconnectWorker()
        {
            if (listen) return;
            Thread.Sleep(1000);
            do
            {
                if (!this.Connected())
                {
						Console.WriteLine("Reconnecting");
                    Connect();
                }
               
                Thread.Sleep(1000);
            } while (running);
        }


    }
}
