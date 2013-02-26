using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using UAVCommons;
using System.Text;
using System.Windows.Forms;

namespace GroundControlUI
{
    public partial class PropertyChooser : Form
    {

        public List<string> choosenKeys = new List<string>();

        /// <summary>
        /// Fill list with all properties
        /// </summary>
        /// <param name="core"></param>
        public PropertyChooser(GroundControlCore.GroundControlCore core,List<string> defaultKeys)
        {
            InitializeComponent();
           // checkedListBox1.Items = core.currentUAV.uavData.Values;
            CheckedParameters.Items.Clear();
            choosenKeys = defaultKeys;
            List<UAVSingleParameter> uavData = MonitoredDictionary<string, UAVSingleParameter>.NormaliseDictionary(core.currentUAV.uavData);
            foreach (UAVSingleParameter param in uavData)
            {
                bool checkeditem = false;
                if (defaultKeys.Contains(param.Name)) checkeditem = true;
                CheckedParameters.Items.Add(param.Name,checkeditem);
            }
            
        }

      

      
                
    


        private void btn_OK_Click(object sender, EventArgs e)
        {
            choosenKeys.Clear();
            foreach (string item in CheckedParameters.CheckedItems) {
                choosenKeys.Add(item);
            }
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void CheckedParameters_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int i = 0;
            foreach (string name in CheckedParameters.Items){
             //   if (name.Contains(CheckedParameters.Items[e.Index].ToString())) CheckedParameters.SetItemChecked(i, true);
                i++;
         //   if (name.Contains(CheckedParameters.Items[e.Index])) 
            }
        }

        private void btn_all_Click(object sender, EventArgs e)
        {
            int i = 0;
           for (i = 0; i< CheckedParameters.Items.Count; i++)
            {
                CheckedParameters.SetItemChecked(i, true);
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            int i = 0;
            for (i = 0; i < CheckedParameters.Items.Count; i++)
            {
                CheckedParameters.SetItemChecked(i, false);
            }
        }
    }
}
