using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UAVCommons.Commands
{
       [Serializable]
    public class SaveUAV : BaseCommand
    {
           public string filename;
       
        public SaveUAV() { }

           public SaveUAV(string filename)
           {
               this.filename = filename;
           }

           public override void RemoteExecute(UAVBase core)
        {
            base.RemoteExecute(core);
            if (filename == null) filename = "rcuav.uav";
               if (File.Exists(filename)) File.Delete(filename);
            using (FileStream stream = File.OpenWrite(filename)){
            UAVBase.SaveValues(stream,core);
            Console.WriteLine("Save Command Recieved and Done");
            }
       }
    }
}
