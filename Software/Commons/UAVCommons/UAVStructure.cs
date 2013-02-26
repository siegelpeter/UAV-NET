using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Xml;
using UAVCommons.Datamodel;
namespace UAVCommons
{
    /// <summary>
    /// A Object for Uavdata which can consist if many UAVParameters
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(UAVStructureObjectConverter))]
    public class UAVStructure:UAVSingleParameter
    {
        public MonitoredDictionary<string, UAVSingleParameter> values;

        public delegate void ValueChangedHandler(UAVSingleParameter param, bool isremote);
        public event ValueChangedHandler ValueChanged;
		
		 public event ValueChangedHandler DataRecieved;


        public UAVStructure(string name, string port)
            : base(name, port)
        {
            this.Name = name;
            values = new MonitoredDictionary<string, UAVSingleParameter>(this);
           values.ValueChanged += new MonitoredDictionary<string, UAVSingleParameter>.ValueChangedHandler(gpsValues_ValueChanged);
        }

        protected void gpsValues_ValueChanged(UAVSingleParameter param, bool isremote)
        {
	
            if (ValueChanged != null) ValueChanged(param, isremote);
        }

        public override void SavetoXML(System.Xml.XmlElement parent, System.Xml.XmlDocument doc)
        {
		try{
			if ((parent != null)&&(doc != null)){
            base.SavetoXML(parent, doc);
			 XmlElement myitem = (XmlElement)parent.SelectSingleNode("Element[@Name='"+Name.Trim()+"']");
					if (myitem == null) {
						Console.WriteLine("myitem is null");

					}
				myitem.SetAttribute ("isStructure", "true");

			if ((values != null)&&(values.Values != null)){
            foreach (UAVSingleParameter subitem in values.Values)
            {
                subitem.SavetoXML(myitem,doc);
            }
			}
			}
			}catch (Exception ex){
				Console.WriteLine("Fehler beim Speichern von"+Name+ex.StackTrace.ToString());
			
			}

        }

        public override string Name
        {
            get
            {
               return base.Name;
            }
            set
            {
                base.Name = value;
            }

        }


        public UAVSingleParameter this[string key]
        {
            get
            {
                return values[key];
            }
            set
            {
                values[key] = value;
                if (ValueChanged != null) ValueChanged(value,false);
            }
        }




        public override object Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
                //values = (MonitoredDictionary<string, UAVSingleParameter>)value;
                //if (ValueChanged != null) {
                //    foreach (UAVSingleParameter myparam in values.Values)
                //    {
                //        ValueChanged(myparam,false);
                //}

                //}
                //}
            }
        }

        public void FireChangedEvent(bool isremote)
        {
            if (ValueChanged != null)
                ValueChanged(this, isremote);
        }
       
        //public void FireValueChangedEvent(UAVSingleParameter param, bool isremote)
        //{
        //    if (ValueChanged != null) ValueChanged(param, isremote);
        //}

		/// <summary>
		/// Fires the changed event. 
		/// </summary>
		/// <param name='param'>
		/// Parameter.
		/// </param>
		/// <param name='isremote'>
		/// Isremote.
		/// </param>
        public void FireDataRecievedEvent(UAVSingleParameter param, bool isremote)
        {
			if (DataRecieved != null) DataRecieved(param,isremote);			
		}
		
        #region Overrides 
		
        public override UAVSingleParameter SilentUpdate(string key, object value,bool isremote)
        {
            if (key == Name) {
                return base.SilentUpdate(key,value,isremote);
            }else{
            return values.SilentUpdate(key, value, isremote);
            }
        }
        #endregion

	
    }
}
