using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace UAVCommons.Commands
{
    /// <summary>
    /// Kills a Remote Process
    /// </summary>
    [Serializable]
    public class KillProcess : BaseCommand
    {
        public int pid = 0;

        
        /// <summary>
        /// Constructor sets Pid of Process to kill on uav
        /// </summary>
        /// <param name="pid"></param>
        public KillProcess(int pid)
        {
            this.pid = pid;
        }
		public KillProcess(){


		}

        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
            Process p = Process.Start("/bin/kill ");
            p.StartInfo.Arguments = Convert.ToString(pid);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            pid = p.Id;

        }

        public override void ProcessResults(UAVBase core)
        {
            base.ProcessResults(core);
        }
    }
}
