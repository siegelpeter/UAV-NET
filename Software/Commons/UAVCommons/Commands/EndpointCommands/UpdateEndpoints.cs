using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Commands
{
     [Serializable]
    public class UpdateEndpoints : EndPointCommand
    {
         /// <summary>
         /// EndpunktImage einpacken
         /// </summary>
         /// <param name="core"></param>
         public override void Send(UAVBase core)
         {
             base.Send(core);
         }

         /// <summary>
         /// Endpunktimage anwenden
         /// </summary>
         /// <param name="core"></param>
         public override void RemoteExecute(UAVBase core)
         {
             base.RemoteExecute(core);
         }
    }
}
