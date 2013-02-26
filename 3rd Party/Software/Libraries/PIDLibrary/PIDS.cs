using System;
using System.Threading;
using UAVCommons;

namespace PIDLibrary
{
	/// <summary>
	/// A Pid which accepts UAVParameters as Values
	/// </summary>
	[Serializable]
	public class PIDS
	{
		private UAVParameter kp;	//Gain proportional
		private UAVParameter ki;	//Gain integral
		private UAVParameter kd;	//Gain differnzial
		private UAVParameter ks;	//Konstante
		
		private UAVParameter lp;	//Limit pTerm
		private UAVParameter li;	//Limit iTerm
		private UAVParameter ld;	//Limit dTerm
		private UAVParameter ls;	//Totbereich sTerm
		
		private UAVParameter diff; 	// Drehrate vom Gyro
		private UAVParameter pv;	// Lage - Winkel
		private UAVParameter sp;	//Soll-Winkel
		private UAVParameter output;//Ergebnis
		
		private DateTime lastUpdate; //Running Values
			private UAVParameter totalgain;// Gewichtung für alle gains gemeinsam
	
		double pTerm=0;
		double iTerm=0;
		double dTerm=0;
		double sTerm=0;
		double dT=0;
		double direction = 0;
		double out_temp =0;
				
		public PIDS (UAVParameter kp, UAVParameter ki, UAVParameter kd, UAVParameter ks, 
		             UAVParameter lp, UAVParameter li, UAVParameter ld,	UAVParameter ls, UAVParameter pv, UAVParameter diff, UAVParameter output,UAVParameter sp,UAVParameter totalgain)
		{
			this.pv = pv;
			this.diff = diff;
			this.output = output;
			this.sp = sp;
			this.totalgain = totalgain;
			this.kp = kp;
			this.ki = ki;
			this.kd = kd;
			this.ks = ks;
			
			this.lp = lp;
			this.li = li;
			this.ld = ld;
			this.ls = ls;
		}
		
		public void PIDS_Calc ()
		{
			double pv_temp = pv.DoubleValue;
			double sp_temp = sp.DoubleValue;
			double diff_temp = diff.DoubleValue;
						
			pv_temp   = Tools.ScaleValue (pv_temp  , Convert.ToDouble (pv.Min)  , Convert.ToDouble (pv.Max)  , -1.0f, 1.0f);//We need to scale the pv to +/- 100%
			sp_temp   = Tools.ScaleValue (sp_temp  , Convert.ToDouble (sp.Min)  , Convert.ToDouble (sp.Max)  , -1.0f, 1.0f);//We also need to scale the setpoint
			diff_temp = Tools.ScaleValue (diff_temp, Convert.ToDouble (diff.Min), Convert.ToDouble (diff.Max), -1.0f, 1.0f);
			//-------------------------------------------------------------------------
			pTerm = Tools.Limit ((sp_temp - pv_temp) * kp.DoubleValue,-(lp.DoubleValue*totalgain.DoubleValue), (lp.DoubleValue*totalgain.DoubleValue));
			//-------------------------------------------------------------------------
			dT = (DateTime.Now - lastUpdate).TotalSeconds;
			lastUpdate = DateTime.Now;
			iTerm = Tools.Limit((iTerm + (ki.DoubleValue*totalgain.DoubleValue) * dT *(sp_temp - pv_temp)),-(li.DoubleValue*totalgain.DoubleValue),(li.DoubleValue*totalgain.DoubleValue));
			//-------------------------------------------------------------------------
    		dTerm = Tools.Limit (diff_temp * (kd.DoubleValue*totalgain.DoubleValue),-(ld.DoubleValue*totalgain.DoubleValue),(ld.DoubleValue*totalgain.DoubleValue));
			//-------------------------------------------------------------------------
			direction = ((Math.Abs(sp_temp - pv_temp))/(sp_temp - pv_temp));
			if (Math.Abs(sp_temp - pv_temp)<(ls.DoubleValue*totalgain.DoubleValue)){direction = 0.0;}
			sTerm = (ks.DoubleValue*totalgain.DoubleValue) * direction;
			//-------------------------------------------------------------------------		
			out_temp = Tools.Limit(pTerm + iTerm + dTerm + sTerm,-1.0,1.0);
			//Now we have to scale the output value to match the requested scale
		 	//  Console.WriteLine("PID   P:" + pTerm +"PID   I:" + iTerm +"PID   D:" + dTerm );
			output.DoubleValue = Tools.ScaleValue (out_temp, -1.0f, 1.0f, Convert.ToDouble (output.Min), Convert.ToDouble (output.Max));		
		}
	}
}

