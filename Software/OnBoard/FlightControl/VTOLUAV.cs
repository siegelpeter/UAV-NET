using System;
using System.Collections.Generic;
using System.Text;
using UAVCommons;
using System.Threading;
using FlightControlCommons;
using FlightControlCommons.components;
using UAVCommons.Commands;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using PIDLibrary;


	/// <summary>
	/// VTOLUAV erbt seine Funktionen von dem BasisFluggerät
	/// Max seine Spielewiese
	/// </summary>
	[Serializable]
	public class VTOLUAV : UAVBase
	{  
		// Entkoppelungsglied Variablendefinition
	
		// Alle anderen Variablen
		IAsyncResult AusgabecallResult = null;
	
		PWM servo1;
		PWM servo2;
		PWM servo3;
		PWM servo4;
		PWM servo5;
		PWM servo6;
		
	
//		PID PID_Höhe;
//		PID PID_Quer;
//		PID PID_Seite;
	
		PIDS PIDS_Höhe;
		PIDS PIDS_Quer;
		PIDS PIDS_Seite;
		//------------------------------
		UAVParameter kp_Höhe;
		UAVParameter kd_Höhe;
		UAVParameter ki_Höhe;
		UAVParameter ks_Höhe;
	
		UAVParameter lp_Höhe;
		UAVParameter ld_Höhe;
		UAVParameter li_Höhe;
		UAVParameter ls_Höhe;
		//------------------------------
		UAVParameter kp_Quer;
		UAVParameter kd_Quer;
		UAVParameter ki_Quer;
		UAVParameter ks_Quer;
	
		UAVParameter lp_Quer;
		UAVParameter ld_Quer;
		UAVParameter li_Quer;
		UAVParameter ls_Quer;
		//------------------------------
		UAVParameter kp_Seite;
		UAVParameter kd_Seite;
		UAVParameter ki_Seite;
		UAVParameter ks_Seite;
	
		UAVParameter lp_Seite;
		UAVParameter ld_Seite;
		UAVParameter li_Seite;
		UAVParameter ls_Seite;
		//------------------------------
		UAVParameter sp_Höhe;
		UAVParameter sp_Quer;
		UAVParameter sp_Seite;
		
		UAVParameter output_Höhe;
		UAVParameter output_Quer;
		UAVParameter output_Seite;
	
	 	UAVParameter NewSeitePV; 
	
		UAVParameter SteuerMotorLeistung;
	UAVParameter HauptMotorLeistung;
	UAVParameter HauptMotorDiff;
	
	
		System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

		//UAVJoystick empfängerusb; // Usb Stick empfänger onboard
		AHRS lagesensor;
		GPS gpsempfänger;
			
		// GroundControl Jostick inputs
		UAVStructure joystick = new UAVStructure("Joystick", null);
        UAVParameter theta_rolerate = new UAVParameter("theta_rolerate", 0, -100, 100);
        UAVParameter psi_rolerate = new UAVParameter("psi_rolerate", 0, -100, 100);
        UAVParameter phi_rolerate = new UAVParameter("phi_rolerate", 0, -100, 100);
        UAVParameter throttle = new UAVParameter("throttle", 0, -100, 100);	
		UAVParameter totalpidgain = new UAVParameter("TotalPIDGain", 1, -100, 100);		
		UAVParameter Bearing;
	    DateTime timestamp;
	    //int counter = 0;
		bool running = true;
						
		public VTOLUAV ()	//  Constructor -> Is Called when the Object is created
		{                   // When the Object is loaded from the sd card this method will not be called
			
			uavData.Add(totalpidgain);
			joystick.values.Add(phi_rolerate);
            joystick.values.Add(theta_rolerate);
            joystick.values.Add(psi_rolerate);
            joystick.values.Add(throttle);
            uavData.Add(joystick);
           
			lagesensor = new AHRS ("lagesensor", UsbHelper.GetDevicPathebyClass ("AHRS"));
            lagesensor["cursettings"].Value = "371,376,379,17,-127,-87";
			
            uavData.Add (lagesensor);
			gpsempfänger = new GPS ("gpsempfänger", UsbHelper.GetDevicPathebyClass ("GPS"), 38400);
			uavData.Add (gpsempfänger);
		//	empfängerusb = new UAVJoystick ("empfängerusb", "", new IntPtr ());
			//uavData.Add (empfängerusb);
		watch.Start();
			// Servo Initialisierung
			servo1 = new PWM ("servo1", 0, null, 1); //("Name", startwert, "null"=ausgabegerät_maestro, Kanal am Maestro)
			uavData.Add (servo1);
			servo1 ["LowLimit"].DoubleValue = -100;
			servo1 ["Neutral"].DoubleValue = 0;
			servo1 ["HighLimit"].DoubleValue = 100;
			servo1 ["Invert"].IntValue = 0;
			servo1.SetHomePosition(0);
		    
			servo2 = new PWM ("servo2", 0, null, 2);
			uavData.Add (servo2);
			servo2 ["LowLimit"].DoubleValue = -100;
			servo2 ["Neutral"].DoubleValue = 0;
			servo2 ["HighLimit"].DoubleValue = 100;
			servo2 ["Invert"].IntValue = 0;
		    servo2.SetHomePosition(0);
		
			servo3 = new PWM ("servo3", 100, null, 3);
			uavData.Add (servo3);
			servo3 ["LowLimit"].DoubleValue = -80;
			servo3 ["Neutral"].DoubleValue = 0;
			servo3 ["HighLimit"].DoubleValue = 80;
			servo3 ["Invert"].IntValue = 1;
			servo3.SetHomePosition(100);
		
			servo4 = new PWM ("servo4", 100, null, 4);
			uavData.Add (servo4);
			servo4 ["LowLimit"].DoubleValue = -80;
			servo4 ["Neutral"].DoubleValue = 0;
			servo4 ["HighLimit"].DoubleValue = 80;
			servo4 ["Invert"].IntValue = 1;
			servo4.SetHomePosition(100);
		
			servo5 = new PWM ("servo5", -80, null, 5); 
			uavData.Add (servo5);
			
			servo5 ["LowLimit"].DoubleValue = -100;
			servo5 ["Neutral"].DoubleValue = 0;
			servo5 ["HighLimit"].DoubleValue = 100;
			servo5 ["Invert"].IntValue = 0;
		    servo5.SetHomePosition(-80);
		
			servo6 = new PWM ("servo6", -80, null, 6); 
			uavData.Add (servo6);
			
			servo6 ["LowLimit"].DoubleValue = -100;
			servo6 ["Neutral"].DoubleValue = 0;
			servo6 ["HighLimit"].DoubleValue = 100;
			servo6 ["Invert"].IntValue = 0;
			servo6.SetHomePosition(-80);
			
			// PID CONFIG --------------------------------------
			kp_Höhe = new UAVParameter ("kp_Höhe", 7  , 0, 10, 0);//Sinnvoll=1Höhe = new UAVParameter ("kp_Höhe", 1, max, min, updaterate)
			kd_Höhe = new UAVParameter ("kd_Höhe", 10 ,  -10, 10, 0);//Sinnvoll=-3  Achtung aus irgend einem Grund negativ !!!
			ki_Höhe = new UAVParameter ("ki_Höhe", 0  , 0, 10, 0);//Sinnvoll=1
			ks_Höhe = new UAVParameter ("ks_Höhe", 0.0, 0, 10, 0);//Sinnvoll=0.2
		
			lp_Höhe = new UAVParameter ("lp_Höhe", 1  , 0, 10, 0);
			ld_Höhe = new UAVParameter ("ld_Höhe", 1  , 0, 10, 0);
			li_Höhe = new UAVParameter ("li_Höhe", 1  , 0, 10, 0);
			ls_Höhe = new UAVParameter ("ls_Höhe", 0.2  , 0, 10, 0);
		
			uavData.Add (kp_Höhe);
			uavData.Add (kd_Höhe);
			uavData.Add (ki_Höhe);
			uavData.Add (ks_Höhe);
		
			uavData.Add (lp_Höhe);
			uavData.Add (ld_Höhe);
			uavData.Add (li_Höhe);
			uavData.Add (ls_Höhe);
			//----------------------------------------------------
			kp_Quer = new UAVParameter ("kp_Quer", 5  , 0, 10, 0);//Sinnvoll=1
			kd_Quer = new UAVParameter ("kd_Quer", 5  , 0, 10, 0);//Sinnvoll=3
			ki_Quer = new UAVParameter ("ki_Quer", 0  , 0, 10, 0);//Sinnvoll=1
			ks_Quer = new UAVParameter ("ks_Quer", 0.0, 0, 10, 0);//Sinnvoll=0.2
		
			lp_Quer = new UAVParameter ("lp_Quer", 1  , 0, 10, 0);	
			ld_Quer = new UAVParameter ("ld_Quer", 1  , 0, 10, 0);
			li_Quer = new UAVParameter ("li_Quer", 1  , 0, 10, 0);
			ls_Quer = new UAVParameter ("ls_Quer", 0.2 , 0, 10, 0);
		
			uavData.Add (kp_Quer);
			uavData.Add (kd_Quer);		
			uavData.Add (ki_Quer);
			uavData.Add (ks_Quer);
		
			uavData.Add (lp_Quer);
			uavData.Add (ld_Quer);		
			uavData.Add (li_Quer);
			uavData.Add (ls_Quer);
			//----------------------------------------------------
			kp_Seite = new UAVParameter ("kp_Seite", -3  , -10, 10, 0);
			kd_Seite = new UAVParameter ("kd_Seite", 6  , -10, 10, 0);
			ki_Seite = new UAVParameter ("ki_Seite", 0  , -10, 10, 0);
			ks_Seite = new UAVParameter ("ks_Seite", 0  , -10, 10, 0);
		
			lp_Seite = new UAVParameter ("lp_Seite", 0.5  , 0, 10, 0);
			ld_Seite = new UAVParameter ("ld_Seite", 0.5  , 0, 10, 0);
			li_Seite = new UAVParameter ("li_Seite", 1  , 0, 10, 0);
			ls_Seite = new UAVParameter ("ls_Seite", 0  , 0, 10, 0);
		
			uavData.Add (kp_Seite);
			uavData.Add (kd_Seite);
			uavData.Add (ki_Seite);
			uavData.Add (ks_Seite);
		
			uavData.Add (lp_Seite);
			uavData.Add (ld_Seite);
			uavData.Add (li_Seite);
			uavData.Add (ls_Seite);
			//----------------------------------------------------
			sp_Höhe  = new UAVParameter ("Höhe_SP" , 0,  -90,  90, 10);
			sp_Quer  = new UAVParameter ("Quer_SP" , 0, -180, 180, 10);
			sp_Seite = new UAVParameter ("Seite_SP", 0, -180, 180, 10);
			uavData.Add (sp_Höhe);
			uavData.Add (sp_Quer);
			uavData.Add (sp_Seite);

			output_Höhe  = new UAVParameter("PID_Out_Höhe" ,0,-100,100,0);
			output_Quer  = new UAVParameter("PID_Out_Quer" ,0,-100,100,0);
			output_Seite = new UAVParameter("PID_Out_Seite",0,-100,100,0);
		    
			uavData.Add(output_Höhe);
			uavData.Add(output_Quer);
			uavData.Add(output_Seite);
			SteuerMotorLeistung = new UAVParameter("SteuerMotorLeistung",0,0,100,0);
				HauptMotorLeistung = new UAVParameter("HauptMotorLeistung",-80,-100,100,0);
				HauptMotorDiff = new UAVParameter("HauptMotorDiff",0,-20,20,0);
			uavData.Add(SteuerMotorLeistung);
			uavData.Add(HauptMotorLeistung);
			uavData.Add(HauptMotorDiff);
		    
		
			NewSeitePV = new UAVParameter("NewSeitePV",0,-180,180,0);
		    uavData.Add(NewSeitePV);
			uavData.Add(output_Höhe);
			}
		
		/// <summary>
		/// Main Method of the UAV Control Logic
		/// Will be called when the System is fully loaded and running
		/// If this Mehthod is completed the Program will exit :-(
		/// </summary>
		public override void run ()
		{
			initialised = true; // Wir sind fertig mit dem Laden und Starten unser eigentliches Programm daher erlauben wir auch den Empfang von Daten der Bodenstation
			SteuerMotorLeistung.DoubleValue =0;
		    servo5.DoubleValue = -80;
		    servo6.DoubleValue = -80;
		
		Console.WriteLine("Warte auf Motoren Init");
		Thread.Sleep(1000);
		Console.WriteLine("Motoren Init Done");
		
		// public ParameterPID (UAVParameter pG, UAVParameter iG, UAVParameter dG, UAVParameter pv, UAVParameter diff, UAVParameter output,UAVParameter sp)
//			PID_Höhe = new PID(kp_Höhe, ki_Höhe, kd_Höhe, lagesensor["theta"], lagesensor ["gyroalphastrich"],output_Höhe, sp_Höhe);
//		 	PID_Quer = new PID(kp_Quer, ki_Quer, kd_Quer, lagesensor["phi"], lagesensor ["gyrogammastrich"],output_Quer, sp_Quer);
//			//PID_Seite = new PID(kp_Seite, ki_Seite, kd_Seite, lagesensor["psi"], lagesensor ["gyrobetastrich"],output_Quer, sp_Seite);
		
		
			//	public PIDS (	UAVParameter kp, UAVParameter ki, UAVParameter kd, UAVParameter ks, 
		    //       			UAVParameter lp, UAVParameter li, UAVParameter ld, UAVParameter ls, UAVParameter pv, UAVParameter diff, UAVParameter output,UAVParameter sp)
			PIDS_Höhe =new PIDS (	kp_Höhe, ki_Höhe, kd_Höhe, ks_Höhe, 
		                     lp_Höhe, li_Höhe, ld_Höhe, ls_Höhe, (UAVParameter)lagesensor["theta"], (UAVParameter)lagesensor ["gyroalphastrich"], output_Höhe,sp_Höhe,totalpidgain);
			
			PIDS_Quer =new PIDS (	kp_Quer, ki_Quer, kd_Quer, ks_Quer, 
		                     lp_Quer, li_Quer, ld_Quer, ls_Quer, (UAVParameter)lagesensor["phi"]  , (UAVParameter)lagesensor ["gyrogammastrich"], output_Quer,sp_Quer,totalpidgain);
		    PIDS_Seite =new PIDS (	kp_Seite, ki_Seite, kd_Seite, ks_Seite, 
		             				lp_Seite, li_Seite, ld_Seite, ls_Seite, NewSeitePV , (UAVParameter)lagesensor ["gyrobetastrich"], output_Seite, sp_Seite,totalpidgain);
		
			sp_Seite.DoubleValue = 0;
		//sp_Seite.DoubleValue  = 0;
			lagesensor.DataRecieved+=new UAVCommons.UAVStructure.ValueChangedHandler(MakeAusgabeAsync);			
			do { // läuft immer durch damit run nicht beendet und somit das Programm beendet wird
				//Thread.Sleep(1000);
			//Console.WriteLine("output_Höhe" + output_Höhe);
			//counter = 0;
			//	Console.WriteLine("Achse0 = " + empfängerusb ["Axis0"].DoubleValue + "   Achse1 =  " + empfängerusb ["Axis1"].DoubleValue + "   Achse2 =  " + empfängerusb ["Axis2"].DoubleValue + "   Achse3 = " + empfängerusb ["Axis3"].DoubleValue +  "   Achse4 = " + empfängerusb ["Axis4"].DoubleValue + "   Achse5 = " + empfängerusb ["Axis5"].DoubleValue +"   Achse6 = " + empfängerusb ["Axis6"].DoubleValue);	
				Console.WriteLine("Servo5 = " + servo5.DoubleValue +"     Servo6 = " + servo6.DoubleValue);
			//	Console.WriteLine("sp_Quer = " + sp_Quer.DoubleValue +"     sp_Höhe = " + sp_Höhe.DoubleValue +"     sp_Seite = " + sp_Seite.DoubleValue);
				Thread.Sleep(3000);	
			} while (running == true);        
		}
	
	//Entkoppelungsglied damit eine Langsame Ausführung von Ausgaberoutine nicht den Lagesensor ausbremst
	public void MakeAusgabeAsync (UAVSingleParameter param,bool isremote)
	{
		//Console.WriteLine(watch.ElapsedMilliseconds+"ms");
		if ((AusgabecallResult != null)&&(!AusgabecallResult.IsCompleted)){ // Wenn noch immer nicht fertig aber bereits ausgeführt
	   	Console.WriteLine("Programm zu langsam");
		}else{ // Noch nicht gestartet oder vom letzten Mal schon fertig dann ausführen 
		AHRS.ValueChangedHandler mehtodenaufruf = new AHRS.ValueChangedHandler(Ausgaberoutine); // Speicher ausgaberoutine in Variable
		AusgabecallResult = mehtodenaufruf.BeginInvoke(param,isremote,null,null); //Rufe Ausgabe asyncron auf
		}	

		    }

	public double ComputePV (UAVParameter sensor, UAVParameter sp)
	{
		double PV = 0;
		if (sensor.DoubleValue < 0){ //=WENN(A6<0; 360 + A6;A6)
			PV = 360+sensor.DoubleValue;
			
		}else{
			PV = sensor.DoubleValue;		
		}
		double delta = sp.DoubleValue-PV;
		//=WENN(D6<-180;360+D6;WENN(D6>180;D6-360;D6))
		double corrdelta = 0;
		if (delta < -180){
			corrdelta = 360+delta;
		}else{
		
			if (delta > 180){
				corrdelta = delta-360;
			}else{
				corrdelta = delta;
			}
		}
			//Console.WriteLine("Psi:"+(int)sensor.DoubleValue+" PV: "+(int)PV+" delta SP-PV "+(int)delta+" delta"+(int)corrdelta+" NeuerPV"+(int)(sp.DoubleValue-corrdelta)+" Setpoint: "+(int)sp.DoubleValue+" Gyrobstrich:"+(int)lagesensor["gyrobetastrich"].DoubleValue);
		return sp.DoubleValue-corrdelta;
	}
	
	
		public void Ausgaberoutine (UAVSingleParameter param,bool isremote)//100Hz Schleife //keine Ahnung wie das geht ,...was ist die max aktualisierungsrate von Maestro und PWM-switch ???
		{
		

		System.Diagnostics.Stopwatch w2 = new System.Diagnostics.Stopwatch();
		w2.Start();
	try{
		//	Console.WriteLine(watch.ElapsedMilliseconds+" ms Phi "+lagesensor["phi"].DoubleValue+" Theta"+lagesensor["theta"].DoubleValue+" psi"+lagesensor["psi"].DoubleValue);
			//counter ++;
			//	sp_Quer.DoubleValue = empfängerusb ["Axis0"].DoubleValue/10;//bin mir nicht sicher welche Achse
			//	sp_Höhe.DoubleValue = empfängerusb ["Axis1"].DoubleValue/10; //bin mir nicht sicher welche Achse
				sp_Quer.DoubleValue = phi_rolerate.DoubleValue;//(empfängerusb ["Axis0"].DoubleValue+18)/2;
				sp_Höhe.DoubleValue  = theta_rolerate.DoubleValue;//(empfängerusb ["Axis1"].DoubleValue+03)/3;
		
				NewSeitePV.DoubleValue = ComputePV((UAVParameter)lagesensor["psi"],sp_Seite);
				sp_Seite.DoubleValue = 0;// sp_Seite.DoubleValue + psi_rolerate.DoubleValue/100;//(empfängerusb ["Axis2"].DoubleValue+18)/20;
		
			
		
				if(sp_Seite.DoubleValue<0)
					{sp_Seite.DoubleValue=sp_Seite.DoubleValue + 360;}
				if(sp_Seite.DoubleValue>360)
					{sp_Seite.DoubleValue = sp_Seite.DoubleValue-360;}
					
			
				
		    	PIDS_Höhe.PIDS_Calc();
				PIDS_Quer.PIDS_Calc();
				PIDS_Seite.PIDS_Calc();
		
	//	Wie gehe ich sicher dass die PIDS fertig mit der Berechnung sind bevor ich die output-Werte in Folge zusammenzähle?????
	//	(möchte keine alten Werte zusammenzählen)
		      

		servo1.DoubleValue = Tools.Limit((output_Quer.DoubleValue - output_Höhe.DoubleValue) , -80, 80);
		servo2.DoubleValue = Tools.Limit((output_Quer.DoubleValue + output_Höhe.DoubleValue) , -80, 80); 
		servo3.DoubleValue = Tools.Limit((SteuerMotorLeistung.DoubleValue)*1.8-80 + output_Seite.DoubleValue, -100, 100); //mal 1.1 //(empfängerusb ["Axis2"].DoubleValue+18)/2
		servo4.DoubleValue = Tools.Limit((SteuerMotorLeistung.DoubleValue)*1.8-80 - output_Seite.DoubleValue , -100, 100);
		//((empfängerusb ["Axis4"].DoubleValue)+90)*-0.8+110
		//	HauptMotorLeistung.Value = (int)Tools.ScaleValue(throttle.DoubleValue,-100,100,-80,100);
		//	Console.WriteLine(HauptMotorLeistung.DoubleValue);
		servo5.DoubleValue = HauptMotorLeistung.DoubleValue - HauptMotorDiff.DoubleValue;
		servo6.DoubleValue = HauptMotorLeistung.DoubleValue + HauptMotorDiff.DoubleValue;
		PWM.UpdateServos();
	
	//	Console.WriteLine("HauptMotor Left "+(HauptMotorLeistung.DoubleValue - HauptMotorDiff.DoubleValue)+" Right"+(HauptMotorLeistung.DoubleValue + HauptMotorDiff.DoubleValue)+"Differenz: "+HauptMotorDiff.DoubleValue);
				//servo5.DoubleValue = (empfängerusb ["Axis4"].DoubleValue)*-1.8-80;
			//	servo6.DoubleValue = (empfängerusb ["Axis4"].DoubleValue)*-1.8-80;
		}catch(Exception ex){
			Console.WriteLine(ex.Message+" "+ex.StackTrace.ToString());

		}finally{
            if (w2.ElapsedMilliseconds > 10) Console.WriteLine("too long" + w2.ElapsedMilliseconds);

		}
			
		//	servo5.DoubleValue = Tools.Limit((((empfängerusb ["Axis4"].DoubleValue)+90)*-1+100)*1 , 100, -100);
				//- (empfängerusb ["Axis2"].DoubleValue+30)/2
				//	+ (empfängerusb ["Axis2"].DoubleValue+30)/2
				//+ lagesensor ["gyrobetastrich"].DoubleValue 
				//- lagesensor ["gyrobetastrich"].DoubleValue 
		
	//	Warum sollte ich Servo-Werte limitieren ?? -> sind doch bereits limitiert !! -> Limits kommen weg
	//	Wie gehe ich sicher dass die Umrechnung in PWM genau nach dem Setzen der Servo-Werte passiert????????

			
		  	//Console.WriteLine("Servo3=" + servo3.DoubleValue +"       Servo4=" + servo4.DoubleValue);
			//Console.WriteLine("Wert Servo1:"+Tools.Limit ((output_Quer.DoubleValue - output_Höhe.DoubleValue) , 80, -80));
//		
//		servo3.DoubleValue =(empfängerusb ["Axis4"].DoubleValue +90)*-0.8+110 - output_Quer.DoubleValue ;
//		servo4.DoubleValue =(empfängerusb ["Axis4"].DoubleValue +90)*-0.8+110 + output_Quer.DoubleValue ;
		}
	}