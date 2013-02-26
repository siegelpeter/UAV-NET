using System;
using System.Collections.Generic;

using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace UAVCommons.TCPServer
{
    [Serializable]
    public abstract class TCPServer
    {
        public IPAddress IP;
        public int Port;
        private TcpListener tcpListener;
          [NonSerialized]
        private Thread listenThread;

        public List<TcpClient> clients = new List<TcpClient>();
        public object lockclients = new object();
        public delegate void ConnectionStateChangeHandler(TCPServer source, string state);
        public event ConnectionStateChangeHandler StateChanged;
        bool isrunning = true;
        public TCPServer(IPAddress ip, int port)
        {
            this.IP = ip;
            this.Port = port;
            this.tcpListener = new TcpListener(ip, port);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        public TCPServer() {
            this.tcpListener = new TcpListener(IP, Port);
            this.listenThread = new Thread(new ThreadStart(ListenForClients));
            this.listenThread.Start();
        }

        private void ListenForClients()
        {
          this.tcpListener.Start();

            while (isrunning)
            {
                try{
                //blocks until a client has connected to the server
                TcpClient client = this.tcpListener.AcceptTcpClient();
                lock (lockclients)
                {
                    clients.Add(client);

                }
                
                //create a thread to handle communication
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
                if (StateChanged != null) StateChanged(this, "Connected");
            }catch(SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            }
        }

        public void Stop() {
            isrunning = false;
            tcpListener.Stop();
        }


        protected abstract void HandleClientComm(object client);

        public void Disconnect(TcpClient tcpClient)
        {
            lock (lockclients)
            {
                clients.Remove(tcpClient);
            }
            if (StateChanged != null) StateChanged(this, "Disconnected");
        }


    }
}
