using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroundControlUI
{
    [Serializable]
    public class LogData
    {
        public List<string> messages = new List<string>();
        public string Logname = "";

        public LogData() { 
        
        }
    }
}
