using System;
using System.Collections.Generic;

using System.Text;
using UAVCommons.TCPServer;
using UAVCommons.Commands;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace UAVCommons
{
     [Serializable]
    public class TcpCommandClient:TcpBaseClient
    {
     
        CommandHandler cmdHandler = null;
        public TcpCommandClient(string adr, int port,CommandHandler cmdhandler)
            : base(adr, port)
        {
            this.cmdHandler = cmdhandler;
        }


        public override bool Connect()
        {
             base.Connect();
            Thread recieveThread = new Thread(new ThreadStart(Recieve));

             recieveThread.Start();
             return true;
        }
            
        public void Send(BaseCommand cmd)
        {
            lock (SendSync)
            {
                if (client.Connected)
                {
                    if (clientStream != null)
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(clientStream, cmd);
                        clientStream.Flush();
                    }
                }
            }
        }

        public void Recieve()
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            string line = "";
            byte[] message = new byte[4096];
            int bytesRead;
            try
            {
                while (true)
                {
                    Thread.Sleep(0);
                    bytesRead = 0;
                    clientStream.Flush();
                    try
                    {
                        //blocks until a client sends a message
                        UTF8Encoding encoder = new UTF8Encoding();
                        BinaryFormatter formatter = new BinaryFormatter();
                        Commands.BaseCommand recievedCommand = (Commands.BaseCommand)formatter.Deserialize(clientStream);
						 //   System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
                        cmdHandler.CommandRecieved(tcpClient, recievedCommand);
                    }
                    catch (Exception ex)
                    {
						Console.WriteLine("Error on Recieve Command"+ex.Message);
                        //a socket error has occured
                        break;
                    }


                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                
                tcpClient.Close(); //Close the current Connection
                Disconnect();
            }
        }

        }
    }

