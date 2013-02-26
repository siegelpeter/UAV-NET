using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading;
using UAVCommons.Events;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;
/*! \mainpage Welcome to the API Documentation of the UAV Project
 *
 * \section intro_sec Introduction
 *
 *  API stands for Application Programming Interface and supplies Functionality which helps the developer to implement his Program
 *  
 * \section help_sec Additional Help
 *
 * \subsection diagramms Diagramms: 
 * 
 * \\Documentation\Onboard\*.ppt
 * 
 * \subsection redmine Redmine Wiki:
 * 
 * You can find following Information in our Wiki in the Redmine Project Management System
 * 
 * Sensor Values and their Max / Min Values
 * Testing System 
 * Setup...
 * etc...
 */

namespace UAVCommons
{
    /// <summary>
    /// The UAV Base class: 
    /// All UAVs extend this class and inherit Communications etc. from it 
    /// </summary>
    [Serializable]
    public abstract class UAVBase : HierachyItem, UAVCommons.Commands.CommandHandler
    {
        public MonitoredDictionary<string, UAVSingleParameter> uavData; // known Data about the UAV ( attitude speed track ...)
        public Dictionary<string, Commands.BaseCommand> SyncCommands = new Dictionary<string, Commands.BaseCommand>();
        Dictionary<string, DateTime> recievedParameters = new Dictionary<string,DateTime>();
        public string uavGuid = null;
        object SynclockCommands = new object();
        object SynclockRecieve = new object();
        public bool initialised = false;
        public DateTime lastsecond = DateTime.Now;
        public int SendEventCounter = 0;
        public int RecieveEventCounter = 0;
        double RecEvents = 0;
        double SendEvents = 0;
        [NonSerialized]
        Timer cleanuptimer;
        [NonSerialized]
        public UpdateRateRegulator mySendRegulator = new UpdateRateRegulator();

		public static bool KeepInRange = false;

        public List<CommunicationEndpoint> knownEndpoints = new List<CommunicationEndpoint>();

      
        /// <summary>
        /// gives the Endpoint where the Status changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="?"></param>
        public delegate void NewLogEntryHandler(object sender, string LogName, string LogMessage);
        public event NewLogEntryHandler NewLogEntry;

        /// <summary>
        /// Logs Message to UAV LogFile
        /// </summary>
        /// <param name="LogMessage"></param>
        public void WriteToLog(string LogMessage)
        {
            WriteToLog(this.ToString(), null, LogMessage);
        }

        /// <summary>
        /// Log Message and Fire NewLogEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="LogName"></param>
        /// <param name="LogMessage"></param>
        public void WriteToLog(object sender, string LogName, string LogMessage)
        {
            if (LogName == null) { LogName = "GroundLog"; }
            if (NewLogEntry != null) NewLogEntry(sender, LogName, LogMessage);
            log4net.ILog logger = log4net.LogManager.GetLogger(LogName);
            logger.Info(LogMessage);
        }
        /// <summary>
        /// gives the Endpoint where the Status changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="?"></param>
        public delegate void CommunicationStatusHandler(UAVBase source, CommunicationEndpoint arg, string state);
        public event CommunicationStatusHandler CommunicationStatusChanged;

        /// <summary>
        /// An Event fired when new Data has arrived
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arg"></param>
        public delegate void DataArrivedHandler(CommunicationEndpoint source, UAVSingleParameter arg);
        public event DataArrivedHandler DataArrived;

        /// <summary>
        /// Constructor
        /// </summary>
        public UAVBase()
        {
            uavData = new MonitoredDictionary<string, UAVSingleParameter>(this);
            uavData.ValueChanged += new MonitoredDictionary<string, UAVSingleParameter>.ValueChangedHandler(uavData_ValueChanged);
            mySendRegulator.forwarded += new UpdateRateRegulator.ForwardEventhandler(SendDataForwardNow);
            CommunicationStatusChanged += new CommunicationStatusHandler(UAVBase_CommunicationStatusChanged);
            
            
         

            
        }

        public void UAVBase_CommunicationStatusChanged(UAVBase source, CommunicationEndpoint arg, string state)
        {

        }


        public double RecEventsPerSeond {
            get
            {
                tryCalcPerSecond();
                return RecEvents;
            }
        }

        public double SendEventsPerSeond
        {
            get
            {
                tryCalcPerSecond();
                return SendEvents;
            }
        }

        private void tryCalcPerSecond()
        {
            if (lastsecond <= DateTime.Now.AddSeconds(-1))
            {
                RecEvents = RecieveEventCounter / DateTime.Now.Subtract(lastsecond).TotalSeconds;
                SendEvents = SendEventCounter / DateTime.Now.Subtract(lastsecond).TotalSeconds;
                lastsecond = DateTime.Now;
                RecieveEventCounter = 0;
                SendEventCounter = 0;
            }
        }

        /// <summary>
        /// This Method is used to send the filtered Eventstream to the CommunicationEndpoints
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        void SendDataForwardNow(UAVSingleParameter param, bool isremote)
        {
		
			try{
            if (!isremote)
            {
                foreach (CommunicationEndpoint myendpoint in knownEndpoints)
                {
                    //if (param is UAVStructure)
                    //{
					
                    //    // param.ValueChanged+=new UAVParameter.ValueChangedHandler(param_ValueChanged);
                    //    foreach (UAVSingleParameter myparam in ((UAVStructure)param).values.Values)
                    //    {
                    //        myendpoint.SendData(myparam);
                    //        SendEventCounter++;
                    //    }

                    //}
                  
						
                        myendpoint.SendData(param);
                        SendEventCounter++;
                    
                }
            }
			}catch (Exception ex){
				Console.WriteLine("Error on send forward "+ex.Message+ex.StackTrace.ToString());

			}
        }

        /// <summary>
        /// Something has been changed to queue the Event /filter it using the Send Regulator
        /// </summary>
        /// <param name="param">The Parameter Object which has been changed</param>
        /// <param name="isremote">A boolean Variable to determine if the event comes from here or is an remote event</param>
        public void uavData_ValueChanged(UAVSingleParameter param, bool isremote)
        {
			
            if (!isremote) {
				mySendRegulator.EventRecieved(param, isremote);
        }
		}

		public void ConnectWorker(){
			Connect();
			
		}
		
        /// <summary>
        /// Establishes Communication with the UAV 
        /// 
        /// </summary>
        /// <returns>Returns False if the Communication to the UAV is unavailable</returns>
        public bool Connect()
        {
			Console.WriteLine("Connecting Network");
            bool CommunicationEstablished = false;
            foreach (CommunicationEndpoint mycomm in knownEndpoints)
            {
                mycomm.Connect();
                {
                    CommunicationEstablished = true;
                    if (mycomm.commType == CommunicationEndpoint.Communicationtype.Recieve)
                    {
                        ((CommunicationEndpoint)mycomm).DataRecieved += new CommunicationEndpoint.RecieveHandler(mycomm_DataRecieved);
                      }
                    else if (mycomm.commType == CommunicationEndpoint.Communicationtype.Send)
                    {
                        ((CommunicationEndpoint)mycomm).Sendall(new List<UAVSingleParameter>(this.uavData.Values).ToArray());
                  
                    }
                    else if (mycomm.commType == CommunicationEndpoint.Communicationtype.Command)
                    {
                        ((CommunicationEndpoint)mycomm).DataRecieved += new CommunicationEndpoint.RecieveHandler(mycomm_DataRecieved);
                              }
                    if (mycomm is TCPCommunicationEndPoint) ((TCPCommunicationEndPoint)mycomm).StateChanged += new TCPCommunicationEndPoint.ConnectionStateChangeHandler(UAV_CommunicationStatusChanged);
               //     ((TCPCommunicationEndPoint)mycomm).LatencyUpdated += new TCPCommunicationEndPoint.LatencyHandler(mycomm_LatencyUpdated);
                
      
                }
            }
			Console.WriteLine("done");
          
            return CommunicationEstablished;
        }



        public virtual void UAV_CommunicationStatusChanged(CommunicationEndpoint source, string state)
        {
            if (this.CommunicationStatusChanged != null) this.CommunicationStatusChanged(this, source, state);
      
        }
                    
            

        /// <summary>
        /// We have recieved Data from an CommunicationEndpoint, now we process it
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="data"></param>
        private void mycomm_DataRecieved(DateTime timestamp, List<UAVParameter> data)
        {
            if (initialised)
            {
                lock (SynclockRecieve)
                {
                    foreach (UAVParameter param in data)
                    {
                        string name = "";
                       // Console.WriteLine(param.Name + ": " + param.Value);
                        RecieveEventCounter++;
                        if (recievedParameters.ContainsKey(timestamp.Ticks.ToString() + param.Name)) continue;
                        UAVSingleParameter result = uavData.SilentUpdate(param.Name, param.Value, true);
                        if (DataArrived != null) DataArrived(null, result);
                        recievedParameters.Add(timestamp.Ticks.ToString() + param.Name, timestamp);

                    }

                    List<string> deletelist = new List<string>();
                    foreach (KeyValuePair<string, DateTime> pair in recievedParameters)
                    {
                        if (pair.Value < DateTime.UtcNow.AddSeconds(-10))
                        {
                            deletelist.Add(pair.Key);
                        }
                    }

                    foreach (string key in deletelist)
                    {
                        recievedParameters.Remove(key);
                    }
                    deletelist.Clear();
                }
            }
        }


        /// <summary>
        /// Disconnect all Communication Connections
        /// </summary>
        public void Disconnect()
        {
            foreach (CommunicationEndpoint mycomm in knownEndpoints)
            {
                mycomm.Disconnect();
            }
        }


        /// <summary>
        /// Ein neuer Kommand ist über eine Kommunikationsschnittstelle angekommen
        /// kümmern wir uns darum!
        /// </summary>
        /// <param name="client"></param>
        /// <param name="Command"></param>
        public virtual void CommandRecieved(System.Net.Sockets.TcpClient client, Commands.BaseCommand recievedCommand)
        {
			Console.WriteLine(recievedCommand.GetType().ToString());
			try{
            if (recievedCommand.completed)
            {
                recievedCommand.ProcessResults(this);
                if (!recievedCommand.isAsync)
                {
                    lock (SynclockCommands)
                    {
                        SyncCommands.Add(recievedCommand.guid, recievedCommand);
                    }
                }
            }
            else
            {
					Console.WriteLine("RemoteExec");
                recievedCommand.RemoteExecute(this);
                recievedCommand.completed = true;
                BinaryFormatter formatter = new BinaryFormatter();
					MemoryStream memstr = new MemoryStream();
                formatter.Serialize(memstr, recievedCommand);
					var outputstream = client.GetStream();
						outputstream.Write(memstr.ToArray(),0,Convert.ToInt32(memstr.Length));
					outputstream.Flush();
                
            }
			}catch (Exception ex){
				Console.WriteLine(ex.Message+ " "+ ex.StackTrace.ToString());

			}
        }

        /// <summary>
        /// Send a Command but does not wait for it to succeed and come back
        /// </summary>
        /// <param name="cmd"></param>
        public void SendCommandAsync(Commands.BaseCommand cmd)
        {
            cmd.Send(this);
            foreach (CommunicationEndpoint mycomm in knownEndpoints)
            {
                if (mycomm.Connected())
                {

                    if (mycomm.commType == CommunicationEndpoint.Communicationtype.Command)
                    {
                        mycomm.SendCommand(cmd);
                    }

                }
            }
        }

        /// <summary>
        /// Sends a Command and waits until it comes back with the Results
        /// </summary>
        /// <param name="cmd">An Command object to be sent, containing code and values</param>
        /// <returns>The resulting Command object which came back</returns>
        public Commands.BaseCommand SendCommand(Commands.BaseCommand cmd)
        {
            //Sende Kommando an alle Endpunkte
            bool sent = false;
            cmd.Send(this);
            foreach (CommunicationEndpoint mycomm in knownEndpoints)
            {
                if (mycomm.Connected())
                {

                    if (mycomm.commType == CommunicationEndpoint.Communicationtype.Command)
                    {
                        sent = true;
                        mycomm.SendCommand(cmd);
                    }

                }
            }
            if (!sent) return cmd;
            // Warte auf ankunft des Ergebnisses
            bool CommandRecieved = false;
            do
            {
                lock (SynclockCommands)
                {
                    if (SyncCommands.ContainsKey(cmd.guid))
                    {
                        CommandRecieved = true;
                    }
                }
                Thread.Sleep(5);
            } while (!CommandRecieved);

            ///Remove recieved Command from store
            lock (SynclockCommands)
            {
                Commands.BaseCommand recievedCmd = SyncCommands[cmd.guid];
                SyncCommands.Remove(cmd.guid);
                return recievedCmd;

            }


        }

   


        /// <summary>
        /// Saves the Given UAV to a given Stream
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Stream Save(Stream target, UAVBase source)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(target, source);
            return target;
        }

        /// <summary>
        /// Saves the Current UAV Object to the File specified: rcuav.uav is loaded by default on startup
        /// </summary>
        /// <param name="filename">A String representing a filename or an absolute Path</param>
        public void Save(string filename)
        {
            using (FileStream filestream = File.OpenWrite(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                this.OnSave();
                formatter.Serialize(filestream, this);
            }
        }

		public static void SaveValues(Stream target, UAVBase source)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US")
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new
    System.Globalization.CultureInfo("en-US");
            }

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("UAVData");
            doc.AppendChild(root);

            foreach (var key in  source.uavData.Keys)
            {
               source.uavData[key].SavetoXML(root,doc);
        }
            doc.Save(target);
        }

            public static void LoadValues(Stream source,UAVBase target)
            {
                if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US")
                {
                    System.Threading.Thread.CurrentThread.CurrentCulture = new
        System.Globalization.CultureInfo("en-US");
                }

                if (source.Length == 0) return;
                XmlDocument doc = new XmlDocument();
                doc.Load(source);
                XmlElement elem = (XmlElement)doc.SelectSingleNode("UAVData");
                LoadValues(elem,target);
                }

        public static void LoadValues(XmlElement elem, UAVBase target)
        {
            if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US")
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new
    System.Globalization.CultureInfo("en-US");
            }
            foreach (XmlElement val in  elem.ChildNodes)
            {
                LoadValues(val,target);
            }
            string value = null;
            string path = null;
            try
            {
                 path = elem.GetAttribute("Path");
                if (elem.HasAttribute("Value")) {
					string strval = elem.GetAttribute("Value");
					if (strval == "[[NULL]]"){
						value = null;
					}else{
						value = strval;
					}
				}
                if (path == "") return;
                UAVSingleParameter param = target.uavData.SilentUpdate(path, value, false);
                param.LoadValues(elem);
                if (elem.HasAttribute("Max")) param.Max = elem.GetAttribute("Max");
                if (elem.HasAttribute("Min")) param.Min = elem.GetAttribute("Min");
                if (elem.HasAttribute("URate")) param.updateRate = Convert.ToInt32(elem.GetAttribute("URate"));
            }
            catch (Exception ex) {
                
                Console.WriteLine("Error on loading Values for "+path+ " value was "+ value);
                throw ex;
            }
          }



        /// <summary>
        /// This Method is called before the Object is saved 
        /// </summary>
        public virtual void OnSave()
        {
        
        }

        /// <summary>
        /// Loads an Instance of a UAV from file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static UAVBase Load(string filename)
        {
            using (FileStream filestream = File.OpenRead(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                UAVBase uav = (UAVBase)formatter.Deserialize(filestream);
                uav.OnLoad();
                return uav;
            }
            

        }

        /// <summary>
        /// Lade UAV von Datei 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static UAVBase Load(Stream source)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (UAVBase)formatter.Deserialize(source);
        }


		public abstract void run ();


        public virtual void Stop()
        {
            mySendRegulator.Stop();
            foreach (var endpoint in knownEndpoints)
            {
                endpoint.Disconnect();
            }
        }

        /// <summary>
        /// This Method is called after the UAV has been Loaded
        /// </summary>
        public virtual void OnLoad()
        {
            
            ConnectHardware();
		   ThreadStart threadDelegate = new ThreadStart(ConnectWorker);
        Thread connecter = new Thread(threadDelegate);
			connecter.Start();
			
        }

        /// <summary>
        /// Used to connect to the Hardware on Initalisation and on Load
        /// </summary>
        private void ConnectHardware()
        {
            foreach (UAVSingleParameter device in uavData.Values)
            {
                if (device is IHardwareConnectable)
                {
					Console.WriteLine("Connecting "+device.Name);
                    ((IHardwareConnectable)device).ConnectHardware();
                }
            }
			Console.WriteLine("All Hardware Connected");
        }

        public override string Name
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        public override HierachyItem Parent
        {
            get
            {
                return null;    
            }
            set
            {
              
            }
        }
    }
}
