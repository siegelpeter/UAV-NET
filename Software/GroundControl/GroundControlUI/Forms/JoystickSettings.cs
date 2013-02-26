using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace GroundControlUI.Forms
{

    public partial class JoystickSettings : Form
    {
        public UAVCommons.UAVStructure joystick = null;
        UAVCommons.UAVDataMapping mapping = null;

        public JoystickSettings(UAVCommons.UAVStructure joystick, UAVCommons.UAVDataMapping mapping)
        {
            InitializeComponent();
            this.mapping = mapping;
            this.joystick = joystick;
            string[] keys = new string[mapping.Mapping.Keys.Count];
            mapping.Mapping.Keys.CopyTo(keys, 0);
            List<string> stearingFunctions = new List<string>();
            stearingFunctions.Add("phi_rollrate");
            stearingFunctions.Add("theta_rollrate");
            stearingFunctions.Add("psi_rollrate");
            stearingFunctions.Add("throttle");
            stearingFunctions.Add("");
            comboBox1.Items.AddRange(stearingFunctions.ToArray());
            comboBox2.Items.AddRange(stearingFunctions.ToArray());
            comboBox3.Items.AddRange(stearingFunctions.ToArray());
            comboBox4.Items.AddRange(stearingFunctions.ToArray());
            comboBox5.Items.AddRange(stearingFunctions.ToArray());
            comboBox6.Items.AddRange(stearingFunctions.ToArray());
            if (mapping.Mapping.ContainsKey("Axis0"))
            {
                comboBox1.SelectedItem = mapping.Mapping["Axis0"];
            }
            if (mapping.Mapping.ContainsKey("Axis1"))
            {

                comboBox2.SelectedItem = mapping.Mapping["Axis1"];
            }
            if (mapping.Mapping.ContainsKey("Axis2"))
            {
                comboBox3.SelectedItem = mapping.Mapping["Axis2"];
            }
            if (mapping.Mapping.ContainsKey("Axis3"))
            {
            
                comboBox4.SelectedItem = mapping.Mapping["Axis3"];
            }
            if (mapping.Mapping.ContainsKey("Axis4"))
            {
            
                comboBox5.SelectedItem = mapping.Mapping["Axis4"];
            }
            if (mapping.Mapping.ContainsKey("Axis5"))
            {
            
                comboBox6.SelectedItem = mapping.Mapping["Axis5"];
            }
               // mapping.ValueChanged += new UAVCommons.UAVStructure.ValueChangedHandler(mapping_ValueChanged);
            joystick.ValueChanged += new UAVCommons.UAVStructure.ValueChangedHandler(joystick_ValueChanged);
            if (((FlightControlCommons.UAVJoystick)joystick).errorOnCreate) MessageBox.Show(this, "Es ist ein Fehler beim Verbinden mit dem Joystick aufgetrehten. Bitte prüfen Sie die Verbindung und die Treiber und starten Sie das Programm neu", "GroundControl", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }

        void joystick_ValueChanged(UAVCommons.UAVSingleParameter param, bool isremote)
        {
            try
            {
                mapping_ValueChanged(param, isremote);
            }
            catch (Exception ex) { }

        }

        void mapping_ValueChanged(UAVCommons.UAVSingleParameter param, bool isremote)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(UpdateBars));
            }
            else
            {
                UpdateBars();
            }
        }

        public void UpdateBars()
        {
            foreach (string value in mapping.Source.values.Keys)
            {
                if (value == "Axis0")
                {
                    UpdateBar(joystick[value], colorProgressBar1);

                }
                if (value == "Axis1")
                {
                    UpdateBar(joystick[value], colorProgressBar2);

                }
                if (value == "Axis2")
                {
                    UpdateBar(joystick[value], colorProgressBar3);

                } if (value == "Axis3")
                {
                    UpdateBar(joystick[value], colorProgressBar4);

                } if (value == "Axis4")
                {
                    UpdateBar(joystick[value], colorProgressBar5);

                } if (value == "Axis5")
                {
                    UpdateBar(joystick[value], colorProgressBar6);

                }



            }

        }

        private void UpdateBar(UAVCommons.UAVSingleParameter value, ColorProgressBar.ColorProgressBar bar)
        {
         
            bar.Minimum = Convert.ToInt32(value.Min) * 100;
            bar.Maximum = Convert.ToInt32(value.Max) * 100;
            bar.Value = Convert.ToInt32(Convert.ToDouble(value.Value) * 100);
            bar.Refresh();
        }
        /// <summary>
        /// OK Pressed Werte übernehmen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            joystick.ValueChanged -= new UAVCommons.UAVStructure.ValueChangedHandler(joystick_ValueChanged);
          
            Dictionary<string, string> map = new Dictionary<string, string>();
            if ( Convert.ToString(comboBox1.SelectedItem) != "") map.Add("Axis0", Convert.ToString(comboBox1.SelectedItem));
            if ( Convert.ToString(comboBox2.SelectedItem) != "") map.Add("Axis1", Convert.ToString(comboBox2.SelectedItem));
            if ( Convert.ToString(comboBox3.SelectedItem) != "") map.Add("Axis2", Convert.ToString(comboBox3.SelectedItem));
            if ( Convert.ToString(comboBox4.SelectedItem) != "") map.Add("Axis3", Convert.ToString(comboBox4.SelectedItem));
            if ( Convert.ToString(comboBox5.SelectedItem) != "") map.Add("Axis4", Convert.ToString(comboBox5.SelectedItem));
            if (Convert.ToString(comboBox6.SelectedItem) != "") map.Add("Axis5", Convert.ToString(comboBox6.SelectedItem));
            mapping.Mapping = map;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        /// <summary>
        /// Abbrechen, Abbrechen, Abbrechen Arrrggg!!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            joystick.ValueChanged -= new UAVCommons.UAVStructure.ValueChangedHandler(joystick_ValueChanged);
          
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();

        }
    }
}
