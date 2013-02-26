using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using UAVCommons;
using System.Threading;
using FlightControlCommons;
using FlightControlCommons.components;
using UAVCommons.Commands;
using log4net.Config;
using log4net;
using System.Runtime.Serialization;

namespace FlightControl
{
    /// <summary>
    /// VTOLUAV erbt seine Funktionen von dem BasisFluggerät und stellt einen Kommunikationshandler
    /// für die Befehlsverabeitung von der Basisstation zur Verfügung
    /// </summary>
    [Serializable]
    public class WayPointUAV : UAVBase
    {

        bool stabilise = true;

        //Lokale Variable 
        public string name = "Easyglider RC";
        public bool running = true;
        [NonSerialized]
        public log4net.ILog Sensorlog = LogManager.GetLogger("SensorLog");

        [NonSerialized]
        public log4net.ILog Netlog = LogManager.GetLogger("NetworkLog");

        private BaseAutoPilot pilot = null;


        /// <summary>
        /// Initialise Values and Config Settings
        /// 
        /// </summary>
        public WayPointUAV()
        {
            initialised = true;
            // Verbinde mit Hardware
            ConnectPeripherals();
            //  theta = The pitch in degrees relative to a plane(mathematical) normal to the earth radius line at the point the plane(aircraft) is at
            //    phi = The roll of the aircraft in degrees
            //     psi = The true heading of the aircraft in degrees
            // Direkt Steuern in dem Wertänderungen soffort weitergeben werden

            UAVStructure performanceData = new UAVStructure("PerformanceData", "");
            performanceData.values.Add(new UAVParameter("MaxBankLeft", 10));
            performanceData.values.Add(new UAVParameter("MaxBankRight", 10));
            performanceData.values.Add(new UAVParameter("MaxDescendAngle", 10));
            performanceData.values.Add(new UAVParameter("MaxClimbAngle", 10));
            uavData.Add(performanceData);

            BaseNavigation navi = new BaseNavigation("navi", (UAVCommons.UAVStructure)uavData["GPS"], performanceData);
            WaypointMissionControl waypointMissionControl = new WaypointMissionControl(navi, "Mission Control");
            pilot = new BaseAutoPilot("AP", navi, (UAVStructure)uavData["GPS"], (UAVStructure)uavData["AHRS"], performanceData);
            navi.PassingWayPoint += new BaseNavigation.PassingWayPointHandler(navi_PassingWayPoint);

            navi.ap = ((BaseAutoPilot)pilot);
            BaseFlightStabilizer flightstabi = new FlightControlCommons.components.BaseFlightStabilizer("Flugstabilisierung", (UAVStructure)uavData["AHRS"], pilot, uavData["Querruder"], uavData["Seitenruder"], uavData["Höhenruder"]);
            uavData.Add(waypointMissionControl);
            uavData.Add(navi);
            uavData.Add(flightstabi);
            uavData.Add(pilot);


            uavData.ValueChanged += new MonitoredDictionary<string, UAVParameter>.ValueChangedHandler(uavData_ValueChanged);
            this.DataArrived += new DataArrivedHandler(RcUAV_DataArrived);

     

        }

        /// <summary>
        /// When Passing Waypoint, report to Ground Control
        /// </summary>
        /// <param name="waypoint"></param>
        void navi_PassingWayPoint(UAVCommons.Navigation.WayPoint waypoint)
        {
            UAVCommons.Commands.WriteToFlightLog logcmd = new WriteToFlightLog("Passing WayPoint: " + waypoint.Latitude + "," + waypoint.Longitude, DateTime.Now);
            this.SendCommandAsync(logcmd);
        }


        public override void OnLoad()
        {
            base.OnLoad();

            // starte Logging 
            XmlConfigurator.Configure();
            this.Sensorlog = LogManager.GetLogger("SensorLog");
            this.Netlog = LogManager.GetLogger("NetworkLog");
        }

        public virtual void ConnectPeripherals()
        {
            if (Convert.ToBoolean(ConfigurationSettings.AppSettings["UseSimulator"]))
            {
                /// In Case ConfigValue UseSimulator is True
                ///Connects to Xplane as a Simulator
                /// Forwards all Output to the Simulator and Fetches Sensor Inputs from the Sim
                SimConnector.simConnector connector = new SimConnector.simConnector(SimConnector.SimulatorType.Xplane, ConfigurationSettings.AppSettings["SimulatorIP"]);
                connector.Start();
                uavData.Add(new SimGPS("GPS", connector));
                uavData.Add(new SimAHRS("AHRS", connector));
                uavData.Add(new SimPWM("Motor", 1000, null, 3, connector));
                uavData.Add(new SimPWM("Höhenruder", 1000, null, 2, connector));
                uavData.Add(new SimPWM("Querruder", 1000, null, 1, connector));
                uavData.Add(new SimPWM("Seitenruder", 1000, null, 4, connector));

            }
            else
            {
                /// In Case use Simulator is False Connect to the Hardware 
                Console.WriteLine("AHRS Device:" + FlightControlCommons.UsbHelper.GetDevicPathebyClass("AHRS"));
                Console.WriteLine("GPS Device:" + FlightControlCommons.UsbHelper.GetDevicPathebyClass("GPS"));

                // Verbinde mit Sensoren
                try
                {
                    uavData.Add(new GPS("GPS", UsbHelper.GetDevicPathebyClass("GPS"), 38400));
                    //  Console.WriteLine("Verbindung mit GPS Reciever erfolgreich");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Verbinden mit dem GPS");
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace.ToString());
                }

                try
                {
                    //  Console.WriteLine("Versuche AHRS unter " + USBHelper.GetDevices()[ConfigurationSettings.AppSettings["AHRSDeviceID"]] + "zu finden...");

                    uavData.Add(new AHRS("AHRS", UsbHelper.GetDevicPathebyClass("AHRS"))); //USBHelper.GetDevices()["0403:6001"] /dev/ttyUSB0
                    // Console.WriteLine("Verbindung mit AHRS Sensor erfolgreich");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("exception:" + ex.ToString());
                    Console.WriteLine("Fehler beim Verbinden mit dem AHRS");
                }

                try
                {
                    // Verbinde mit Servos 
                    uavData.Add(new PWM("Motor", 0, null, 4));
                    uavData.Add(new PWM("Höhenruder", 0, null, 2));
                    uavData.Add(new PWM("Querruder", 0, null, 1));
                    uavData.Add(new PWM("Seitenruder", 0, null, 3));
                    //    Console.WriteLine("Verbindung mit dem Mini Maestro erfolgreich");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Fehler beim Verbinden mit dem Mini Maestro");
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace.ToString());
                }


            }


            uavData.Add(new UAVStructure("Joystick", ""));
        }

        /// <summary>
        /// Motor wird nicht vom FlightStabiliser gesetzt setze bei Änderung selber
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        void Motor_ValueChanged(UAVParameter param, bool isremote)
        {
            // Console.WriteLine(param.Name);
            // dval = param.Value;

            if ((param.GetStringPath().ToLower().Contains("gps")) || (param.GetStringPath().ToLower().Contains("gps"))) return;
            // Console.WriteLine("MotorWerte:"+param.Name + ": " + param.Value);

            //   Console.WriteLine("Wert:"+param.Name+": "+param.Value.ToString());
            /* if (param.Name == "throttleout")
             {
                 uavData["Motor"].Value = param.Value;
             }
            */

            /*
             * Peter: bitte ignorieren
             * 
             * 
             */
            if (!stabilise)
            {
                try
                {
                    string ValueStr = param.Value.ToString().Replace(",", "."); //
                    Double dval = 0;
                    if (ValueStr.Contains("."))
                    {

                        string komma = ValueStr.Split('.')[1];
                        dval = Convert.ToDouble(komma) / Math.Pow(10, komma.Length);
                        //      Console.WriteLine("ValueStr: " + Convert.ToDouble(komma) + dval);

                    }
                    if (ValueStr == "1")
                    {
                        dval = 1;
                    }
                    // Console.WriteLine("preAusgabe");


                    /*
                     * Max: Setze Ausgänge
                     * 
                     */

                    if (param.Name == "theta_rollrateout") //"phi_rollrateout"
                    {
                        uavData["Höhenruder"].Value = dval * 1000 + 1000;
                        // Console.WriteLine("Ausgabe");


                    }
                    else if (param.Name == "phi_rollrateout")//"psi_rollrateout"
                    {
                        uavData["Querruder"].Value = dval * 1000 + 1000;
                        //  Console.WriteLine("Ausgabe");

                    }
                    else if (param.Name == "psi_rollrateout")//""
                    {
                        uavData["Seitenruder"].Value = dval * 1000 + 1000;
                        //  Console.WriteLine("Ausgabe");

                    }
                    else if (param.Name == "throttleout")//"theta_rollrateout"
                    {
                        uavData["Motor"].Value = dval * 1000 + 1000;
                        //    Console.WriteLine("Ausgabe");

                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace.ToString());
                }
            }

        }

        /// <summary>
        /// Neuer Sensorwert schreibe in Datei
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        void uavData_ValueChanged(UAVParameter param, bool isremote)
        {
            //  if (!isremote)
            {
                UAVCommons.Logging.ParameterLogEvent logevent = new UAVCommons.Logging.ParameterLogEvent();
                if ((param != null) && (param.Value != null))
                {
                    logevent.name = param.Name;
                    logevent.value = param.Value.ToString();
                    Sensorlog.Info(logevent);
                }

                //  Motor_ValueChanged(param, isremote);
            }
        }

        /// <summary>
        /// Daten über das Netzwerk empfangen schreibe in datei.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="arg"></param>
        void RcUAV_DataArrived(CommunicationEndpoint source, UAVParameter arg)
        {
            UAVCommons.Logging.ParameterLogEvent logevent = new UAVCommons.Logging.ParameterLogEvent();
            logevent.name = arg.Name;
            logevent.value = arg.Value.ToString();
            Netlog.Info(logevent);
        }


        /// <summary>
        /// Main Method of the UAV Control Logic
        /// </summary>
        public override void run()
        {
            initialised = true;
            uavData["Motor"].Value = 2000;
            //   uavData["phi_rollrateout"].Value = 0;
            do
            {
                //         uavData["phi_rollrateout"].Value = Convert.ToDouble(uavData["phi_rollrateout"].Value) + 1;

                //  this.SendCommand(new UAVCommons.Commands.WriteToFlightLog("Testmeldung", DateTime.Now));
                /* uavData["Höhenruder"].Value = gyrogammastrich;
                   uavData["Höhenruder"].Value = gyrogammastrich;
                   uavData["Höhenruder"].Value = gyrogammastrich;
                   uavData["Höhenruder"].Value = gyrogammastrich;
                  */
                Thread.Sleep(50);
            } while (running);

        }

    }
}
