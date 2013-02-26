using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
namespace UAVCommons.Commands
{
    /// <summary>
    /// The Base Command
    /// </summary>
    [Serializable]
   public abstract class BaseCommand{

        public bool completed = false;
      
        public delegate void CommandCompletedHandler();
        /// <summary>
        /// Event is fired when Command has returned from UAV / Ground
        /// </summary>
    
        public event CommandCompletedHandler CommandCompleted;

        public BaseCommand(string CommandName)
        {
            this.CommandName = CommandName;
            this.guid = Guid.NewGuid().ToString();
        }

        public BaseCommand() { 
        
        }
       
       public string CommandName =null;
       public DateTime SendDate = DateTime.MinValue;
       public bool isAsync = false;
       public string guid = null;
       public virtual void Send(UAVBase core){
           /// Wird vom Kommandogeber (sender) aufgerufen
           /// 
           /// 

           SendDate = DateTime.UtcNow;
           guid = Guid.NewGuid().ToString();
       }
        

       public virtual void RemoteExecute(UAVBase core) // Wird automatisch vom Kommandoempfänger aufgerufen
       {

           SendDate = DateTime.UtcNow;
       
       }

       public virtual void ProcessResults(UAVBase core) {
           if (CommandCompleted != null) CommandCompleted();
       
       }
   }
}
