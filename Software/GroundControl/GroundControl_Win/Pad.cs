using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FlightControlCommons.PadForms;
using WeifenLuo.WinFormsUI.Docking;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace GroundControl_Win
{
    public partial class Pad : DockContent
    {
        public UserControl basecontrol = null;
        public Pad(UserControl mycontrol,string title,WeifenLuo.WinFormsUI.Docking.DockState target)
        {
            basecontrol = mycontrol;
            this.ShowHint = target;
            this.TabText = title;
            this.Text = title;
            basecontrol.Dock = DockStyle.Fill;
            this.Controls.Add(basecontrol);
           
            
        }

        void Pad_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
         
            RenameForm frm = new RenameForm("New Name for Pad: " + this.Text);
            frm.ShowDialog();
            TabText = frm.inputfield.Text;
            Text = TabText;
        }
    }

       

        protected override string GetPersistString()
        {
            return basecontrol.GetType().Assembly.FullName+";"+basecontrol.GetType().ToString()+";"+this.Text+";"+((GroundControlUI.PersistentData)basecontrol).PersistentData;

        }
    }
}