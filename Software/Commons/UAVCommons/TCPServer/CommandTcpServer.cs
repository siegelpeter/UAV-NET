using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using UAVCommons.Commands;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace UAVCommons.TCPServer
{
     [Serializable]
    public class CommandTcpServer : TCPServer
    {
     
        CommandHandler cmdHandler = null;
        public CommandTcpServer(CommandHandler myhandler, IPAddress ip, int port)
            : base(ip, port)
        {
            cmdHandler = myhandler;
        }

        public CommandTcpServer() { 
        
        }

        protected override void HandleClientComm(object client)
        {

            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();
            string line = null;
            byte[] message = new byte[4096];
            int bytesRead;
            
            while (true)
            {
                bytesRead = 0;

                try
                {
                    
                    //message has successfully been received
                    BinaryFormatter formatter = new BinaryFormatter();
                    Commands.BaseCommand recievedCommand = (Commands.BaseCommand)formatter.Deserialize(clientStream);
                    
                    cmdHandler.CommandRecieved(tcpClient, recievedCommand);
                }
                catch (SerializationException ex)
                {
                    Console.WriteLine("Cannot recieve Command:"+ex.Message);
                    break;
                    // Unable to Understand Command?
                }
                catch (Exception ex) {

                    //a socket error has occured

                    break;
                }


            }
            Disconnect((TcpClient)client);
           
            tcpClient.Close();
        }


        public void SendCommand(BaseCommand cmd)
        {
            TcpClient lastclient = null;
            lock (lockclients)
            {
                try
                {
                    foreach (TcpClient myclient in clients)
                    {
                        lastclient = myclient;
                        if (myclient.Connected)
                        {
                            NetworkStream clientStream = myclient.GetStream();
                            if (clientStream != null)
                            {
                                BinaryFormatter formatter = new BinaryFormatter();
                                formatter.Serialize(clientStream, cmd);
                                clientStream.Flush();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    lock (lockclients)
                    {
                        clients.Remove(lastclient);
                    }
                }
            }
        }

    }
}
