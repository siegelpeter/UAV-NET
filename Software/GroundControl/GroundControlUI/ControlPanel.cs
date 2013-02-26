using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;
using FlowLayoutPanelWithDragging;

namespace GroundControlUI
{

    public partial class ControlPanel : UserControl, PersistentData
    {
        GroundControlCore.GroundControlCore core = null;
        int count = 5;

        /// <summary>
        /// Initialisiere neues OBJ
        /// </summary>
        /// <param name="core"></param>
        public ControlPanel(GroundControlCore.GroundControlCore core)
        {
            this.core = core;
            InitializeComponent();
            UpdateActiveParameters();
        }

        public string PersistentData
        {
            get
            {
                return count+","+String.Join(",",GetActiveParameters());
            }
            set
            {
                count = Convert.ToInt32(value.Split(',')[0]);
                List<string> args = new List<string>();
                args.AddRange(value.Split(new string[]{","},StringSplitOptions.RemoveEmptyEntries));
                args.RemoveAt(0);
                UpdateActiveParameters(args);

            }
        }

        /// <summary>
        /// Anzahl wurde ausgewählt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_count_TextChanged(object sender, EventArgs e)
        {
            int oldcount = count;
            int result = 0;
            if (Int32.TryParse(cb_count.Text, out result))
            {
                count = result;
            }
            else
            {
                count = oldcount;
                cb_count.Text = Convert.ToString(count);    
            }
            UpdateActiveParameters();
        }

        /// <summary>
        /// Checks all ParameterControl Panels in the Control Panel and returns their selected Parameters
        /// </summary>
        /// <returns></returns>
        private List<string> GetActiveParameters()
        {
            int newcount = 0;
            List<string> result = new List<string>();
            foreach (ParameterControl control in this.ParameterPanel.Controls)
            {
             
                if (control.selectedParameter != null)
                {
                    newcount++;
                    result.Add(control.selectedParameter.GetStringPath());
                }
            }
            if (count < newcount) count = newcount;
            return result;
        }

        private void UpdateActiveParameters()
        {
            UpdateActiveParameters(null);
        }

        private void UpdateActiveParameters(List<string> keys)
        {
            if (keys == null)
            {
                keys = GetActiveParameters();
            }
            else {
                ParameterPanel.Controls.Clear();
            }
            
            foreach (string mykey in keys) {
                bool found = false;
                foreach (ParameterControl control in this.ParameterPanel.Controls){
                    if (control.selectedParameter != null)
                    {
                        if (mykey == control.selectedParameter.GetStringPath())
                        {
                            found = true;
                        }
                    }
                }
                if (!found) { // Wenn noch nicht vorhanden 
                    ParameterControl newControl = new ParameterControl(core, core.currentUAV.uavData.GetFromPath(mykey));
                    ParameterPanel.Controls.Add(newControl);
                }

            }

            if (count > ParameterPanel.Controls.Count) { 
               int i;
               for (i = ParameterPanel.Controls.Count; i < count; i++) {
                   ParameterControl newControl = new ParameterControl(core, null);
                   ParameterPanel.Controls.Add(newControl);
               }
            }
            if (count < ParameterPanel.Controls.Count) {
                do
                {
                    try
                    {
                        ParameterPanel.Controls.RemoveAt(0);
                    }
                    catch (Exception ex) { 
                    
                    }
                } while (count < ParameterPanel.Controls.Count);
            }
 

        }

        private void btn_all_Click(object sender, EventArgs e)
        {
            List<string> keys = new List<string>();
            foreach (string key in core.currentUAV.uavData.Keys) {
                keys.Add(key);
            }
            UpdateActiveParameters(keys);
        }

     

        private void btn_Choose_Click(object sender, EventArgs e)
        {
            PropertyChooser myfrm = new PropertyChooser(core, GetActiveParameters());
            if (myfrm.ShowDialog() == DialogResult.OK)
            {
                count = myfrm.choosenKeys.Count;
                UpdateActiveParameters(myfrm.choosenKeys);
            }
    
        }

        private void ParameterPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;

        }

        private void ParameterPanel_DragDrop(object sender, DragEventArgs e)
        {
            ParameterControl data = (ParameterControl)e.Data.GetData(typeof(ParameterControl));
            FlowLayoutPanel _destination = (FlowLayoutPanel)sender;
            FlowLayoutPanel _source = (FlowLayoutPanel)data.Parent;

            if (_source != _destination)
            {
                // Add control to panel
                _destination.Controls.Add(data);
                data.Size = new Size(_destination.Width, 50);

                // Reorder
                Point p = _destination.PointToClient(new Point(e.X, e.Y));
                var item = _destination.GetChildAtPoint(p);
                int index = _destination.Controls.GetChildIndex(item, false);
                _destination.Controls.SetChildIndex(data, index);

                // Invalidate to paint!
                _destination.Invalidate();
                _source.Invalidate();
            }
            else
            {
                // Just add the control to the new panel.
                // No need to remove from the other panel, this changes the Control.Parent property.
                Point p = _destination.PointToClient(new Point(e.X, e.Y));
                var item = _destination.GetChildAtPoint(p);
                int index = _destination.Controls.GetChildIndex(item, false);
                _destination.Controls.SetChildIndex(data, index);
                _destination.Invalidate();
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            foreach (ParameterControl control in this.ParameterPanel.Controls)
            {
               
            control.UpdateValue();
            }
        }

       
    }
}
