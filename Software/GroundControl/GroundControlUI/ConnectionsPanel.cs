using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;

namespace GroundControlUI
{

    public partial class ConnectionsPanel : UserControl,PersistentData
    {
        GroundControlCore.GroundControlCore core = null;

        /// <summary>
        /// Initialise Object
        /// </summary>
        /// <param name="core"></param>
        public ConnectionsPanel(GroundControlCore.GroundControlCore core)
        {
            InitializeComponent();
            this.core = core;
            cb_Source.SelectedText = "Bodenstation";
            foreach (UAVCommons.CommunicationEndpoint endpoint in core.currentUAV.knownEndpoints)
            {
                endpoint.LatencyUpdated += new UAVCommons.CommunicationEndpoint.LatencyHandler(endpoint_LatencyUpdated);

            }

            UpdateGrid(core.currentUAV.knownEndpoints);
            
            core.currentUAV.CommunicationStatusChanged += new UAVCommons.UAVBase.CommunicationStatusHandler(currentUAV_CommunicationStatusChanged);
        }


        public string PersistentData
        {
            get
            {
                return "";
            }
            set
            {

            }
        }

        /// <summary>
        /// Updates the Table 
        /// </summary>
        /// <param name="core"></param>
        private void UpdateGrid(List<UAVCommons.CommunicationEndpoint> knownEndpoints)
        {
            this.dataGridView1.Rows.Clear();

            //Für alle Kommunikationsverbindungen
            foreach (UAVCommons.CommunicationEndpoint endpoint in knownEndpoints)
            {
                List<object> values = new List<object>();
                values.Add(endpoint.Priority);
                if (endpoint is UAVCommons.TCPCommunicationEndPoint)
                {
                    values.Add("TCP");
                }
                else {
                    values.Add("UDP");
                }

 
                if ((endpoint is UAVCommons.TCPCommunicationEndPoint) && (((UAVCommons.TCPCommunicationEndPoint)endpoint).listen))
                {
                    values.Add("Server");
                }
                else
                {
                    values.Add("Client");

                }
                values.Add(endpoint.endpointSpeedType.ToString());
                values.Add(endpoint.commType.ToString());
                
              
                values.Add(endpoint.Latency.ToString());
                if (endpoint.Connected())
                {
                    values.Add("Verbunden (" + endpoint.ConnectionCount + ")");

                }
                else
                {
                    values.Add("Getrennt");


                }
                this.dataGridView1.Rows.Add(values.ToArray());
                      }
            dataGridView1.Refresh();
        }

        void endpoint_LatencyUpdated(UAVCommons.CommunicationEndpoint source, int ms)
        {
            if (this.dataGridView1.InvokeRequired)
            {
                this.dataGridView1.Invoke((MethodInvoker)delegate
                {
                    UpdateGrid(this.core.currentUAV.knownEndpoints);
                });
            }
            else
            {
                UpdateGrid(this.core.currentUAV.knownEndpoints);
            }
           
        }
        /// <summary>
        /// Something has Changed in our Connections update the Pad!!!
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arg"></param>
        /// <param name="state"></param>
        void currentUAV_CommunicationStatusChanged(UAVCommons.UAVBase source, UAVCommons.CommunicationEndpoint arg,string state)
        {
            try
            {
                if (this.dataGridView1.InvokeRequired)
                {
                    this.dataGridView1.Invoke((MethodInvoker)delegate {
                    UpdateGrid(source.knownEndpoints);
                });
                }
                else
                {
                    UpdateGrid(source.knownEndpoints);
                }
            }
            catch (Exception ex) { }
        }

        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
             
               //core.currentUAV.knownEndpoints
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            ((UAVCommons.CommunicationEndpoint)e.Row.Tag).Disconnect();
            core.currentUAV.knownEndpoints.Remove(((UAVCommons.CommunicationEndpoint)e.Row.Tag));

        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (((UAVCommons.CommunicationEndpoint)e.Row.Tag).Connected())
            {
                if (MessageBox.Show(this, "GroundControl", "Wenn Sie diesen Endpunkt löschen trennen Sie eine bestehende Verbindung zum UAV!\n \n Wollen Sie wirklich fortfahren?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {

                }
                else {
                    e.Cancel = true;
                }
            }
        }
        /// <summary>
        /// Der Benutzer kann zwischen Bodenstation und UAV Einstellungen umschalten 
        /// beim Editieren und Anzeigen der Einstellungen am UAV muss alles über Commands erfolgen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Source_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            lbl_EventsperSec.Text = "Recieved "+ core.currentUAV.RecEventsPerSeond+" Sent "+ core.currentUAV.SendEventsPerSeond;
      

        }
    }
}
