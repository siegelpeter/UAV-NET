using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace GroundControlUI
{

    public partial class LogViewer : UserControl, PersistentData
    {
        GroundControlCore.GroundControlCore core = null;
        string LogName = null;
        ConcurrentQueue<string> insertqueue = new ConcurrentQueue<string>();
        public LogViewer(GroundControlCore.GroundControlCore core)
        {
            InitializeComponent();
            this.core = core;
            LogName = "";
            core.currentUAV.NewLogEntry += new UAVCommons.UAVBase.NewLogEntryHandler(currentUAV_NewLogEntry);
            logbox.Items.Clear();
            core.currentUAV.CommunicationStatusChanged += new UAVCommons.UAVBase.CommunicationStatusHandler(currentUAV_CommunicationStatusChanged);
            
       
        }
        
        public LogViewer(GroundControlCore.GroundControlCore core,string LogName)
        {
            InitializeComponent();
            this.core = core;
            this.LogName = LogName;
            core.currentUAV.NewLogEntry += new UAVCommons.UAVBase.NewLogEntryHandler(currentUAV_NewLogEntry);
            logbox.Items.Clear();
            core.currentUAV.CommunicationStatusChanged += new UAVCommons.UAVBase.CommunicationStatusHandler(currentUAV_CommunicationStatusChanged);
        }


        public string PersistentData
        {
            get
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                LogData data = new LogData();
                data.Logname = this.LogName;
                
                foreach (var item in logbox.Items){
                    data.messages.Add(item.ToString());
                }

                formatter.Serialize(stream, data);

                return Convert.ToBase64String(stream.ToArray());
            }
            set
            {
                BinaryFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                byte[] data = Convert.FromBase64String(value);
                stream.Write(data, 0, data.Length);
                var logdata = (LogData)formatter.Deserialize(stream);
                this.LogName = logdata.Logname;
                logbox.Items.Clear();
                foreach (var item in logdata.messages) {
                    logbox.Items.Add(item);
                }

            }
        }

        void currentUAV_NewLogEntry(object sender, string LogName, string LogMessage)
        {
                if (LogName.ToLower() == this.LogName.ToLower())
                {
                    insertqueue.Enqueue(LogMessage);
                }
       
        }

        void currentUAV_CommunicationStatusChanged(UAVCommons.UAVBase source, UAVCommons.CommunicationEndpoint arg,string state)
        {
            if (ShowConnectionsStateChanges)
            {
                string connectedstring = "";
                if (arg.Connected())
                {
                    connectedstring = " established";
                }
                else
                {
                    connectedstring = " lost";
                }
                if (logbox.InvokeRequired)
                {
                    logbox.Invoke((MethodInvoker)delegate { logbox.Items.Insert(0, "Verbindung mit uav:" + arg.commType.ToString() + connectedstring); });

                }
            }
        }

        private void logbox_SizeChanged(object sender, EventArgs e)
        {
            if (logbox.Items.Count == 0)
                return;
            //This selects and highlights the last line
            this.logbox.SetSelected(logbox.Items.Count - 1, true);
            //This unhighlights the last line
            this.logbox.SetSelected(logbox.Items.Count - 1, false);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string message = null;
            logbox.BeginUpdate();

            while(insertqueue.TryDequeue(out message)){
                if (logbox.InvokeRequired)
                {
                    logbox.Invoke((MethodInvoker)delegate { logbox.Items.Insert(0, message); });

                }
                else
                {
                    logbox.Items.Insert(0, message);
                }
            }
            logbox.EndUpdate();
        }

        public bool ShowConnectionsStateChanges { get; set; }
    }
}
