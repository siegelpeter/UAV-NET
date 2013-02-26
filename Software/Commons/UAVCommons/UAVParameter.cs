using System;
using System.Collections.Generic;

using System.Text;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml;

using System.ComponentModel;
using UAVCommons.Datamodel;

namespace UAVCommons
{
	/// <summary>
	/// A Simple Variable for uavdata 
	/// </summary>
	[Serializable]
  	public class UAVParameter : UAVStructure,ICloneable
	{
		private double parsedMin = Double.NaN;
        private double parsedMax = Double.NaN;
		public int updateRate = 100; ///< UpdateRate in ms  wenn 0 dann sofort Übertragung wenn Int32.MaxValue

      
		/// <summary>
		/// Contains the Parent Object if this UAVParameter is part of uavdata or part of a UAVStructor
		/// </summary>
		private HierachyItem parent = null;

	    private UAVSingleParameter minparam = null;
	    private UAVSingleParameter maxparam = null;
        private UAVSingleParameter urateparam = null;

		/// <summary>
		/// Creates a new UAVParameter Object
		/// </summary>
		/// <param name="paramname">A String representing the name of the Object in the Parent Structure eg. TestWert</param>
		/// <param name="paramvalue">The Value of the Parameter</param>
		/// <param name="Min">the LowerLimit of the Parameter</param>
		/// <param name="Max">The UpperLimit of the Parameter</param>
		/// <param name="urate">The UpdateRate @see updateRate</param>
        public UAVParameter(string paramname, object paramvalue, object Min, object Max, int urate)
            : base(paramname, "")
		{
			
			this.value = paramvalue;
			CreateProperties( Min,Max,urate);
            this.Max = Max;
            this.Min = Min;
            this.updateRate = urate;
            
		}

	    private void CreateProperties(object Min, object Max, int rate)
	    {
            minparam = new UAVSingleParameter("Min", Min, Int16.MinValue, Int16.MaxValue,rate);
            maxparam = new UAVSingleParameter("Max", Max, Int16.MinValue, Int16.MaxValue,rate);
            urateparam = new UAVSingleParameter("UpdateRate", rate, int.MinValue, int.MaxValue,rate);
			urateparam.Value = updateRate;
	        values.Add(minparam);
	        values.Add(maxparam);
	        values.Add(urateparam);
	        minparam.ValueChanged += new UAVSingleParameter.ValueChangedHandler(minparam_ValueChanged);
	        maxparam.ValueChanged += new UAVSingleParameter.ValueChangedHandler(maxparam_ValueChanged);
	        urateparam.ValueChanged += new UAVSingleParameter.ValueChangedHandler(urateparam_ValueChanged);
	    }

	    void urateparam_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            updateRate = urateparam.IntValue;

        }

        void maxparam_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            max = maxparam.Value;
        }

        void minparam_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            min = minparam.Value;
        }

		/// <summary>
		/// Creates a new UAVParameter Object
		/// </summary>
		/// <param name="paramname">A String representing the name of the Object in the Parent Structure eg. TestWert</param>
		/// <param name="paramvalue">The Value of the Parameter</param>
		/// <param name="Min">the LowerLimit of the Parameter</param>
		/// <param name="Max">The UpperLimit of the Parameter</param>
        public UAVParameter(string paramname, object paramvalue, object Min, object Max)
            : base(paramname, "")
		{
		
			this.value = paramvalue;
			this.Max = Max;
			this.Min = Min;

            CreateProperties(Min, Max,100);
		}

		/// <summary>
		/// Creates a new UAVParameter Object
		/// </summary>
		/// <param name="paramname">A String representing the name of the Object in the Parent Structure eg. TestWert</param>
		/// <param name="paramvalue">The Value of the Parameter</param>
        public UAVParameter(string paramname, object paramvalue)
            : base(paramname, "")
		{
			
			this.value = paramvalue;
            Min = -100;
            Max = 100;
            CreateProperties(Min, Max, 100);
		}

		/// <summary>
		/// Name of the Parameter e.g. Wert1
		/// </summary>
		public override string Name {
			get {
				return name;
			}
			set {
				name = value;
             
			}
		}

		/// <summary>
		/// Value of the Parameter
		/// </summary>
		public override object Value {
			get {
				return base.value;
			}
			set
			{
			    if (UAVBase.KeepInRange == true)
			    {
			        try
			        {
			            value = Tools.Limit(Convert.ToDouble(value), this.MinDoubleValue, this.MaxDoubleValue);
			        }
			        catch (Exception ex)
			        {
			        }

			    }

			    this.value = value;
			
			        FireChangedEvent(false);


			    }
			}
		

		public virtual double MaxDoubleValue {
			get {
                if (!double.IsNaN(parsedMax))
                {
                    return parsedMax;
                }
                parsedMax = Convert.ToDouble(Max);
				return parsedMax;
			}
			set {
				Max = value;
				parsedMax = value;

			}
		}

		public virtual double MinDoubleValue {
			get {
                if (!double.IsNaN(parsedMin))
					return parsedMin;

				parsedMin = Convert.ToDouble (Min);
                return parsedMin;
			}
			set {
				Min = value;
				parsedMin = value;
			}
		}
	/// <summary>
		/// The Lower Limit for Value (Value cannot be lower than this)
		/// </summary>
		public object Min {
			get {
				return min;
			}
			set {
                parsedMin = Double.NaN;
				min = value;
			    if (minparam != null) minparam.Value = min;
			}
		}
/// <summary>
		/// The Upper Limit for Value (Value cannot be higher than this)
		/// </summary>
		public object Max {
			get {
				return max;
			}
			set {
                parsedMax = Double.NaN;
				max = value;
			    if (maxparam != null) maxparam.Value = max;
			}
		}

        public int UpdateRate
        {
            get
            {
                return updateRate;
            }
            set
            {
               
                updateRate = value;
                
            }
        }



		/// <summary>
		/// Returns the Value as Double
		/// </summary>
		public override double DoubleValue {
			get {
               
				if (Value is double)
					return (double)Value;
				double retval = double.Parse(Value.ToString(),System.Globalization.NumberStyles.Float);
				if (retval.ToString() != Value.ToString()) {

					throw new Exception("double Convert failed "+retval.ToString()+" | "+Value.ToString()+" | "+retval.ToString());
				}
					return retval;
			}
			set { Value = value; }
		}

		/// <summary>
		/// Returns the Value as Int
		/// </summary>
		public override int IntValue {
			get { return Convert.ToInt32 (Value); }
			set { Value = value; }
		}


		/// <summary>
		/// Parent object of the Parameter e.g. uavdata or a Structure like AHRS or GPS
		/// </summary>
		public override HierachyItem Parent {
			get {
				return this.parent;
			}
			set {
				parent = value;
				stringPathCache = null;
			}
		}


		/// <summary>
		/// Updates the Value of the Parameter without notifing everbody about it
		/// </summary>
		/// <param name="value"></param>
		public virtual UAVParameter SilentUpdate (string key, object value, bool isremote)
		{
			if (this.value.Equals (value))
				return this;
			this.value = value;
			if (((Min != null) && (Max != null) && (this.value != null)) && ((Min is double) && (Max is double) && (value is double))) {
				this.value = ValueControl.ValueRange ((double)value, (double)Min, (double)Max);
			} else {
				this.value = value;
			}
         
			FireChangedEvent (isremote);
			return this;
         
		}

		/// <summary>
		/// creates a duplicate of the Parameter
		/// </summary>
		/// <returns></returns>
		public virtual object Clone ()
		{
			UAVParameter newParam = null;
			if (this.value is double) {
				newParam = new UAVParameter ((string)this.name.Clone (),
                                                        Convert.ToDouble (value.ToString ()));
			} else {

				newParam = new UAVParameter ((string)this.name.Clone (),
                                            ObjectCopier.Clone<object> (this.value));
			}
			newParam.updateRate = this.updateRate;
			newParam.Min = this.Min;
			newParam.Max = this.Max;
			newParam.parent = this.parent;
			return newParam;
              
		}

	

		public TypeCode GetTypeCode ()
		{
			throw new NotImplementedException ();
		}

	
	}

	/// <summary>
	/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// 
	/// Provides a method for performing a deep copy of an object.
	/// Binary Serialization is used to perform the copy.
	/// </summary>

	public static class ObjectCopier
	{
		/// <summary>
		/// Perform a deep Copy of the object.
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T Clone<T> (T source)
		{
			if (!typeof(T).IsSerializable) {
				throw new ArgumentException ("The type must be serializable.", "source");
			}

			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals (source, null)) {
				return default(T);
			}

			IFormatter formatter = new BinaryFormatter ();
			Stream stream = new MemoryStream ();
			using (stream) {
				formatter.Serialize (stream, source);
				stream.Seek (0, SeekOrigin.Begin);
				return (T)formatter.Deserialize (stream);
			}
		}
	}    
    
}
