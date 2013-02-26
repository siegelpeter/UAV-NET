using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

namespace GroundControlCore
{
    public partial class GroundMainform : Form
    {
        public GroundControlCore core;
        public GroundMainform()
        {
            InitializeComponent();
            core = new GroundControlCore(this.Handle);
           
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
