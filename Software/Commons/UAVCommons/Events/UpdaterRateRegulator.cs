using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

namespace UAVCommons.Events
{
    /// <summary>
    /// Reduces the number of events which will be Sent according to updaterate 
    /// </summary>
    public class UpdateRateRegulator
    {
        public delegate void ForwardEventhandler(UAVSingleParameter source, bool isremote);
        public event ForwardEventhandler forwarded;
        private Timer timer1 = null;

		Dictionary<string, EventValue> list = new Dictionary<string, EventValue>();
        bool isremote = false;
         object Syncobj = new object();
         DateTime lastrun = DateTime.Now;
	    Thread t1 = null;
        private bool isrunning = true;	
         public UpdateRateRegulator()
         { 
			t1 = new Thread(new ThreadStart(tick));
			t1.Start();
	     }

         void OutputEvent(object test)
        {
			lock (Syncobj){
        
         DateTime nowtime = DateTime.Now;
                foreach (KeyValuePair<string,EventValue> param in list)
                {
                    if (param.Value.Timestamp <= nowtime.AddMilliseconds(-param.Value.Value.updateRate))
                    {
						param.Value.Timestamp = nowtime;
                    }
                }
                 

          
			}
            
        }
		public void tick(){
			    
			do{
				Thread.Sleep(10);
                lastrun = DateTime.Now;
                OutputEvent(null);
            } while (isrunning);
		}
			
        public void Stop()
        {
            isrunning = false;
        }

        /// <summary>
        /// Call this method to insert a Event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="isremote"></param>
        public void EventRecieved(UAVSingleParameter source, bool isremote)
        { 
			
            if (source.updateRate == Int32.MaxValue) return; 
            if (source.updateRate == 0){
                if (forwarded != null) forwarded(source, false);
                return;
            } 
            lock (Syncobj)
            {
					
            if (list.ContainsKey(source.GetStringPath()))
            {
             //   timespans[source.Name] = DateTime.Now;
             /*   if (source.Value is double)
                {
                     list[source.Name].Value = Convert.ToDouble(source.Value.ToString());
                }else
                {
              * */
					var toupdate =list[source.GetStringPath()];
					toupdate.Value = (UAVSingleParameter)source;
					toupdate.Timestamp = DateTime.Now;
				
              //  }
                
            }
            else {
					list.Add(source.GetStringPath(), new EventValue(DateTime.Now,source));
				
              //  list.Add(source.GetStringPath(), (UAVParameter)source);//.Clone()
               // timespans.TryAdd(source.GetStringPath(), DateTime.Now);
            }
      
            }

           
        }
    }
}
