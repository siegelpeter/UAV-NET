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
    public class FindFilesCommand : BaseCommand
    {
        public string Pattern;
        public string Path;
        public string[] files;
        [NonSerialized]
        private UAVBase local;

        public FindFilesCommand()
        {


		}

        public FindFilesCommand(string path,string pattern)
        {
            this.Pattern = pattern;
            this.Path = path;
        }

        public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
            files = Directory.GetFiles(Path, Pattern);
        }
        
    }
}
