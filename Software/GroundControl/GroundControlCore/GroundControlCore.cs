using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Core;
using System.Configuration;
using UAVCommons;

using log4net.Layout;
using log4net.Util;
using System.IO;
namespace GroundControlCore
{
        [Serializable]
    public class GroundControlCore
    {
        public UAVCommons.UAVBase currentUAV = null;
        public ILog logger = null;
        public FlightControlCommons.UAVJoystick stick;
            
        public UAVCommons.UAVDataMapping mapping = null;
        
        public GroundControlCore(IntPtr handle) { 
    
            XmlConfigurator.Configure();
            logger = LogManager.GetLogger("");
            logger.Info(new UAVCommons.Logging.ParameterLogEvent());
            currentUAV = new UAV();
            try
            {
                stick = new FlightControlCommons.UAVJoystick("Joystickunmapped", "", handle);
                stick.ConnectHardware();

               currentUAV.uavData.Add(stick);
                
                mapping = new UAVCommons.UAVDataMapping("Joystick", "", stick);

                foreach (string key in stick.values.Keys)
                {
                    if (!mapping.Mapping.ContainsKey(key))
                    {
                        mapping.Mapping.Add(key, key);
                    }
                }
                currentUAV.uavData.Add(mapping);


            

            }
            catch (Exception ex) {
               // currentUAV.WriteToLog("GroundLog","Error on Adding Joystick");
            }
            
            
            //  currentUAV.uavData.Add("Joystick", new FlightControlCommons.UAVJoystick("Joystick",0));
          //  currentUAV.uavData.Add("Keyboard", new FlightControlCommons.UAVKeyboard("Keyboard", 0));

        }

       
    }
}
