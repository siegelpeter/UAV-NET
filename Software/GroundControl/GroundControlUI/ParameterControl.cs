using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using UAVCommons;
using System.Text;
using System.Windows.Forms;

namespace GroundControlUI
{

    public partial class ParameterControl : FlowLayoutPanelWithDragging.DraggableUserControl
    {
        public bool output = false;
        GroundControlCore.GroundControlCore core = null;
        public UAVCommons.UAVSingleParameter selectedParameter = null;

        public ParameterControl(GroundControlCore.GroundControlCore core, UAVCommons.UAVSingleParameter parameter)
            : base()
        {
            
            InitializeComponent();
            this.core = core;
            core.currentUAV.uavData.NewValue += new UAVCommons.MonitoredDictionary<string, UAVCommons.UAVSingleParameter>.NewValueHandler(uavData_NewValue);
            if (parameter != null) selectedParameter = parameter;

           
            Update_Combobox();
            if (parameter != null)
            {
                cb_Parameter.SelectedItem = parameter.GetStringPath();
                    }
            btn_output.ForeColor = Color.Blue;
            btn_output.Text = "Input";
            output = false;
            
            if (parameter != null)
            {
                ValueBar.Step = 1;
                valueTracker.TickFrequency = 10;
                valueTracker.TickStyle = TickStyle.Both;
                valueTracker.Maximum = Convert.ToDecimal(parameter.Max);
                valueTracker.Minimum = Convert.ToDecimal(parameter.Min);
                ValueBar.Minimum = valueTracker.Minimum;
                ValueBar.Maximum = valueTracker.Maximum;


                if (parameter.Max != null) tb_max.Text = parameter.Max.ToString();
                if (parameter.Min != null) tb_min.Text = parameter.Min.ToString();
                if ((parameter.Value != null) && (parameter.Value.ToString() != ""))
                {
                    if ((parameter.DoubleValue >= parameter.MinDoubleValue) &&
                        (parameter.DoubleValue <= parameter.MaxDoubleValue))
                    {
                    
                    valueTracker.Value = Convert.ToDecimal(parameter.Value);
                    ValueBar.Value = Convert.ToDecimal(valueTracker.Value);
                    lbl_value.Text = parameter.Value.ToString();
                }
                else
                {
                    lbl_value.Text = "Value out of Bounds " + parameter.Value.ToString();
                }
            }

        }
            else {
                ValueBar.Minimum = 0;
                valueTracker.Minimum = 0;
                ValueBar.Maximum = 100;
                valueTracker.Maximum = 100;
                ValueBar.Value = 50;
                valueTracker.Value = 50;
              
                lbl_value.Text = "50";
            }
            this.valueTracker.Factor = 0.0001m;

        }

        public void Update_Combobox() {

            this.cb_Parameter.Items.Clear();

            List<UAVSingleParameter> uavData = MonitoredDictionary<string, UAVSingleParameter>.NormaliseDictionary(core.currentUAV.uavData);

            foreach (UAVSingleParameter param in uavData)
            {
                this.cb_Parameter.Items.Add(param.Name);
            }
          

        }

        void uavData_NewValue(UAVCommons.UAVSingleParameter param, bool isremote)
        {
            if (cb_Parameter.InvokeRequired) {
                cb_Parameter.Invoke(new MethodInvoker(Update_Combobox));
            }
         
            
           // param.ValueChanged += new UAVCommons.UAVParameter.ValueChangedHandler(parameter_ValueChanged);
            
         
        }



        private void UpdateValue(UAVCommons.UAVSingleParameter param)
        {
            if (param != null)
            {

                double min = 0;
                double max = 0;
                UAVSingleParameter p = selectedParameter;
                // Check if is Servo
                if ((selectedParameter is UAVStructure) && (((UAVStructure)selectedParameter).values.ContainsKey("Value")))
                {
                    p = ((UAVStructure)selectedParameter)["Value"];
                }
                
                    if ((p is UAVStructure) && (((UAVStructure)p)["Min"] != null))
                    {
                        min = Convert.ToDouble(((UAVStructure)p)["Min"].Value.ToString());

                    }
                    else
                    {
                        min = Convert.ToDouble(p.Min.ToString());
                    }


                    if ((p is UAVStructure) && (((UAVStructure)p)["Max"] != null))
                    {
                        max = Convert.ToDouble(((UAVStructure)p)["Max"].Value);

                    }
                    else
                    {
                        max = Convert.ToDouble(p.Max.ToString());
                    }

                


                double result = 0;
                if (Double.TryParse(Convert.ToString(param.Value), out result))
                {
                    if (output)
                    {
                        if (ValueBar.InvokeRequired)
                        {
                            ValueBar.Invoke((MethodInvoker)delegate
                            {
                                ValueBar.Minimum = Convert.ToDecimal(min);
                                ValueBar.Maximum = Convert.ToDecimal(max);
                                if ((Convert.ToDouble(param.Value) > min) && (Convert.ToDouble(param.Value) < max))
                                {
                                    ValueBar.Value = Convert.ToDecimal(Convert.ToDouble(param.Value));
                                }
                                ValueBar.Refresh();

                            });
                        }
                        else
                        {
                            ValueBar.Minimum = Convert.ToDecimal(min);
                            ValueBar.Maximum = Convert.ToDecimal(max);
                            if ((Convert.ToDouble(param.Value) > min) && (Convert.ToDouble(param.Value) < max))
                            {
                                ValueBar.Value = Convert.ToDecimal(Convert.ToDouble(param.Value));
                            }

                            ValueBar.Invalidate();


                        }
                    }
                    else
                    {
                        if (valueTracker.InvokeRequired)
                        {

                            valueTracker.Invoke((MethodInvoker)delegate
                            {

                                valueTracker.Maximum = Convert.ToDecimal(max);
                                valueTracker.Minimum = Convert.ToDecimal(min);
                                if ((Convert.ToDouble(param.Value) > min) && (Convert.ToDouble(param.Value) < max))
                                {
                                    valueTracker.Value = Convert.ToDecimal(Convert.ToDouble(param.Value));
                                }
                                valueTracker.TickFrequency = 10;
                                valueTracker.TickStyle = TickStyle.Both;


                            });



                            valueTracker.Invalidate();

                        }
                        else
                        {

                            // valueTracker.Value = Convert.ToDecimal(param.Value);
                            valueTracker.TickFrequency = 10;
                            valueTracker.TickStyle = TickStyle.Both;
                            valueTracker.Maximum = Convert.ToDecimal(max);
                            valueTracker.Minimum = Convert.ToDecimal(min);
                            if ((param.DoubleValue > min) && (param.DoubleValue < max))
                            {
                                valueTracker.Value = Convert.ToDecimal(param.Value);
                            }
                            valueTracker.Invalidate();
                        }
                    }
                    if (lbl_value.InvokeRequired)
                    {
                        //    lbl_value.Invoke((MethodInvoker)(() => lbl_value.Text = param.Value.ToString()));
                        tb_max.Invoke((MethodInvoker)(() => tb_max.Text = max.ToString()));
                        tb_min.Invoke((MethodInvoker)(() => tb_min.Text = min.ToString()));
                        //  lbl_value.Invalidate();
                    }
                    else
                    {

                        tb_max.Text = max.ToString();
                        tb_min.Text = min.ToString();

                        lbl_value.Text = param.Value.ToString();
                        lbl_value.Invalidate();
                    }
                }
            }

        }
    
        /// <summary>
        /// Ändere den Modus
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (output)
            {
                btn_output.ForeColor = Color.Blue;
                btn_output.Text = "Input";
                output = false;
                this.ValueBar.Visible = false;
                valueTracker.Visible = true;
                
            }
            else {
                btn_output.ForeColor = Color.Red;
                output = true;
                btn_output.Text = "Output";
                this.ValueBar.Visible = true;
                this.valueTracker.Visible = false;
                     
            }
            
        }

        /// <summary>
        /// Anderer Parameter wurde ausgewählt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_Parameter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_Parameter.SelectedIndex != -1) {

                try
                {
                    selectedParameter = core.currentUAV.uavData.GetFromPath(Convert.ToString(cb_Parameter.SelectedItem));
                    if (selectedParameter == null) throw new Exception("Parameter not found in the given Path");
                    this.valueTracker.TickFrequency = 100;
                    this.valueTracker.TickStyle = TickStyle.Both;
                    valueTracker.Minimum = Convert.ToDecimal(selectedParameter.Min);
                    valueTracker.Maximum = Convert.ToDecimal(selectedParameter.Max);
                    this.ValueBar.Minimum = Convert.ToDecimal(valueTracker.Minimum);
                    this.ValueBar.Maximum = Convert.ToDecimal(valueTracker.Maximum);
                    this.tb_max.Text = Convert.ToString(selectedParameter.Max);
                    this.tb_min.Text = Convert.ToString(selectedParameter.Min);
                    this.lbl_value.Text = selectedParameter.Value.ToString();
                    if ((selectedParameter.DoubleValue >= selectedParameter.MinDoubleValue) &&
                       (selectedParameter.DoubleValue <= selectedParameter.MaxDoubleValue))
                    {
                        ValueBar.Value = Convert.ToDecimal(selectedParameter.DoubleValue);
                        valueTracker.Value = Convert.ToDecimal(selectedParameter.DoubleValue);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show("Cannot select Value for Parameter Control: " + ex.Message);
                    
                }
            }
        }

        /// <summary>
        /// Speichere geänderten Wert sobald sich die Trackbar bewegt
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void valueTracker_Scroll(object sender, EventArgs e)
        {
            try
            {
                UAVSingleParameter p = selectedParameter;
                if ((selectedParameter is UAVStructure) && (((UAVStructure)selectedParameter).values.ContainsKey("Value")))
                {
                    p = ((UAVStructure)selectedParameter)["Value"];
                }




                p.Value = valueTracker.Value;
                ValueBar.Value = valueTracker.Value; 
                this.lbl_value.Text = Convert.ToString(valueTracker.Value);
            }
            catch (Exception ex) { 
            
            }
        }

        /// <summary>
        /// Setze Min Value und schicke Sie per Command an UAV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_min_TextChanged(object sender, EventArgs e)
        {
           /* int result = 0;
            if (Int32.TryParse(tb_min.Text, out result))
            {

                if (selectedParameter != null)
                {
                    if (Convert.ToInt32(selectedParameter.Min) != result)
                    {
                        selectedParameter.Min = result;
                        UAVCommons.Commands.SetParameterOptions options = new UAVCommons.Commands.SetParameterOptions((UAVCommons.UAVParameter)selectedParameter.Clone());
                        core.currentUAV.SendCommand(options);
                    }
                }
            }*/
        }

        /// <summary>
        /// Setze Max Value und schicke Sie per Command an UAV
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tb_max_TextChanged(object sender, EventArgs e)
        {
            /* int result = 0;
             if (Int32.TryParse(tb_max.Text,out result)){
             if (selectedParameter != null)
             {
                 if (Convert.ToInt32(selectedParameter.Max) != result)
                 {
                     selectedParameter.Max = result;
                     UAVCommons.Commands.SetParameterOptions options = new UAVCommons.Commands.SetParameterOptions((UAVCommons.UAVParameter)selectedParameter.Clone());
                     core.currentUAV.SendCommand(options);
                 }
             }
           
         } * */
        }

        private void cb_Parameter_DropDown(object sender, EventArgs e)
        {
            Update_Combobox();
        }




        internal void UpdateValue()
        {
            UpdateValue(selectedParameter);
        }
    }
}
