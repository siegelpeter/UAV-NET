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
    public class TcpDataClient : TcpBaseClient
    {
        public delegate void RecieveHandler(DateTime timestamp, List<UAVParameter> data);
        public event RecieveHandler DataRecieved;
        [NonSerialized]
        Thread recieveThread;

        public TcpDataClient(string adr, int port, CommunicationEndpoint.Communicationtype mode)
            : base(adr, port)
        {
            this.mode = mode;

        }

        public override bool Connect()
        {
            base.Connect();
            if (this.mode == CommunicationEndpoint.Communicationtype.Recieve)
            {
                recieveThread = new Thread(new ThreadStart(Recieve));
                recieveThread.Start();
            }
            return true;
        }

        public void Send(UAVSingleParameter param)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US")
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new
    System.Globalization.CultureInfo("en-US");
            }
            if (param.Value == null) return;
            lock (SendSync)
            {
                if (clientStream != null)
                {
                    UTF8Encoding encoder = new UTF8Encoding();
                    byte[] buffer = encoder.GetBytes(param.GetStringPath() + "|" + DateTime.UtcNow.Ticks.ToString() + "|" + param.Value.ToString() + "\n");
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }
            }
        }

        public void Recieve()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new
System.Globalization.CultureInfo("en-US");
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            StreamReader reader = new StreamReader(clientStream);
            StreamWriter writer = new StreamWriter(clientStream);
            string line = "";
            byte[] message = new byte[4096];
            int bytesRead;
            try
            {
                while (true)
                {
               
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


                    // UTF8Encoding encoder = new UTF8Encoding();
                    //   string commandl = encoder.GetString(message, 0, bytesRead);


                    // Checking for Commands from the GUI
                    //     if (line.StartsWith(";"))   // Creates a new Instance
                    if ((line != null)&&(line.Contains("|")))
                    {
                        string name = line.Split(new char[] { '|' })[0];
                        DateTime timestamp = new DateTime(Convert.ToInt64(line.Split(new char[] { '|' })[1]));
                        string value = line.Split(new char[] { '|' })[2];
                        List<UAVParameter> parameters = new List<UAVParameter>();
                        parameters.Add(new UAVParameter(name, value));
						try{
                        if (DataRecieved != null) DataRecieved(timestamp, parameters);
						}catch(Exception ex){
								Console.WriteLine("Error on Recieve Value (Could not process event)"+ex.Message+ex.StackTrace+line);  

						}
                    }

                }
            }
            catch (Exception ex)
            {
				Console.WriteLine("Error on Recieve Value "+ex.Message+ex.StackTrace+line);  
            }
            finally
            {

                tcpClient.Close(); //Close the current Connection
                Disconnect();
            }
        }

    }
}