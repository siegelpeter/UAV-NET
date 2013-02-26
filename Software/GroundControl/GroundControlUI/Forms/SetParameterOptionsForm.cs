using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroundControlUI.Forms
{
    public partial class SetParameterOptionsForm : Form
    {
        private UAVCommons.UAVParameter param = null;
        public SetParameterOptionsForm(UAVCommons.UAVParameter myparam)
        {
            param = myparam;
            InitializeComponent();

            errorProvider1.Clear();
      
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            errorProvider1.Clear();
            param.Max = tb_max.Text;
            param.Min = tb_min.Text;
            try
            {
                param.updateRate = Convert.ToInt32(tb_updaterate.Text);
            }
            catch (Exception)
            {
                errorProvider1.SetError(tb_updaterate,"Please enter a positiv Int");
            }
            this.Close();
          
        }
    }
}
