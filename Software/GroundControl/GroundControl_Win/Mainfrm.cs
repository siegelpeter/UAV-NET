using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using WeifenLuo.WinFormsUI.Docking;
namespace GroundControl_Win
{
    public partial class Mainfrm : GroundControlCore.GroundMainform
    {
        private DeserializeDockContent m_deserializeDockContent;

        public Mainfrm()
        {
            InitializeComponent();
        }

        /*    private IDockContent GetContentFromPersistString(string persistString)
            {
                if (persistString == typeof(DummySolutionExplorer).ToString())
                    return m_solutionExplorer;
           
              //  else if (persistString == typeof(DummyTaskList).ToString())
               //     return m_taskList;
                else
                {
                    // DummyDoc overrides GetPersistString to add extra information into persistString.
                    // Any DockContent may override this value to add any needed information for deserialization.

                    string[] parsedStrings = persistString.Split(new char[] { ',' });
                    if (parsedStrings.Length != 3)
                        return null;

                    if (parsedStrings[0] != typeof(DummyDoc).ToString())
                        return null;

                    DummyDoc dummyDoc = new DummyDoc();
                    if (parsedStrings[1] != string.Empty)
                        dummyDoc.FileName = parsedStrings[1];
                    if (parsedStrings[2] != string.Empty)
                        dummyDoc.Text = parsedStrings[2];

                    return dummyDoc;
                }
            }
    */

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.PropertyWindow(), "Properties", WeifenLuo.WinFormsUI.Docking.DockState.DockRight);

            mypad.Show(this.dockPanel1);
        }

        private void Mainfrm_Load(object sender, EventArgs e)
        {
            //string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");

          //  if (File.Exists(configFile))
        //        dockPanel1.LoadFromXml(configFile, m_deserializeDockContent);
        }

        private void Mainfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
           // string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
           // dockPanel1.SaveAsXml(configFile);
        }

        private void attitudeIndicatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
          
      
        }

        private void instrumentPanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Pad mypad = new Pad(new GroundControlUI.InstrumentPanel(), "Instrument Panel", WeifenLuo.WinFormsUI.Docking.DockState.Document);


            mypad.Show(this.dockPanel1);
        }

    }
}
