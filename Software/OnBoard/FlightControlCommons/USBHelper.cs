using System;
using System.Collections.Generic;
using System.Text;
using Pololu.UsbWrapper;
using Pololu.Usc;
using System.IO;
using System.Diagnostics;
using System.Configuration;
using System.Xml;
namespace FlightControlCommons
{
    public class UsbHelper
    {
        private static Dictionary<string, Dictionary<string, string>> devicecache = null;

        public static Dictionary<string, Dictionary<string, string>> GetDevices()
        {
            devicecache = new Dictionary<string, Dictionary<string, string>>();

            if (IsLinux)
            {
                //Usb usb = new Usb();
                if (devicecache.Count > 0) return devicecache;

                Process p = new Process();
                p.StartInfo.FileName = "/usr/sbin/hwinfo";
                p.StartInfo.Arguments = " --usb";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.Start();

                p.WaitForExit();
                string lines = p.StandardOutput.ReadToEnd();
                StringReader reader = new StringReader(lines);
                string line;
                bool nextdev = true;
                string deviceID = "";
                Dictionary<string, string> mydevice = new Dictionary<string, string>();

                while ((line = reader.ReadLine()) != null)
                {
                    if (nextdev)
                    {
                        deviceID = line;
                        nextdev = false;
                        mydevice = new Dictionary<string, string>();

                    }
                    int pos = line.IndexOf(":");
                    if (pos > 0)
                    {
                        if (!mydevice.ContainsKey(line.Substring(0, pos).Trim())) mydevice.Add(line.Substring(0, pos).Trim(), line.Substring(pos + 1).Trim(new char[] { ' ', '"' }));


                    }
                    if (line == String.Empty)
                    {

                        devicecache.Add(deviceID, mydevice);
                        nextdev = true;
                    }
                }
                devicecache.Add(deviceID, mydevice);

            }
            return devicecache;
        }

        /// <summary>
        /// Returns true if Running under Linux e.g.: on the UAV 
        /// </summary>
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        /// <summary>
        /// Returns the Device Path e.g. /dev/ttyUSB0 for a classid
        /// </summary>
        /// <param name="classid">The class Identifier... e.g.: AHRS or GPS</param>
        /// <returns>A string representing the Hardwaredevice e.g. /dev/ttyUSB0</returns>
        public static string GetDevicPathebyClass(string classid)
        {
            if (IsLinux)
            {
                //TODO Read XML Device Config file and return AHRS IDS
                XmlDocument mydoc = new XmlDocument();
                mydoc.Load(ConfigurationManager.AppSettings["DeviceFile"]);
                XmlNode rootelement = mydoc.FirstChild;
                foreach (XmlNode child in rootelement.ChildNodes)
                {
                    if (child.Attributes["classid"].InnerText == classid)
                    {
                        var mydevice = GetDevice(child.Attributes["serial"].InnerText);
                        if (mydevice != null) return mydevice["Device File"];
                    }
                }
            }
            else
            {
                //On Windows always return Com Port from Config file
                if (classid == "AHRS")
                {
                    return System.Configuration.ConfigurationSettings.AppSettings["AHRSDevice"];
                }
                else if (classid == "GPS")
                {
                    return System.Configuration.ConfigurationSettings.AppSettings["GPSDevice"];
                }
            }

            return null;
        }

        /// <summary>
        /// Is a Device of the Given Type Connected?
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDeviceClassConnected(string classid)
        {
            return CountDeviceConnectedByClass(classid) > 0;
        }

        /// <summary>
        /// Fetches a Device with a given Identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetDevice(string identifier)
        {
            if ((devicecache == null) || (devicecache.Count == 0)) GetDevices(); //Check and rebuild cache

            foreach (KeyValuePair<string, Dictionary<string, string>> mydevice in devicecache) // Check all Devices
            {
             if (mydevice.Value.ContainsKey("Serial ID")) // Check if it contains Property Serial and if identifier equals
                {
                    if (mydevice.Value["Serial ID"].ToLower().Trim() == identifier.ToLower().Trim())
                    {
                        return mydevice.Value; // Return found Device 
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// How many devices of the given type are connected
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int CountDeviceConnectedByClass(string classid)
        {
            //  GetDevices();
            int count = 0;
            XmlDocument mydoc = new XmlDocument();
            mydoc.Load(ConfigurationManager.AppSettings["DeviceFile"]);
               
            XmlNode rootelement = mydoc.FirstChild;
            foreach (XmlNode child in rootelement.ChildNodes)
            {
                if (child.Attributes["classid"].InnerText == classid)
                {
                    var mydevice = GetDevice(child.Attributes["serial"].InnerText);
                    if (mydevice != null) count++;
                }
            }
            return count;

        }
    }
}
