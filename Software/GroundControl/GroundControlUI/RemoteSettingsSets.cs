using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GroundControlUI
{
    public partial class RemoteSettingsSets : UserControl
    {
        public RemoteSettingsSets()
        {
            InitializeComponent();
            refreshme();
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            refreshme();
        }

        public void refreshme()
        {
            //TODO: fetch Settingssets via ssh findfiles from uav and display them
           //On Click on item send load command to uav

        }
    }
}
