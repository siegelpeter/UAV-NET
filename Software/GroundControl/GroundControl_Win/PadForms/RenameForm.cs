using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FlightControlCommons.PadForms
{
    public partial class RenameForm : Form
    {
      

        public RenameForm()
        {
            InitializeComponent();
        }

        public RenameForm(string p)
        {
            label1.Text = p;
        }
    }
}
