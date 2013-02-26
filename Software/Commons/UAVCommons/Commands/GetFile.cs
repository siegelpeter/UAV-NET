using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace UAVCommons.Commands
{
    /// <summary>
    /// Starts the remote encoding Process and the Local player and puts it in foreground
    /// </summary>
    [Serializable]
    public class LoadValuesCommand : BaseCommand
    {
        public string Filename;
        
        [NonSerialized]
        private UAVBase local;

        public LoadValuesCommand()
        {


		}

        public LoadValuesCommand(string filename)
        {
            this.Filename = filename;
        }

        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
            using (FileStream stream = File.OpenRead(Filename))
            {
                UAVBase.LoadValues(stream, core);
            }
        }
        
    }
}
