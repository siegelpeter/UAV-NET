using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using FlightControlCommons.PadForms;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;
using System.Configuration;
namespace GroundControl_Win
{
    public partial class UAVOSOptionsFrm : Form
    {
        private RemoteDevice device;
        private GroundControlCore.GroundControlCore core;
        public UAVOSOptionsFrm(GroundControlCore.GroundControlCore core)
        {
            InitializeComponent();
            RemoteDevice device = new RemoteDevice(ConfigurationSettings.AppSettings["Remoteip"],ConfigurationSettings.AppSettings["RemoteUsername"],ConfigurationSettings.AppSettings["RemotePassword"]);
            this.device = device;
            this.core = core;
            UpdateSettingsList(core);
        }

        private void UpdateSettingsList(GroundControlCore.GroundControlCore core)
        {
            listBox1.Items.Clear();
            UAVCommons.Commands.FindFilesCommand findfiles = (UAVCommons.Commands.FindFilesCommand)core.currentUAV.SendCommand(new UAVCommons.Commands.FindFilesCommand("", "*.uav"));
            if (findfiles.files != null)
            {
                listBox1.Items.AddRange(findfiles.files);
            }
        }

        public void Kill(RemoteDevice target)
        {
            //hacky but openssh seems to ignore signals
            Action kill = delegate
            {
                var killExec = new SshExec(target.target.ToString(), target.username, target.password);
                killExec.Connect();
                var killcmd = @"killall mono";
                var result = killExec.RunCommand(killcmd);
                result = killExec.RunCommand("killall FCRestarter");
                killExec.Close();
            };
            kill.Invoke();
        }

        public void Start(RemoteDevice target)
        {

            //hacky but openssh seems to ignore signals
            Action start = delegate
            {
                var startexec = new SshExec(target.target.ToString(), target.username, target.password);
                startexec.Connect();
                var result = startexec.RunCommand("mono /root/FlightControl/FlightControl.exe");

                startexec.Close();
            };
            start.Invoke();
       
        }

        public static void Upload(RemoteDevice target, string sourcefile, string targetfile, bool overwrite)
        {

            var sftp = new Sftp(target.target.ToString(), target.username, target.password);


           sftp.Connect();
            if (!sftp.Connected)
            {
                //MonoDevelop.Ide.Gui.Dialogs.MultiMessageDialog dlg = new MultiMessageDialog();
                //dlg.AddMessage("Can not connect to Remote Host for File Transfer", "");
                //dlg.ShowAll();
                //return null;
            }
         
            sftp.Close();
         
        }
        
        private void btn_start_Click(object sender, EventArgs e)
        {
            Start(device);
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            Kill(device);
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked){
            Upload(device,"","",true);
            }else
            {
            Upload(device, "", "", true);
            }
            this.Close();
        }

        private void btn_LoadFrom_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                UAVCommons.Commands.LoadValuesFromFileCommand cmd = (UAVCommons.Commands.LoadValuesFromFileCommand)core.currentUAV.SendCommand(new UAVCommons.Commands.LoadValuesFromFileCommand(listBox1.SelectedItem.ToString()));
            }
        }

        private void btn_SaveAs_Click(object sender, EventArgs e)
        {
           FlightControlCommons.PadForms.RenameForm form = new RenameForm("settings.uav");
            form.Text = "Please enter a filename";
            form.ShowDialog();
            string filename = form.inputfield.Text;
            UAVCommons.Commands.SaveUAV cmd = (UAVCommons.Commands.SaveUAV)core.currentUAV.SendCommand(new UAVCommons.Commands.SaveUAV(filename));
            UpdateSettingsList(core);
        }
    }
}
