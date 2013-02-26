using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons.Logging
{
    /// <summary>
    /// One LogEntry
    /// 
    ///    Run on Start:
    ///    public log4net.ILog Netlog = LogManager.GetLogger("NetworkLog");
    ///    
    ///    Create Log Entry:
    ///    UAVCommons.Logging.ParameterLogEvent logevent = new UAVCommons.Logging.ParameterLogEvent();
    ///    logevent.name = arg.Name;
    ///    logevent.value = arg.Value.ToString();
    ///    Netlog.Info(logevent);
    /// </summary>
    public class ParameterLogEvent
    {
		private string strname = null;
		private string strvalue = null;
        /// <summary>
        /// Name of the Logged Parameter
        /// </summary>
        public string name { get{
				return strname;
				
			} set{strname = value;} }

        /// <summary>
        /// Value of the Logged Parameter
        /// </summary>
        public string value {  get{
				return strvalue;
				
			} set{strvalue = value;} }
    

    }
}
