using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;
using FlightControlCommons.components;
using UAVCommons;
using System.Configuration;
using FlightControlCommons;
using UAVCommons.Navigation;
using System.Reflection;
using System.Globalization;
using System.Linq;

namespace FlightControl
{
	class Program
	{
		static Timer autoSaveTimer;
		static UAVBase uav;
		static string filename;
		static int autoSaveTime;

		static void Main (string[] args)
		{
		//	PWM test = new PWM("",0,"",0);

		//	return;
			Console.Write ("Running on ");
			if (FlightControlCommons.UsbHelper.IsLinux) {
				Console.WriteLine ("Linux");
			} else {
				Console.WriteLine ("Windows");
                    
			}


			Console.WriteLine ("Flight Control Service");
			Console.WriteLine ("Controls the UAV");
			Console.WriteLine ("");

			string autosavetimestr = ConfigurationSettings.AppSettings ["AutoSaveTime"];
			if (Int32.TryParse (autosavetimestr, out autoSaveTime)) {
				autoSaveTimer = new Timer (new TimerCallback (Program.AutoSave), null, 10000, autoSaveTime);
				Console.WriteLine ("AutoSaveTimer Set from Config File!");
			}

			filename = ConfigurationSettings.AppSettings ["UAVFilename"];
			bool loadFromFile = false;

			try {
				loadFromFile = Convert.ToBoolean (ConfigurationSettings.AppSettings ["LoadSettingsFromFile"]);
			} catch (Exception ex) {
				Console.WriteLine ("Can not parse LoadSettingsFromFile in Config File");
			}

		
			uav = new VTOLUAV ();
		
					
			
			Console.WriteLine ("Connecting to GroundControl..");
			if (Convert.ToBoolean (ConfigurationSettings.AppSettings ["WLAN"])) {
				// Baue Kommunikationskanal für Befehle in beide Richtungen auf
				TCPCommunicationEndPoint controlconnection = new TCPCommunicationEndPoint ();
				controlconnection.commType = CommunicationEndpoint.Communicationtype.Command;
				controlconnection.endpointAdress = ConfigurationSettings.AppSettings ["GroundIP"]; // IP Addresse der Gegenstelle (Groundkontroll)
				controlconnection.endpointPort = Convert.ToInt32 (ConfigurationSettings.AppSettings ["GroundCommandPort"]);
				  /// Kommunikationskanal muss auf beiden seiten gleich sein
				controlconnection.endpointSpeedType = CommunicationEndpoint.SpeedType.Wifi; // Geschwindigkeit
				controlconnection.listen = false; // einer muss verbindungen annehmen (Server) der andere hinverbinden (Connect)
				controlconnection.cmdHandler = uav;
				uav.knownEndpoints.Add (controlconnection); // Aktiviere Kommunikationskanal
			}
			if (Convert.ToBoolean (ConfigurationSettings.AppSettings ["VPN"])) {

				// Baue Kommunikationskanal für Empfang von Werten von der Bodenstation
				TCPCommunicationEndPoint recieveConnection1 = new TCPCommunicationEndPoint ();
				recieveConnection1.commType = CommunicationEndpoint.Communicationtype.Send;
				recieveConnection1.endpointAdress = ConfigurationSettings.AppSettings ["VPNGroundIP"];
				recieveConnection1.endpointPort = Convert.ToInt32 (ConfigurationSettings.AppSettings ["VPNSendPort"]);
				recieveConnection1.endpointSpeedType = CommunicationEndpoint.SpeedType.Gprs3G;
				recieveConnection1.listen = false;
				uav.knownEndpoints.Add (recieveConnection1);

				// Baue Kommunikationskanal für das Senden von Werten zur Bodenstation auf
				TCPCommunicationEndPoint sendConnection1 = new TCPCommunicationEndPoint ();
				sendConnection1.commType = CommunicationEndpoint.Communicationtype.Recieve;
				sendConnection1.endpointAdress = ConfigurationSettings.AppSettings ["VPNGroundIP"];
				sendConnection1.endpointPort = Convert.ToInt32 (ConfigurationSettings.AppSettings ["VPNRecievePort"]);
				sendConnection1.endpointSpeedType = CommunicationEndpoint.SpeedType.Gprs3G;
				sendConnection1.listen = false;
				uav.knownEndpoints.Add (sendConnection1);
			}
			if (Convert.ToBoolean (ConfigurationSettings.AppSettings ["WLAN"])) {
				TCPCommunicationEndPoint recieveConnection = new TCPCommunicationEndPoint ();
				recieveConnection.commType = CommunicationEndpoint.Communicationtype.Send;
				recieveConnection.endpointAdress = ConfigurationSettings.AppSettings ["GroundIP"];
				recieveConnection.endpointPort = Convert.ToInt32 (ConfigurationSettings.AppSettings ["SendPort"]);
				recieveConnection.endpointSpeedType = CommunicationEndpoint.SpeedType.Wifi;
				recieveConnection.listen = false;
				uav.knownEndpoints.Add (recieveConnection);

				// Baue Kommunikationskanal für das Senden von Werten zur Bodenstation auf
				TCPCommunicationEndPoint sendConnection = new TCPCommunicationEndPoint ();
				sendConnection.commType = CommunicationEndpoint.Communicationtype.Recieve;
				sendConnection.endpointAdress = ConfigurationSettings.AppSettings ["GroundIP"];
				sendConnection.endpointPort = Convert.ToInt32 (ConfigurationSettings.AppSettings ["RecievePort"]);
				sendConnection.endpointSpeedType = CommunicationEndpoint.SpeedType.Wifi;
				sendConnection.listen = false;
				uav.knownEndpoints.Add (sendConnection);
			}
	
		
				
			uav.OnLoad (); //Load all non serialised objects from uav and connect to hardware

			uav.run ();		


		}

		public static void AutoSave (object state)
		{
		  Console.WriteLine ("Autosaving UAV State to " + filename);
            if (uav != null)
            {
                Console.WriteLine("Events Per Second: Recieved " + uav.RecEventsPerSeond + " Sent " + uav.SendEventsPerSeond);
                if (uav != null)
                {
                    try
                    {
                        if (File.Exists(filename))
                            File.Delete(filename);
                        using (FileStream stream = File.OpenWrite(filename))
                        {
                            UAVBase.SaveValues(stream, uav);
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine("Cannot save UAV State: "+ex.Message);
                    }
                }
            }else{
                Console.WriteLine("Cannot save UAV");
            }

		}

	
	

	}
}
