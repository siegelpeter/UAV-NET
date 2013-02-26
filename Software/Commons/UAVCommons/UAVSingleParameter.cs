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
  //  [TypeConverter(typeof(UAVSingleParameterObjectConverter))]
	public class UAVSingleParameter : HierachyItem,ICloneable,IConvertible
	{
        protected string name = null;
        protected object value = null;

        protected Object min = null;

		protected Object max = null;
		private double parsedMin = Double.NaN;
        private double parsedMax = Double.NaN;
		public int updateRate = 100; ///< UpdateRate in ms  wenn 0 dann sofort Übertragung wenn Int32.MaxValue
		///
		public delegate void ValueChangedHandler (UAVSingleParameter param,bool isremote);

		/// <summary>
		/// The Event ValueChanged is fired when this Parameter has been Updates
		/// </summary>
		public event ValueChangedHandler ValueChanged;

		/// <summary>
		/// Contains the Parent Object if this UAVParameter is part of uavdata or part of a UAVStructor
		/// </summary>
		private HierachyItem parent = null;

		/// <summary>
		/// Creates a new UAVParameter Object
		/// </summary>
		/// <param name="paramname">A String representing the name of the Object in the Parent Structure eg. TestWert</param>
		/// <param name="paramvalue">The Value of the Parameter</param>
		/// <param name="Min">the LowerLimit of the Parameter</param>
		/// <param name="Max">The UpperLimit of the Parameter</param>
		/// <param name="urate">The UpdateRate @see updateRate</param>
		public UAVSingleParameter (string paramname, object paramvalue, object Min, object Max, int urate)
		{
			this.name = paramname;
			this.value = paramvalue;
			this.Max = Max;
			this.Min = Min;
			this.updateRate = urate;
		}

		/// <summary>
		/// Creates a new UAVParameter Object
		/// </summary>
		/// <param name="paramname">A String representing the name of the Object in the Parent Structure eg. TestWert</param>
		/// <param name="paramvalue">The Value of the Parameter</param>
		/// <param name="Min">the LowerLimit of the Parameter</param>
		/// <param name="Max">The UpperLimit of the Parameter</param>
		public UAVSingleParameter (string paramname, object paramvalue, object Min, object Max)
		{
			this.name = paramname;
			this.value = paramvalue;
			this.Max = Max;
			this.Min = Min;

		}

		/// <summary>
		/// Creates a new UAVParameter Object
		/// </summary>
		/// <param name="paramname">A String representing the name of the Object in the Parent Structure eg. TestWert</param>
		/// <param name="paramvalue">The Value of the Parameter</param>
        public UAVSingleParameter(string paramname, object paramvalue)
		{
			this.name = paramname;
			this.value = paramvalue;
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
		public virtual object Value {
			get {
				return value;
			}
			set
			{
                if (value == null) {
							
				//	throw new Exception(Name+ ":value may not be null");
				}
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
					return parsedMax;

				parsedMax =  Convert.ToDouble (Max);
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

				parsedMin =  Convert.ToDouble (Min);
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
        public virtual object Min
        {
			get {
				return min;
			}
			set {
                parsedMin = Double.NaN;
				min = value;
			}
		}
/// <summary>
		/// The Upper Limit for Value (Value cannot be higher than this)
		/// </summary>
        public virtual object Max
        {
			get {
				return max;
			}
			set {
                parsedMax = Double.NaN;
				max = value;
			}
		}

		/// <summary>
		/// Saves current element to an XMLDocument
		/// </summary>
		/// <param name='parent'>
		/// Parent.
		/// </param>
		/// <param name='doc'>
		/// Document.
		/// </param>
		public virtual void SavetoXML (XmlElement parent, XmlDocument doc)
		{
			XmlElement elem = doc.CreateElement ("Element");
			
			parent.AppendChild (elem);
                
			elem.SetAttribute ("Name", this.Name.Trim());
			elem.SetAttribute ("Path", this.GetStringPath ());
			elem.SetAttribute ("Type", this.GetType ().ToString ());
			if (Min != null)
				elem.SetAttribute ("Min", Min.ToString ());
			if (Max != null)
				elem.SetAttribute ("Max", Max.ToString ());
				if (updateRate != null)
			elem.SetAttribute ("URate", updateRate.ToString ());
			if (Value != null){
					if (Value.ToString() != "NaN")
					elem.SetAttribute ("Value", Value.ToString ());
				}else{
				elem.SetAttribute ("Value", "[[NULL]]");			
			}
			elem.SetAttribute ("isStructure", "false");
		
               
		}

		/// <summary>
		/// Returns the Value as Double
		/// </summary>
		public virtual double DoubleValue {
			get {
				if (System.Threading.Thread.CurrentThread.CurrentCulture.Name != "en-US") {
					Console.WriteLine ("Set Language");
					System.Threading.Thread.CurrentThread.CurrentCulture = new
						System.Globalization.CultureInfo ("en-US");
				}
				if (Value is double)
					return (double)Value;

				double retval =  double.Parse(Value.ToString(),System.Globalization.NumberStyles.Float);
                //if (retval.ToString () != Value.ToString ()) {
					
                //    throw new Exception ("double Convert failed " + retval.ToString () + " | " + Value.ToString () + " | " + retval.ToString ());
                //}
				return retval;

			}
			set { Value = value; }
		}

		/// <summary>
		/// Returns the Value as Int
		/// </summary>
        public virtual int IntValue
        {
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
        public virtual UAVSingleParameter SilentUpdate(string key, object value, bool isremote)
		{
			if ((this.value != null)&&(this.value.Equals (value)))
				return this;
			//this.value = value;
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

		protected void FireChangedEvent (bool isremote)
		{
			if (ValueChanged != null)
				ValueChanged (this, isremote);
		}

		public TypeCode GetTypeCode ()
		{
			throw new NotImplementedException ();
		}

		public bool ToBoolean (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public byte ToByte (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public char ToChar (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public DateTime ToDateTime (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public decimal ToDecimal (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public double ToDouble (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public short ToInt16 (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public int ToInt32 (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public long ToInt64 (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public sbyte ToSByte (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public float ToSingle (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public string ToString (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public object ToType (Type conversionType, IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public ushort ToUInt16 (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public uint ToUInt32 (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

		public ulong ToUInt64 (IFormatProvider provider)
		{
			throw new NotImplementedException ();
		}

        public virtual void LoadValues(XmlElement elem)
        {
          
        }
    }

	/// <summary>
	/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
	/// 
	/// Provides a method for performing a deep copy of an object.
	/// Binary Serialization is used to perform the copy.
	/// </summary>

    
}
