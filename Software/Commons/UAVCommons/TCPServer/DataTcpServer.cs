using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;
namespace UAVCommons.TCPServer
{
     [Serializable]
    public class DataTcpServer : TCPServer
    {
       
        public delegate void RecieveHandler(DateTime timestamp, List<UAVParameter> data);
        public event RecieveHandler DataRecieved;
    
        public DataTcpServer(IPAddress ip, int port) : base(ip, port) { 
        
        }

        public DataTcpServer() { 
        
        }
        /// <summary>
        /// Handles a client Connection to the GUI
        /// Responds to Commands from the GUI
        /// </summary>
        /// <param name="client"></param>
        protected override void HandleClientComm(object client)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new
System.Globalization.CultureInfo("en-US");
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            StreamReader reader = new StreamReader(clientStream);
            StreamWriter writer = new StreamWriter(clientStream);
            string line;
            byte[] message = new byte[4096];
            int bytesRead;
            try
            {
                while (true)
                {
                    bytesRead = 0;
                    clientStream.Flush();
                    try
                    {
                        //blocks until a client sends a message
                        
                        line = reader.ReadLine();
                    }
                    catch
                    {
                        //a socket error has occured
                        break;
                    }

                    if (line != null)
                    {
                        string name = line.Split(new char[] { '|' })[0];
                        DateTime timestamp = new DateTime(Convert.ToInt64(line.Split(new char[] { '|' })[1]));
                        string value = line.Split(new char[] { '|' })[2];
                        List<UAVParameter> parameters = new List<UAVParameter>();
                        parameters.Add(new UAVParameter(name, value));
                        if (DataRecieved != null) DataRecieved(timestamp, parameters);
                    }
                    else {
                        break;
                    }
                 
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Disconnect((TcpClient)client);
                tcpClient.Close(); //Close the current Connection

            }
        }

        /// <summary>
        /// Sends a UAVParameter to all clients
        /// </summary>
        /// <param name="param"></param>
        public void Send(UAVSingleParameter param)
        {
          
            foreach (TcpClient myclient in clients)
            {
                if (myclient.Connected)
                {
                    if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US")
                    {

                        System.Threading.Thread.CurrentThread.CurrentCulture = new
    System.Globalization.CultureInfo("en-US");
                    }
                       NetworkStream clientStream = myclient.GetStream();
                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] buffer = encoder.GetBytes(param.GetStringPath() + "|" + DateTime.UtcNow.Ticks.ToString() + "|" + param.Value.ToString() + "\n");
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }
            }
        }

    }
}
