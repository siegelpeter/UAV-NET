using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
namespace UAVCommons
{
    [Serializable]
    public class UDPDataClient : UDPBaseClient
    {
        public delegate void RecieveHandler(DateTime timestamp, List<UAVParameter> data);
        public event RecieveHandler DataRecieved;
        [NonSerialized]
        Thread recieveThread;

        public UDPDataClient(string adr, int port,CommunicationEndpoint.Communicationtype mode)
            : base(adr, port)
        {
            this.mode = mode;

        }

        public override bool Connect()
        {
            base.Connect();
            if (mode == CommunicationEndpoint.Communicationtype.Recieve)
            {
                recieveThread = new Thread(new ThreadStart(Recieve));
                recieveThread.Start();
            }
                return true;
        }

        public void Send(UAVSingleParameter param)
        {
            if (param.Value == null) return;
            lock (SendSync)
            {
                if (client != null)
                {
                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] buffer = encoder.GetBytes(param.GetStringPath() + "|" + DateTime.UtcNow.Ticks.ToString() + "|" + param.Value.ToString() + "\n");
                    client.Send(buffer, buffer.Length);
                    
                }
            }
        }

        public void Recieve()
        {
            string line = "";
            byte[] message = new byte[4096];
            int bytesRead;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (true)
                {
                    Thread.Sleep(0);
                    //blocks until a client sends a message
                   // if (client.Available > 0)
                    {
                        try
                        {
                            line = new StreamReader(new MemoryStream(client.Receive(ref endpoint))).ReadToEnd();
                        }
                        catch
                        {
                            //a socket error has occured
                            break;
                        }

                        // UTF8Encoding encoder = new UTF8Encoding();
                        //   string commandl = encoder.GetString(message, 0, bytesRead);

                        // Checking for Commands from the GUI
                        //     if (line.StartsWith(";"))   // Creates a new Instance
                        if (line != null)
                        {
                            Console.WriteLine(line);
                            string name = line.Split(new char[] { '|' })[0];
                            DateTime timestamp = new DateTime(Convert.ToInt64(line.Split(new char[] { '|' })[1]));
                            string value = line.Split(new char[] { '|' })[2];
                            List<UAVParameter> parameters = new List<UAVParameter>();
                            parameters.Add(new UAVParameter(name, value.Replace("\r","").Replace("\n","")));
                            if (DataRecieved != null) DataRecieved(timestamp, parameters);

                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                client.Close();
                Disconnect();
            }
        }

    }
}