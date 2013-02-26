using System;
using System.Collections.Generic;

using System.Text;

namespace UAVCommons
{
    /// <summary>
    /// Provides Methods to Work With values 
    /// </summary>
   public  class ValueControl
    {

       /// <summary>
       /// Keeps a given Value in Range
       /// </summary>
       /// <param name="Wert">A double Value to keep in Range</param>
       /// <param name="Min">Lower Range Limit</param>
       /// <param name="Max">Upper Range Limit</param>
       /// <returns></returns>
       public static Double ValueRange(Double Wert, Double Min, Double Max)
       {
           if (Wert < Min) Wert = Min;
           if (Wert > Max) Wert = Max;
           return Wert;
       }
       
       /// <summary>
       /// To be Implemented 
       /// Schlussrechnung
       /// </summary>
       /// <param name="oldValueandRange"></param>
       /// <param name="newValueandRange"></param>
       /// <returns></returns>
       public static Double TransformToRange(UAVParameter oldValueandRange, UAVParameter newValueandRange) {
           return 0;
       }

    }
}
