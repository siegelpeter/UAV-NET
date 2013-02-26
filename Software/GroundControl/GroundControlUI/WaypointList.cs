using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Text;
using System.Windows.Forms;

namespace GroundControlUI
{
    public partial class WaypointList : UserControl, PersistentData
    {
        GroundControlCore.GroundControlCore core = null;
        
        public WaypointList(GroundControlCore.GroundControlCore core)
        {
            InitializeComponent();
        }


        public string PersistentData
        {
            get
            {
                return "";
            }
            set
            {

            }
        }
    }
}
