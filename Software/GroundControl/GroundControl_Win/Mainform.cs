using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using GroundControlUI;
using UAVCommons.Commands;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

namespace GroundControl_Win
{
    public partial class Mainform : GroundControlCore.GroundMainform
    {
        private DeserializeDockContent m_deserializeDockContent;
        private Nullable<int> videoPid = null;
        public Mainform()
        {
            InitializeComponent();
            m_deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
			try{
            core.currentUAV.Connect();
			}catch (Exception ex){
				MessageBox.Show("Verbindungsfehler: Netzwerkkarte nicht eingesteckt?");				
			}
				
				try{
                    if (File.Exists("DefaultLayout.dat")) 
                    {
                        using (FileStream stream = File.OpenRead("DefaultLayout.dat")){
                            UAVCommons.UAVBase.LoadValues(stream, core.currentUAV);
                            core.currentUAV.initialised = true;
                        }
                        }

            if (File.Exists("DefaultLayout.uav")) this.dockPanel1.LoadFromXml("DefaultLayout.uav", m_deserializeDockContent);
			}catch (Exception ex){
			MessageBox.Show("Fehler beim Laden des Standart Layouts"+ex.Message,"GroundControl");	
			}
			}


        Pad axiompad = null;



        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.PropertyWindow(core), "UAV Properties", WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            mypad.Show(this.dockPanel1);
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //            Pad mypad = new Pad(new GroundControlUI.PropertygraphControl(core), "Graph", WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            //          mypad.Show(this.dockPanel1);

            //  Pad mypad1 = new Pad(new GroundControlUI.PropertyWindow(core), "UAV Properties", WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            //  mypad1.Show(this.dockPanel1);

            //   Pad mypad2 = new Pad(new GroundControlUI.LogViewer(core,"GroundLog"), "GroundLog", WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            //   mypad2.Show(this.dockPanel1);

            //        Pad mypad3 = new Pad(new GroundControlUI.InstrumentPanel(core), "Instrument Panel", WeifenLuo.WinFormsUI.Docking.DockState.Document);
            //      mypad3.Show(this.dockPanel1);

            //  Pad mypad4 = new Pad(new GroundControlUI.ControlPanel(core), "Value Control Panel", WeifenLuo.WinFormsUI.Docking.DockState.Document);
            //  mypad4.Show(this.dockPanel1);

            //     Pad mypad6 = new Pad(new GroundControlUI.LogViewer(core, "FlightLog"), "FlightLog", WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            //     mypad6.Show(this.dockPanel1);

        }

        private void groundLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logviewer = new GroundControlUI.LogViewer(core, "GroundLog");
            logviewer.ShowConnectionsStateChanges = true;
            Pad mypad = new Pad(logviewer, "GroundLog", WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            
            mypad.Show(this.dockPanel1);

        }

        private void chartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.PropertygraphControl(core), "Graph", WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            mypad.Show(this.dockPanel1);

        }

        private void instrumentPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.InstrumentPanel(core), "Instrument Panel", WeifenLuo.WinFormsUI.Docking.DockState.Document);
            mypad.Show(this.dockPanel1);

        }

        private void Mainform_Load(object sender, EventArgs e)
        {

        }

   
      

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void kommunikationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.ConnectionsPanel(core), "Verbindungen", WeifenLuo.WinFormsUI.Docking.DockState.DockRight);
            mypad.Show(this.dockPanel1);

        }

        private void werteSetzenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.ControlPanel(core), "Value Control Panel", WeifenLuo.WinFormsUI.Docking.DockState.Document);
            mypad.Show(this.dockPanel1);

        }

        private void flightLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logviewer = new GroundControlUI.LogViewer(core, "FlightLog");
            logviewer.ShowConnectionsStateChanges = false;

            Pad mypad = new Pad(logviewer, "Flight Log", WeifenLuo.WinFormsUI.Docking.DockState.Document);
            mypad.Show(this.dockPanel1);

        }

        private void missionPlanerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.MapView(core), "Map Designer", WeifenLuo.WinFormsUI.Docking.DockState.Document);
            mypad.Show(this.dockPanel1);


        }

        private void Mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            core.currentUAV.Stop();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "UAV Projekt (*.uav)|*.uav";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string configFile = saveFileDialog1.FileName;
                string datfile = Path.Combine(Path.GetDirectoryName(configFile), Path.GetFileNameWithoutExtension(configFile) + ".dat");
                if (File.Exists(configFile))
                {
                    File.Delete(configFile);
                }
                if (File.Exists(datfile)) {
                    File.Delete(datfile);
                }
                
                using (FileStream stream = File.OpenWrite(datfile)){
                    UAVCommons.UAVBase.SaveValues(stream, core.currentUAV);
                }
                this.dockPanel1.SaveAsXml(configFile);
            }
        }

        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            openFileDialog1.Filter = "UAV Projekt (*.uav)|*.uav";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string configFile = openFileDialog1.FileName;
                if (File.Exists(configFile))
                {
                    core.currentUAV.uavData.Clear();
                    using (
                     
                       FileStream stream =
                            File.OpenRead(Path.Combine(Path.GetDirectoryName(configFile),
                                                       Path.GetFileNameWithoutExtension(configFile) + ".dat")))
                    {
                        UAVCommons.UAVBase.LoadValues(stream, core.currentUAV);
                        core.currentUAV.initialised = true;
                    }
                    this.dockPanel1.LoadFromXml(configFile, m_deserializeDockContent);

                }
            }
        }


        private IDockContent GetContentFromPersistString(string persistString)
        {

            // DummyDoc overrides GetPersistString to add extra information into persistString.
            // Any DockContent may override this value to add any needed information for deserialization.
            try
            {
                string[] parsedStrings = persistString.Split(new char[] { ';' });
                Assembly assm = Assembly.Load(parsedStrings[0].Split(',')[0]);
                Type t = assm.GetType(parsedStrings[1], true);
                BindingFlags flags = BindingFlags.Default;
                Pad newpad = new Pad((UserControl)Activator.CreateInstance(parsedStrings[0].Split(',')[0], parsedStrings[1], true, flags, null, new object[] { this.core }, null, null).Unwrap(), parsedStrings[2], DockState.Float);
                ((GroundControlUI.PersistentData)newpad.basecontrol).PersistentData = parsedStrings[3];
                return newpad;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Opens the Joystick Settings Window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void joystickEinstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroundControlUI.Forms.JoystickSettings frm = new GroundControlUI.Forms.JoystickSettings((FlightControlCommons.UAVJoystick)((GroundControlCore.GroundControlCore)core).stick, ((GroundControlCore.GroundControlCore)core).mapping);
            if (frm.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

    

        /// <summary>
        /// Fetches Infomation about Remote UAV Parameters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GroundControlCore.GroundControlCore core = this.core;
            core.currentUAV.SendCommandAsync(new UAVCommons.Commands.GetParameters());
            //  this.SendCommand(new WriteToFlightLog("Testing Sync Command",DateTime.Now));
            //    WriteToFlightLog log =new WriteToFlightLog("Testing Sync Command",DateTime.Now);
            //    log.isAsync = true;
            //     this.SendCommand(log);

        }

        /// <summary>
        /// Opens an About Dialog showing information about the Author
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GroundControlUI.Forms.AboutForm frm = new GroundControlUI.Forms.AboutForm();
            frm.ShowDialog(this);
        }

        /// <summary>
        /// Initiates Saving of the current uav to a file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_RemoteSave_Click(object sender, EventArgs e)
        {
            core.currentUAV.SendCommandAsync(new UAVCommons.Commands.SaveUAV());

        }

        /// <summary>
        /// Start Playing Video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_play_Click(object sender, EventArgs e)
        {

            UAVCommons.Commands.StartVideoStream startvideo = new StartVideoStream("10.0.0.12", 4444, 15, 640, 480);
          
            core.currentUAV.SendCommand(startvideo);
            
        }

        /// <summary>
        /// Stop playing Video
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_stop_Click(object sender, EventArgs e)
        {
            if (videoPid.Value != 0)
            {
                KillProcess killproc = new KillProcess(videoPid.Value);
                core.currentUAV.SendCommand(killproc);
                //TODO kill local Video Player too
            }
        }

        private void flightToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void onBoardSystemOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UAVOSOptionsFrm frm = new UAVOSOptionsFrm(core);
            frm.ShowDialog();
        }












        private void dPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _3DViewer ucontrol = new _3DViewer(core.currentUAV);

            Pad threedPad = new Pad(ucontrol, "3D Viewer", DockState.Document);
            threedPad.Show();
            threedPad.TopMost = true;
        }

        private void eFISToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EFISViewer ucontrol = new EFISViewer(core);

            Pad threedPad = new Pad(ucontrol, "EFIS", DockState.Document);
            threedPad.Show();
            threedPad.TopMost = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            core.currentUAV.uavData.Clear();
        }

        private void saveAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            saveFileDialog1.OverwritePrompt = true;
            saveFileDialog1.Filter = "UAV data (*.dat)|*.dat";
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {

                string datfile = saveFileDialog1.FileName;
                if (File.Exists(datfile))
                {
                    File.Delete(datfile);
                }

                using (FileStream stream = File.OpenWrite(datfile))
                {
                    UAVCommons.UAVBase.SaveValues(stream, core.currentUAV);
                }
             }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            openFileDialog1.Filter = "UAV Data (*.dat)|*.dat";
            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string configFile = openFileDialog1.FileName;
                if (File.Exists(configFile))
                {
                    core.currentUAV.uavData.Clear();
                    using (

                       FileStream stream =
                            File.OpenRead(configFile))
                    {
                        UAVCommons.UAVBase.LoadValues(stream, core.currentUAV);
                        core.currentUAV.initialised = true;
                    }
                  
                }
            }
       
        }


    }
}
