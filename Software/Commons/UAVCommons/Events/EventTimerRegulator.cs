using System;
using System.Collections.Generic;

using System.Text;
using System.Threading;

namespace UAVCommons.Events
{
    public class EventTimerRegulator
    {
        public delegate void ForwardEventhandler(UAVParameter source, bool isremote);
        public event ForwardEventhandler forwarded;
        private Timer timer1 = null;
        Dictionary<string, UAVParameter> list = new Dictionary<string, UAVParameter>();
        bool isremote = false;
         object Syncobj = new object();

        public EventTimerRegulator(int msec) { 
             timer1 = new Timer(OutputEvent, null, 0,msec);
        }

         void OutputEvent(object test)
        {
            Dictionary<string, UAVParameter> listtemp = new Dictionary<string, UAVParameter>();
   
            lock (Syncobj)
            {
                foreach (string param in list.Keys)
                {
                   listtemp.Add(param,(UAVParameter)list[param].Clone());
                }
                 list.Clear();

            }
            foreach (UAVParameter param in listtemp.Values)
                {
                    if (forwarded != null) forwarded(param, false);
                }
               
            
        }

        public void EventRecieved(UAVParameter source, bool isremote)
        {
             lock (Syncobj)
            {
            if (list.ContainsKey(source.Name))
            {
                list[source.Name] = (UAVParameter)source.Clone();
            }
            else {
                list.Add(source.Name, (UAVParameter)source.Clone());
            }
            }

        }
    }
}
