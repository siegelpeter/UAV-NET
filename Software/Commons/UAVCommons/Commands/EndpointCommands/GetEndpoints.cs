using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Commands
{
     [Serializable]
    public class GetEndpoints:EndPointCommand
    {
         /// <summary>
         /// Endpunkte Abbild einpacken
         /// </summary>
         /// <param name="core"></param>
         public override void RemoteExecute(UAVBase core)
         {
             base.RemoteExecute(core);
             knownEndpoints = core.knownEndpoints;
         }

         /// <summary>
         /// Endpunkt Abbild einpacken
         /// </summary>
         /// <param name="core"></param>
         public override void ProcessResults(UAVBase core)
         {
             base.ProcessResults(core);
         }
    }
}
