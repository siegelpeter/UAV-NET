using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using UAVCommons.Datamodel;
using UAVCommons;

namespace GroundControlUI
{

    public partial class PropertyWindow : UserControl, PersistentData
    {
        public GroundControlCore.GroundControlCore core = null;
        List<string> selectedProperties = new List<string>();
        public PropertyWindow(GroundControlCore.GroundControlCore core)
        {
            InitializeComponent();
            this.core = core;
            btn_all_Click(null,null);
            propertyGrid.SelectedObject = new DictionaryPropertyGridAdapter(core.currentUAV.uavData,selectedProperties);
        
     
        }


        public string PersistentData
        {
            get
            {
                return String.Join(",",selectedProperties);
            }
            set
            {
                this.selectedProperties.Clear();
                this.selectedProperties.AddRange(value.Split(','));

            }
        }

     

        void currentUAV_DataArrived(UAVCommons.CommunicationEndpoint source, UAVCommons.UAVParameter arg)
        {
       
        }

        private void propertyGrid_Click(object sender, EventArgs e)
        {
            
        }

        private void btn_all_Click(object sender, EventArgs e)
        {
            this.selectedProperties.Clear();
            List<UAVSingleParameter> parameters = MonitoredDictionary<string, UAVParameter>.NormaliseDictionary(core.currentUAV.uavData);
            foreach (UAVSingleParameter param in parameters)
            {
                this.selectedProperties.Add(param.Name);
            }
          
            propertyGrid.SelectedObject = new DictionaryPropertyGridAdapter(core.currentUAV.uavData, selectedProperties);
        }

        private void btn_Choose_Click(object sender, EventArgs e)
        {
            PropertyChooser frm = new PropertyChooser(this.core, this.selectedProperties);
            frm.ShowDialog();
            this.selectedProperties = frm.choosenKeys;
            propertyGrid.SelectedObject = new DictionaryPropertyGridAdapter(core.currentUAV.uavData, selectedProperties);
        
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {

            try
            {

                propertyGrid.Refresh();
               propertyGrid.Update();


            }
            catch (Exception ex)
            {

            }
            
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            updateTimer_Tick(sender,null);
            if (updateTimer.Enabled)
            {
                updateTimer.Enabled = false;
                btn_update.Text = "AutoUpdate";
                btn_update.BackColor = Color.DarkGray;
            }
            else {
                updateTimer.Enabled = true;
                btn_update.Text = "Edit";
                btn_update.BackColor = btn_Choose.BackColor;
            }
        }
    }
}