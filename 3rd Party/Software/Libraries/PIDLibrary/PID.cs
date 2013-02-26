
using System;
using System.Threading;
using UAVCommons;

namespace PIDLibrary
{
	/// <summary>
	/// A Pid which accepts UAVParameters as Values
	/// </summary>
	[Serializable]
	public class PID
	{
		private UAVParameter kp;	//Gain proportional
		private UAVParameter ki;	//Gain proportional
		private UAVParameter kd;
		
		private UAVParameter diff; 	// Drehrate vom Gyro
		private UAVParameter pv;	// Lage - Winkel
		private UAVParameter sp;	//Soll-Winkel
		private UAVParameter output;//Ergebnis
		
		private DateTime lastUpdate;//Running Values
		
		private UAVParameter totalgain;// Gewichtung für alle gains gemeinsam
		
		double pTerm = 0;
		double iTerm = 0;
		double dTerm = 0;
		double dT = 0;
		double out_temp = 0;
		
		public PID (UAVParameter kp, UAVParameter ki, UAVParameter kd, UAVParameter pv, UAVParameter diff, UAVParameter output,UAVParameter sp,UAVParameter totalgain)
		{
			this.pv = pv;
			this.diff = diff;
			this.output = output;
			this.totalgain = totalgain;
			this.sp = sp;
			this.kp = kp;
			this.ki = ki;
			this.kd = kd;
		}

		public void Compute ()
		{		
			double pv_temp = pv.DoubleValue;
			double sp_temp = sp.DoubleValue;
			double diff_temp = diff.DoubleValue;
						
			pv_temp   = Tools.ScaleValue (pv_temp  , Convert.ToDouble (pv.Min)  , Convert.ToDouble (pv.Max)  , -1.0f, 1.0f);//We need to scale the pv to +/- 100%
			sp_temp   = Tools.ScaleValue (sp_temp  , Convert.ToDouble (sp.Min)  , Convert.ToDouble (sp.Max)  , -1.0f, 1.0f);//We also need to scale the setpoint
			diff_temp = Tools.ScaleValue (diff_temp, Convert.ToDouble (diff.Min), Convert.ToDouble (diff.Max), -1.0f, 1.0f);
			//-------------------------------------------------
			pTerm = (sp_temp - pv_temp) * (kp.DoubleValue*totalgain.DoubleValue) ;
			//-------------------------------------------------
			//double iTerm = 0.0f;
			//double err = sp_temp - pv_temp;//Now the error is in percent... Calculation of the error
			//if (lastUpdate != null) { // Nur wenn LastUpdate initialisiert ist (einen gültigen Wert hat)
			//	double dT = (DateTime.Now - lastUpdate).TotalSeconds;
			//		errSum = Tools.Limit(errSum + dT * err,0.2,-0.2);//Compute the integral if we have to...
			//		iTerm = ki.DoubleValue * errSum;
			//}
			//-------------------------------------------------
			dT = (DateTime.Now - lastUpdate).TotalSeconds;
			lastUpdate = DateTime.Now;
			iTerm = ((ki.DoubleValue*totalgain.DoubleValue) * (Tools.Limit(iTerm,0.1,-0.1)) + dT *(sp_temp - pv_temp));
			//-------------------------------------------------
    		dTerm = diff_temp * (kd.DoubleValue*totalgain.DoubleValue);
			//-------------------------------------------------
			out_temp = pTerm + iTerm + dTerm;//Now we have to scale the output value to match the requested scale
		  	//Console.WriteLine("PID   P:" + pTerm +"PID   I:" + iTerm +"PID   D:" + dTerm );
			out_temp = Tools.Limit (out_temp,  -1.0f,1.0f);
			out_temp = Tools.ScaleValue (out_temp, -1.0f, 1.0f, Convert.ToDouble (output.Min), Convert.ToDouble (output.Max));
			output.DoubleValue = out_temp;
		}
	}
}
