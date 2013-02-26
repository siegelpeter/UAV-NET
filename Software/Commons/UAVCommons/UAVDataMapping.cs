using System;
using System.Collections.Generic;

using System.Text;
using System.Xml;

namespace UAVCommons
{
    /// <summary>
    /// Bietet Umsetzung von UAVParameter Namen in andere an 
    /// </summary>
    [Serializable]
    public class UAVDataMapping : UAVStructure
    {
        private Dictionary<string, string> mapping = new Dictionary<string, string>();
        public UAVStructure Source = null;
        public object Syncobj = new object();
        private MonitoredDictionary<string, UAVParameter> uavData;

        public UAVDataMapping(string name, string port, UAVStructure source)
            : base(name, port)
        {
            this.Source = source;
            Source.ValueChanged += new ValueChangedHandler(Source_ValueChanged);
            Dictionary<string, string> emptymapping = new Dictionary<string, string>();
            foreach (string key in Source.values.Keys) {
                mapping.Add(key, key);
            }
         
        }

        public UAVDataMapping(string p, string p_2, MonitoredDictionary<string, UAVSingleParameter> uavData):base(p,p_2)
        {
            uavData.ValueChanged += new MonitoredDictionary<string, UAVSingleParameter>.ValueChangedHandler(Source_ValueChanged);
            
        }

        public Dictionary<string, string> Mapping
        {
            get
            {
                return mapping;
            }
            set
            {
                lock (Syncobj)
                {
                    this.mapping = value;
                    values.Clear();
                    foreach (string key in mapping.Values)
                    {
                        if (!values.ContainsKey(key)) {
                            values.Add(new UAVParameter(key, 0,-100,100));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sieht nach welcher Parameter in wie abgelegt werden soll
        /// </summary>
        /// <param name="param"></param>
        /// <param name="isremote"></param>
        void Source_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            if (param.GetStringPath() == GetStringPath()) return;
            lock (Syncobj)
            {
                
           //     Console.WriteLine(param.Name + " " + param.Value);
                if (mapping.ContainsKey(param.Name))
                {
                    if (values.ContainsKey(mapping[param.Name])){
                        if (values[mapping[param.Name]].GetStringPath() != param.GetStringPath()) { 
                        values[mapping[param.Name]].Value = param.Value;
                        }
                    }else{
                        
                        values.Add(mapping[param.Name],new UAVParameter(mapping[param.Name],param.Value,param.Min, param.Max));
                        values[mapping[param.Name]].updateRate = 40;
                    }
                }
            }
        }

        public override void SavetoXML(System.Xml.XmlElement parent, System.Xml.XmlDocument doc)
        {
             
		try{
			if ((parent != null)&&(doc != null)){
            base.SavetoXML(parent, doc);
			 XmlElement myitem = (XmlElement)parent.SelectSingleNode("Element[@Name='"+Name.Trim()+"']");
             myitem.SetAttribute("source", Source.GetStringPath());
             
             lock (Syncobj)
             {
                 foreach (KeyValuePair<string, string> pair in mapping)
                 {
                     XmlElement pairelem = doc.CreateElement("mapping");
                     pairelem.SetAttribute("key", pair.Key);
                     pairelem.SetAttribute("value", pair.Value);
                     myitem.AppendChild(pairelem);
                 }
             }
			
            }
			}catch (Exception ex){
				Console.WriteLine("Fehler beim Speichern von"+Name+ex.StackTrace.ToString());
			
			}
        }

        public override void LoadValues(System.Xml.XmlElement elem)
        {
            base.LoadValues(elem);
            var mappingelements = elem.SelectNodes("mapping");
            Source = (UAVStructure)((UAVBase)this.Parent).uavData.GetFromPath(elem.GetAttribute("source"));
            Source.ValueChanged += new ValueChangedHandler(Source_ValueChanged);
          

            lock (Syncobj)
            {
                var mapping = Mapping;
                if (mappingelements.Count > 0)
                {
                    mapping.Clear();
                    foreach (XmlElement item in mappingelements)
                    {
                        mapping.Add(item.GetAttribute("key"), item.GetAttribute("value"));
                    }
                    Mapping = mapping;
                }
            }
        }
    }
}
