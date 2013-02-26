using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace GroundControlUI.Navigation
{
    public partial class EditWayPoint : Form
    {
        private Int32 altitude = 0;
        private bool isAbsolute = true;


        public Int32 Altitude {
            get {
                return Convert.ToInt32(textBox1.Text);

            }
            set {
                altitude = value;
                textBox1.Text = Convert.ToString(value);
            }
        }

        public bool IsAbsolute {
            get {
                
                return radioButton2.Checked;
            }
            set {
                isAbsolute = value;
                this.radioButton1.Checked = !value;
                this.radioButton2.Checked = value;
        
            }
        }

        public EditWayPoint(int alt,bool IsAbsolute)
        {
           
            InitializeComponent();
            this.Altitude = alt;
            this.IsAbsolute = IsAbsolute;
           }

        /// <summary>
        /// OK Pressed Werte übernehmen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            int value = 0;
            if (Int32.TryParse(this.textBox1.Text, out value))
            {
            }
            else {
                textBox1.Text = "";
                MessageBox.Show(this,"Ungültiges Höhenformat","Mission Planer");
            }

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
            this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.Close();
       
        }
    }
}
