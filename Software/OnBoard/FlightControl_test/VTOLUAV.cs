using System;
using System.Collections.Generic;
using System.Text;
using UAVCommons;
using System.Threading;
using FlightControlCommons;
using FlightControlCommons.components;
using UAVCommons.Commands;

using PIDLibrary;


	/// <summary>
	/// VTOLUAV erbt seine Funktionen von dem BasisFluggerät
	/// Max seine Spielewiese
	/// </summary>
	[Serializable]
	public class VTOLUAV : UAVBase
	{  
		PWM servo1;
        UAVStructure joystick = new UAVStructure("Joystick", null);
        UAVParameter theta_rolerate = new UAVParameter("theta_rolerate", 0, -100, 100);
        UAVParameter psi_rolerate = new UAVParameter("psi_rolerate", 0, -100, 100);
        UAVParameter phi_rolerate = new UAVParameter("phi_rolerate", 0, -100, 100);
        UAVParameter throttle = new UAVParameter("throttle", 0, -100, 100);

        PWM servo2;
        PWM servo3;
        PWM servo4;
        PWM servo5;
		
        //PID PID_Höhe;
        //PID PID_Quer;
        //PID PID_Seite;

        UAVParameter kp_Höhe;
        UAVParameter kd_Höhe;
        UAVParameter ki_Höhe;

        UAVParameter kp_Quer;
        UAVParameter kd_Quer;
        UAVParameter ki_Quer;

        UAVParameter kp_Seite;
        UAVParameter kd_Seite;
        UAVParameter ki_Seite;

        UAVParameter sp_Höhe;
        UAVParameter sp_Quer;
        UAVParameter sp_Seite;

        UAVParameter PID_Out_Höhe;
        UAVParameter PID_Out_Quer;
        UAVParameter PID_Out_Seite;

        UAVParameter Factor;

	//	UAVJoystick empfängerusb; // Usb Stick empfänger onboard
		AHRS lagesensor;
	//	GPS gpsempfänger;
			
	    DateTime timestamp;
	    int counter = 0;
		bool running = true;
						
		public VTOLUAV ()	//  Constructor -> Is Called when the Object is created
		{                   // When the Object is loaded from the sd card this method will not be called
            joystick.values.Add(phi_rolerate);
            joystick.values.Add(theta_rolerate);
            joystick.values.Add(psi_rolerate);
            joystick.values.Add(throttle);
            uavData.Add(joystick);
            //Factor = new UAVParameter ("Factory", 10, 0, 1000, 0); // Factor for Mixers
            //uavData.Add (Factor);
            lagesensor = new AHRS ("lagesensor", UsbHelper.GetDevicPathebyClass ("AHRS"));
            uavData.Add (lagesensor);
            ////gpsempfänger = new GPS ("gpsempfänger", UsbHelper.GetDevicPathebyClass ("GPS"), 38400);
            ////uavData.Add (gpsempfänger);
            ////empfängerusb = new UAVJoystick ("empfängerusb", "", new IntPtr ());
            ////uavData.Add (empfängerusb);
			
			// Servo Initialisierung
            servo1 = new PWM("servo1", 0, null, 1); //("Name", startwert, "null"=ausgabegerät_maestro, Kanal am Maestro)
            uavData.Add(servo1);

            servo1["LowLimit"].DoubleValue = -80;
            servo1["Neutral"].DoubleValue = 0;
            servo1["HighLimit"].DoubleValue = 80;
            servo1["Invert"].IntValue = 0;
		
		
            servo2 = new PWM ("servo2", 0, null, 2);
            uavData.Add (servo2);
			
            servo2 ["LowLimit"].DoubleValue = -80;
            servo2 ["Neutral"].DoubleValue = 0;
            servo2 ["HighLimit"].DoubleValue = 80;
            servo2 ["Invert"].IntValue = 0;
			
            servo3 = new PWM ("servo3", 90, null, 3);
            uavData.Add (servo3);
		
            servo3 ["LowLimit"].DoubleValue = -100;
            servo3 ["Neutral"].DoubleValue = 0;
            servo3 ["HighLimit"].DoubleValue = 100;
            servo3 ["Invert"].IntValue = 1;
			
            servo4 = new PWM ("servo4", 90, null, 4);
            uavData.Add (servo4);
		
            servo4 ["LowLimit"].DoubleValue = -100;
            servo4 ["Neutral"].DoubleValue = 0;
            servo4 ["HighLimit"].DoubleValue = 100;
            servo4 ["Invert"].IntValue = 1;
			
            servo5 = new PWM ("servo5", 0, null, 5); 
            uavData.Add (servo5);
			
            servo5 ["LowLimit"].DoubleValue = -100;
            servo5 ["Neutral"].DoubleValue = 0;
            servo5 ["HighLimit"].DoubleValue = 100;
            servo5 ["Invert"].IntValue = 0;
			
            //// PID CONFIG 
            kp_Höhe = new UAVParameter("kp_Höhe", 5, 0, 10, 0);
            kd_Höhe = new UAVParameter("kd_Höhe", 0, 0, 10, 0);
            ki_Höhe = new UAVParameter("ki_Höhe", 0, 0, 10, 0);
            uavData.Add(kp_Höhe);
            uavData.Add(kd_Höhe);
            uavData.Add(ki_Höhe);

            kp_Quer = new UAVParameter("kp_Quer", 5, 1, 10, 0);
            kd_Quer = new UAVParameter("kd_Quer", 0, 1, 10, 0);
            ki_Quer = new UAVParameter("ki_Quer", 0, 1, 10, 0);
            uavData.Add(kp_Quer);
            uavData.Add(kd_Quer);
            uavData.Add(ki_Quer);

            kp_Seite = new UAVParameter("kp_Seite", 5, 0, 10, 0);
            kd_Seite = new UAVParameter("kd_Seite", 0, 0, 10, 0);
            ki_Seite = new UAVParameter("ki_Seite", 0, 0, 10, 0);
            uavData.Add(kp_Seite);
            uavData.Add(kd_Seite);
            uavData.Add(ki_Seite);

            sp_Höhe = new UAVParameter("Höhe_SP", 0, -90, 90, 10);
            sp_Quer = new UAVParameter("Quer_SP", 0, -180, 180, 10);
            sp_Seite = new UAVParameter("Seite_SP", 0, -180, 180, 10);
            uavData.Add(sp_Höhe);
            uavData.Add(sp_Quer);
            uavData.Add(sp_Seite);

            PID_Out_Höhe = new UAVParameter("PID_Out_Höhe", 0, -100, 100, 0);
            PID_Out_Quer = new UAVParameter("PID_Out_Quer", 0, -100, 100, 0);
            PID_Out_Seite = new UAVParameter("PID_Out_Seite", 0, -100, 100, 0);
            uavData.Add(PID_Out_Höhe);
            uavData.Add(PID_Out_Quer);
            uavData.Add(PID_Out_Seite);
		}
		
	
		


		/// <summary>
		/// Main Method of the UAV Control Logic
		/// Will be called when the System is fully loaded and running
		/// If this Mehthod is completed the Program will exit :-(
		/// </summary>
		public override void run ()
		{
			initialised = true; // Wir sind fertig mit dem Laden und Starten unser eigentliches Programm daher erlauben wir auch den Empfang von Daten der Bodenstation
		servo1.ConnectHardware();		
            //// public ParameterPID (UAVParameter pG, UAVParameter iG, UAVParameter dG, UAVParameter pv, UAVParameter diff, UAVParameter output,UAVParameter sp)
            //PID_Höhe = new PID(kp_Höhe, ki_Höhe, kd_Höhe, lagesensor["theta"], lagesensor ["gyroalphastrich"],PID_Out_Höhe, sp_Höhe);
            //PID_Quer = new PID(kp_Quer, ki_Quer, kd_Quer, lagesensor["phi"], lagesensor ["gyrogammastrich"],PID_Out_Quer, sp_Quer);

            ////starte 100Hz Schleife  //keine Ahnung wie das geht
          //  lagesensor.DataRecieved+=new UAVCommons.UAVStructure.ValueChangedHandler(Ausgaberoutine);
        //    servo1.DoubleValue = -100;
            counter = 0;
            do { // läuft immer durch damit run nicht beendet und somit das Programm beendet wird
				Thread.Sleep(10);
               // Console.WriteLine("Counter: " + servo1.DoubleValue);
                counter++;
              //  servo1.DoubleValue++;
                servo1.DoubleValue = counter;
			servo2.DoubleValue = counter;
			servo3.DoubleValue = counter;
			servo4.DoubleValue = counter;
			servo5.DoubleValue = counter;

                PWM.UpdateServos();
                Console.WriteLine("throttle" + this.kd_Höhe.MaxDoubleValue);
                //  servo1.DoubleValue = counter;
			  //  servo1.DoubleValue = counter;
                if (counter == 99) counter = 0;
			} while (running == true);        
		}


	
		public void Ausgaberoutine (UAVSingleParameter param,bool isremote)//100Hz Schleife //keine Ahnung wie das geht ,...was ist die max aktualisierungsrate von Maestro und PWM-switch ???
		{
			
		        counter ++;
			//	sp_Quer.DoubleValue = empfängerusb ["Axis0"].DoubleValue/10;//bin mir nicht sicher welche Achse
			//	sp_Höhe.DoubleValue = empfängerusb ["Axis1"].DoubleValue/10; //bin mir nicht sicher welche Achse
        //sp_Quer.DoubleValue = 0;
        //sp_Höhe.DoubleValue  = 0;
				//SP_Seite = nicht definiert ->Richtung wird nur über Fernsteuerung geregelt
		 //   Console.WriteLine(param.Name + ":" + param.Value);
		//    servo1.DoubleValue = lagesensor["phi"].DoubleValue;
		Console.WriteLine("Phi:"+lagesensor["phi"].DoubleValue);
        //        PID_Höhe.Compute();
        //        PID_Quer.Compute();
        //        //PID_Seite.Compute();->Richtung wird nur über Fernsteuerung geregelt
   
        //        servo1.DoubleValue = Tools.Limit ((PID_Out_Quer.DoubleValue - PID_Out_Höhe.DoubleValue) , 100, -100);
        //        servo2.DoubleValue = Tools.Limit ((PID_Out_Quer.DoubleValue + PID_Out_Höhe.DoubleValue) , 100, -100);
        //        servo3.DoubleValue = Tools.Limit (-empfängerusb ["Axis4"].DoubleValue + lagesensor ["gyrobetastrich"].DoubleValue - (empfängerusb ["Axis2"].DoubleValue+30), 100, -100);
        //        servo4.DoubleValue = Tools.Limit (-empfängerusb ["Axis4"].DoubleValue - lagesensor ["gyrobetastrich"].DoubleValue + (empfängerusb ["Axis2"].DoubleValue+30), 100, -100);
        
        }
	}