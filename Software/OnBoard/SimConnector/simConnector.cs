/*
    UAV.NET: a UAV / Robotic control framework for .net
    Copyright (C) 2012  Peter Siegel

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO.Ports;
using System.IO;
using System.Xml; // config file
using System.Runtime.InteropServices; // structs
using UAVCommons;
namespace SimConnector
{
    public enum SimulatorType { Xplane, Flightgear }
          [Serializable]
    public class simConnector
    {
        UAVCommons.UAVBase core = null;
        [NonSerialized]
        UdpClient XplanesSEND;
        [NonSerialized]
        Socket SimulatorRECV;
        [NonSerialized]
        TcpClient FlightGearSEND;
        byte[] udpdata = new byte[113 * 9 + 5]; // 113 types - 9 items per type (index+8) + 5 byte header
        float[][] DATA = new float[113][];

        object SimValuesSync = new object();
        [NonSerialized]
        System.Threading.Timer sender = null;

        DateTime now = DateTime.Now;
        DateTime lastgpsupdate = DateTime.Now;
        List<string> position = new List<string>();
        int AAAlength = 17 + 3 + 2; // 3 header + 17 data + 2 cr and lf 
        int REV_pitch = 1;
        int REV_roll = 1;
        int REV_rudder = 1;
        int GPS_rate = 200;
        bool displayfull = false;
        int packetssent = 0;
        string logdata = "";
        string[] modes = { "Manual", "Circle", "Stabalize", "", "", "FBW A", "FBW B", "", "", "", "Auto", "RTL", "Loiter", "Takeoff", "Land" };
        int tickStart = 0;
        static int threadrun = 1;
        string simIP = "127.0.0.1";
        int simPort = 49000;

        public bool RAD_softXplanes = false;

        Dictionary<string, object> SimValues = new Dictionary<string, object>();

        public delegate void NewValuesHandler(object sender, Dictionary<string, object> values);
        public event NewValuesHandler NewValues;

        public delegate void ValueChangedHandler(UAVParameter param, bool isremote);
        public event ValueChangedHandler ValueChanged;



        public simConnector(SimulatorType mytype, string IP)
        {
            if (mytype == SimulatorType.Xplane)
            {
                RAD_softXplanes = true;
            }
            simIP = IP;
            SimValues.Add("roll_out", 0);
            SimValues.Add("pitch_out", 0);
            SimValues.Add("throttle_out", 0);
            SimValues.Add("rudder_out", 0);
        }

        ~simConnector()
        {
            if (threadrun == 1)
                stop();
            XplanesSEND = null;
            SimulatorRECV = null;
        }

        public void stop()
        {
            threadrun = 0;
            System.Threading.Thread.Sleep(200); // make sure thread closes
            if (SimulatorRECV != null)
                SimulatorRECV.Close();
            if (SimulatorRECV.Connected)
                SimulatorRECV.Disconnect(true);
            position.Clear();

            if (XplanesSEND != null)
                XplanesSEND.Close();

            System.Threading.Thread.Sleep(100);
            if (FlightGearSEND != null && FlightGearSEND.Connected)
                FlightGearSEND.Close();


        }
        public void Start()
        {
            try
            {
                SetupUDPRecv();

                if (RAD_softXplanes)
                {
                    SetupUDPXplanes();
                }
                else
                {
                    SetupTcpFlightGear();
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Socket setup problem. Do you have this open already? " + ex.ToString()); }

            System.Threading.Thread t11 = new System.Threading.Thread(delegate() { mainloop(); });
            t11.Name = "Main Serial/UDP listener";
            t11.Start();


        }

        /// <summary>
        /// Bincomm location struct
        /// </summary>
        [StructLayout(LayoutKind.Explicit, Size = 20)]
        public struct msg_location
        {
            [FieldOffset(0)]
            public int latitude;
            [FieldOffset(4)]
            public int longitude;
            [FieldOffset(8)]
            public int altitude;
            [FieldOffset(12)]
            public UInt16 groundSpeed;
            [FieldOffset(14)]
            public UInt16 groundCourse;
            [FieldOffset(16)]
            public uint timeOfWeek;

            //[FieldOffset(0)]
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            //public byte[] bytes;
        };

        /// <summary>
        /// Reads from serial and UDP and process's
        /// </summary>
        private void mainloop()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            threadrun = 1;
            EndPoint Remote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

            byte mode = 0;
            Int16 command = 0;
            msg_location loc = new msg_location();

            while (threadrun == 1)
            {
                if (SimulatorRECV.Available > 0)
                {
                    udpdata = new byte[udpdata.Length];

                    int recv = SimulatorRECV.ReceiveFrom(udpdata, ref Remote);

                    try
                    {
                        RECVprocess(udpdata, recv);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Xplanes Data Problem - You need DATA IN/OUT 3, 18, 19, 20\n" + ex.Message + "\n");
                    }
                }

                System.Threading.Thread.Sleep(5); // try minimise delays                    
            }

        }

        private void SetupUDPRecv()
        {
            // setup receiver
            IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 49005);

            SimulatorRECV = new Socket(AddressFamily.InterNetwork,
                            SocketType.Dgram, ProtocolType.Udp);

            SimulatorRECV.Bind(ipep);
        }

        private void SetupUDPXplanes()
        {
            // setup sender
            XplanesSEND = new UdpClient(simIP, simPort);

            sender = new System.Threading.Timer(new System.Threading.TimerCallback(processPilot), null, 0, 10);

        }

        private void SetupTcpFlightGear()
        {
            FlightGearSEND = new TcpClient(simIP, simPort);
        }




        /// <summary>
        /// Recevied UDP packet, process and send required data to serial port.
        /// </summary>
        /// <param name="data">Packet</param>
        /// <param name="receviedbytes">Length</param>
        /// <param name="comPort">Com Port</param>
        private void RECVprocess(byte[] data, int receviedbytes)
        {
            if (data[0] == 'D' && data[1] == 'A')
            {
                // Xplanes sends
                // 5 byte header
                // 1 int for the index - numbers on left of output
                // 8 floats - might be useful. or 0 if not
                int count = 5;
                while (count < receviedbytes)
                {
                    int index = BitConverter.ToInt32(data, count);

                    DATA[index] = new float[8];

                    DATA[index][0] = BitConverter.ToSingle(data, count + 1 * 4); ;
                    DATA[index][1] = BitConverter.ToSingle(data, count + 2 * 4); ;
                    DATA[index][2] = BitConverter.ToSingle(data, count + 3 * 4); ;
                    DATA[index][3] = BitConverter.ToSingle(data, count + 4 * 4); ;
                    DATA[index][4] = BitConverter.ToSingle(data, count + 5 * 4); ;
                    DATA[index][5] = BitConverter.ToSingle(data, count + 6 * 4); ;
                    DATA[index][6] = BitConverter.ToSingle(data, count + 7 * 4); ;
                    DATA[index][7] = BitConverter.ToSingle(data, count + 8 * 4); ;

                    count += 36; // 8 * float
                }
            }
            else
            {
                //FlightGear

                DATA[20] = new float[8];

                DATA[18] = new float[8];

                DATA[19] = new float[8];

                DATA[3] = new float[8];

                // this text line is defined from ardupilot.xml
                string telem = Encoding.ASCII.GetString(data, 0, data.Length);

                try
                {
                    // should convert this to regex.... or just leave it.
                    int oldpos = 0;
                    int pos = telem.IndexOf(",");
                    DATA[20][0] = float.Parse(telem.Substring(oldpos, pos - 1));

                    oldpos = pos;
                    pos = telem.IndexOf(",", pos + 1);
                    DATA[20][1] = float.Parse(telem.Substring(oldpos + 1, pos - 1 - oldpos));

                    oldpos = pos;
                    pos = telem.IndexOf(",", pos + 1);
                    DATA[20][2] = float.Parse(telem.Substring(oldpos + 1, pos - 1 - oldpos));

                    oldpos = pos;
                    pos = telem.IndexOf(",", pos + 1);
                    DATA[18][1] = float.Parse(telem.Substring(oldpos + 1, pos - 1 - oldpos));

                    oldpos = pos;
                    pos = telem.IndexOf(",", pos + 1);
                    DATA[18][0] = float.Parse(telem.Substring(oldpos + 1, pos - 1 - oldpos));

                    oldpos = pos;
                    pos = telem.IndexOf(",", pos + 1);
                    DATA[19][2] = float.Parse(telem.Substring(oldpos + 1, pos - 1 - oldpos));

                    oldpos = pos;
                    pos = telem.IndexOf("\n", pos + 1);
                    DATA[3][6] = float.Parse(telem.Substring(oldpos + 1, pos - 1 - oldpos));
                    DATA[3][7] = DATA[3][6];
                }
                catch (Exception) { }
            }


            //convert data	
            float lat = (float)(DATA[20][0]);
            float lng = (float)(DATA[20][1]);
            float altitude = (float)(DATA[20][2]);		// altitude to meters
            float speed = (int)(DATA[3][7]);		// spped to m/s * 100
            float airspeed = (int)(DATA[3][6]);		// speed to m/s * 100
            float pitch = (float)(DATA[18][0]);
            float roll = (float)(DATA[18][1]);
            float heading = (float)(DATA[19][2]); // heading (ground course) as pulled from gps is the path bearing not the plane nose bearing
            try
            {
                Double accx = (Double)(DATA[15][0]);  // in ft / lbs
                Double accy = (Double)(DATA[15][1]);
                Double accz = (Double)(DATA[15][2]);

                Double gyrogammastrich = (Double)(DATA[17][0]);  // in rad /sec
                Double gyrbetastrich = (Double)(DATA[17][1]);
                Double gyralphastrich = (Double)(DATA[17][2]);
            }
            catch (Exception ex)
            {

            }



            float yaw = (int)(DATA[18][2] * 100); // plane nose bearing



            TimeSpan gpsspan = DateTime.Now - lastgpsupdate;

            if (gpsspan.TotalMilliseconds >= GPS_rate)
            {
                lastgpsupdate = DateTime.Now;




                // Update Events
                UpdateSimValue("lng", lng);
                UpdateSimValue("lat", lat);
                UpdateSimValue("altitude", altitude);
                UpdateSimValue("speed", speed);
                UpdateSimValue("airspeed", airspeed);
                UpdateSimValue("pitch", pitch);
                UpdateSimValue("roll", roll);
                UpdateSimValue("heading", heading);
                UpdateSimValue("yaw", yaw);

                UpdateSimValue("accx", yaw);
                UpdateSimValue("accx", yaw);
                UpdateSimValue("accx", yaw);

                UpdateSimValue("gyrogammastrich", yaw);
                UpdateSimValue("gyrbetastrich", yaw);
                UpdateSimValue("gyralphastrich", yaw);

                lock (SimValuesSync)
                {

                    if (NewValues != null)
                    {
                        NewValues(this, SimValues);
                    }
                }
            }
        }

        public void UpdateSimValue(string p, object p_2)
        {
            lock (SimValuesSync)
            {

                if (SimValues.ContainsKey(p))
                {
                    SimValues[p] = p_2;
                }
                else
                {
                    SimValues.Add(p, p_2);
                }
            }
        }



        /// <summary>
        /// Process's recevied AAA string from the serial port.
        /// </summary>
        /// <param name="line">AAA byte array</param>
        private void processPilot(object state)
        {
            lock (SimValuesSync)
            {



                //Convert data to scaled usable data
                float roll_out = Convert.ToSingle(SimValues["roll_out"]);
                float pitch_out = Convert.ToSingle(SimValues["pitch_out"]);
                float throttle_out = Convert.ToSingle(SimValues["throttle_out"]);
                float rudder_out = Convert.ToSingle(SimValues["rudder_out"]);

                // Limit min and max
                roll_out = Constrain(roll_out, -1, 1);
                pitch_out = Constrain(pitch_out, -1, 1);
                rudder_out = Constrain(rudder_out, -1, 1);
                throttle_out = Constrain(throttle_out, 0, 1);
                //  roll_out = 0.8f;
                //               throttle_out = 0;
                packetssent++;

                if (!RAD_softXplanes)
                {
                    if (packetssent % 10 == 0) { return; } // short supply buffer.. seems to reduce lag

                    string send = "3," + (roll_out * REV_roll) + "," + (pitch_out * REV_pitch * -1) + "," + (rudder_out * REV_rudder) + "," + (throttle_out) + "\r\n";

                    byte[] FlightGear = new System.Text.ASCIIEncoding().GetBytes(send);

                    try
                    {
                        FlightGearSEND.GetStream().Write(FlightGear, 0, FlightGear.Length);
                        //XplanesSEND.Send(FlightGear, FlightGear.Length);  // if FG udp worked......
                    }
                    catch (Exception) { Console.WriteLine("Socket Write failed, FG closed?"); RAD_softXplanes = true; }

                }

                // Xplanes

                if (RAD_softXplanes)
                {
                    // sending only 1 packet instead of many.

                    byte[] Xplane = new byte[5 + 36 + 36];

                    Xplane[0] = (byte)'D';
                    Xplane[1] = (byte)'A';
                    Xplane[2] = (byte)'T';
                    Xplane[3] = (byte)'A';
                    Xplane[4] = 0;

                    Array.Copy(BitConverter.GetBytes((int)25), 0, Xplane, 5, 4); // packet index

                    Array.Copy(BitConverter.GetBytes((float)throttle_out), 0, Xplane, 9, 4); // start data
                    Array.Copy(BitConverter.GetBytes((float)throttle_out), 0, Xplane, 13, 4);
                    Array.Copy(BitConverter.GetBytes((float)throttle_out), 0, Xplane, 17, 4);
                    Array.Copy(BitConverter.GetBytes((float)throttle_out), 0, Xplane, 21, 4);

                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 25, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 29, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 33, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 37, 4);

                    // NEXT ONE - control surfaces

                    Array.Copy(BitConverter.GetBytes((int)11), 0, Xplane, 41, 4); // packet index

                    Array.Copy(BitConverter.GetBytes((float)(pitch_out * REV_pitch)), 0, Xplane, 45, 4); // start data
                    Array.Copy(BitConverter.GetBytes((float)(roll_out * REV_roll)), 0, Xplane, 49, 4);
                    Array.Copy(BitConverter.GetBytes((float)(rudder_out * REV_rudder)), 0, Xplane, 53, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 57, 4);

                    Array.Copy(BitConverter.GetBytes((float)roll_out * REV_roll * 5), 0, Xplane, 61, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 65, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 69, 4);
                    Array.Copy(BitConverter.GetBytes((int)-999), 0, Xplane, 73, 4);

                    try
                    {

                        XplanesSEND.Send(Xplane, Xplane.Length);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error on senddata");
                    }
                }
            }
        }

        /// <summary>
        /// Todo keep in boundaries
        /// </summary>
        /// <param name="roll_out"></param>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        /// <returns></returns>
        private float Constrain(float roll_out, int p, int p_2)
        {
            //  return roll_out;
            if (roll_out == 0) return 0;
            return (roll_out - 1500) / 500;
        }


    }
}
