using System;

namespace UAVCommons
{
	
	/// <summary>
	/// Werkzeugkiste 
	/// </summary>
	public class Tools  
	

    {
        /// <summary>
        /// Inverts the Value 
        /// e.g.:
        /// 50 ( min -100, max 100) => -50
        /// </summary>
        /// <param name="value">A value between min and max </param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double Invert(double value, double min, double max){
            return (max - value) + min;              
        }

	    
		/// <summary>
		/// Limit the specified value, min and max.
		/// </summary>
		public static double Limit (double value, double min, double max)
		{
            if (min > max) {
                throw new Exception("Min > Max");
                }
			if (value > max)
				return max;
			if (value < min)
				return min;
			return value;
		}
		
		/// <summary>
		/// Scales the Values from an actual Range to a new Range
		/// </summary>	
		public static double ScaleValue (double value, double valuemin, double valuemax, double scalemin, double scalemax)
		{
			double vPerc = (value - valuemin) / (valuemax - valuemin);
			double bigSpan = vPerc * (scalemax - scalemin);

			double retVal = scalemin + bigSpan;

			return retVal;
		}		
		
		
		
	}
}

