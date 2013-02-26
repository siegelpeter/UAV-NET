using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Commands
{
    [Serializable]
    public abstract class EndPointCommand:BaseCommand
    {
        //gemeinsam genutzes Feld der Endpunkte
        public List<CommunicationEndpoint> knownEndpoints = new List<CommunicationEndpoint>();

    }
}
