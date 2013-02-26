using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UAVCommons.Events
{
    public class MethodTimerRegulator
    {
        public delegate void ForwardEventhandler()
        
        private Timer timer1 = null;
        CommunicationEndpoint source = null;
        UAVParameter arg = null;

        public MethodTimerRegulator(int msec)
        { 
             timer1 = new Timer(OutputEvent, null, 0,msec);
        }

         void OutputEvent(object test)
        {
            if (forwarded != null) forwarded(source, arg);
        }

        public void EventRecieved(CommunicationEndpoint source, UAVParameter arg)
        {
            this.source = source;
            this.arg = arg;

        }
    }
}
