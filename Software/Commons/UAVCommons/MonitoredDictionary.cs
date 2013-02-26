using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAVCommons
{
    [Serializable]
    public class MonitoredDictionary<T1, T2> : IDictionary<string, UAVSingleParameter>
    {
        public delegate void ValueChangedHandler(UAVSingleParameter param, bool isremote);
        public event ValueChangedHandler ValueChanged;

        public delegate void NewValueHandler(UAVSingleParameter param, bool isremote);
        public event NewValueHandler NewValue;
        public bool AddOnSet = false;

        private SortedDictionary<string, UAVSingleParameter> uavData = new SortedDictionary<string, UAVSingleParameter>(); // known Data about the UAV ( attitude speed track ...)
        public object SyncObj = new object();
        public HierachyItem owner;


        public MonitoredDictionary(HierachyItem owner)
        {
            this.owner = owner;
        }


        public void Add(UAVSingleParameter value)
        {
            value.Parent = owner;
		   Add(value.Name, value);

        }


        public void Add(string key, UAVSingleParameter value)
        {
            lock (SyncObj)
            {
                if (uavData.ContainsKey(key))
                {
                    uavData[key].Min = value.Min;
                    uavData[key].Max = value.Max;
                    uavData[key].updateRate = value.updateRate;
                    uavData[key].Value = value.Value;
                }
                else
                {
                    value.Parent = owner;
                    uavData.Add(key, value);
                    if (value is UAVStructure)
                    {
                        ((UAVStructure)(value)).ValueChanged += new UAVStructure.ValueChangedHandler(MonitoredDictionary_ValueChanged);
                      //  value.ValueChanged += new UAVSingleParameter.ValueChangedHandler(Value_ValueChanged);
                    }
                    else
                    {
                        value.ValueChanged += new UAVSingleParameter.ValueChangedHandler(Value_ValueChanged);
                    }
                    if (ValueChanged != null) ValueChanged(value, false);
                    if (NewValue != null) NewValue(value, false);
                }
            }

        }

        void MonitoredDictionary_ValueChanged(UAVSingleParameter param, bool isremote)
        {
		    if (ValueChanged != null) ValueChanged(param, isremote);
        }

   
        public bool ContainsKey(string key)
        {
            lock (SyncObj)
            {
                return uavData.ContainsKey(key);
            }
        }

        public ICollection<string> Keys
        {
            get { return uavData.Keys; }
        }

        public bool Remove(string key)
        {
            lock (SyncObj)
            {

                return uavData.Remove(key);
            }
        }

        public bool TryGetValue(string key, out UAVSingleParameter value)
        {

            return uavData.TryGetValue(key, out value);
        }

        public ICollection<UAVSingleParameter> Values
        {
            get { return uavData.Values; }
        }

        public UAVSingleParameter this[string key]
        {
            get
            {
                return uavData[key];
            }
            set
            {

                lock (SyncObj)
                {
               
                    try{
                    if (uavData.ContainsKey(key))
                    {
                        if (value is UAVStructure)
                        {
                            ((UAVStructure)(value)).ValueChanged += new UAVStructure.ValueChangedHandler(MonitoredDictionary_ValueChanged);
                        }
                        else
                        {
                            value.ValueChanged += new UAVSingleParameter.ValueChangedHandler(Value_ValueChanged);
                        }
                        uavData[key] = value;
                    }
                    else
                    {
                        UAVParameter param = new UAVParameter(key, value);
                        param.Min = null;
                        param.Max = null;
                        uavData.Add(key, value);
                    }
                    if (ValueChanged != null) ValueChanged(value, false);
                }catch (Exception ex)
                {
                    ;
                    throw new Exception("Error on key: " + key,ex);
                }
                }
            }
        }

        public void Add(KeyValuePair<string, UAVSingleParameter> item)
        {
            lock (SyncObj)
            {
                if (item.Value is UAVStructure)
                {
                    ((UAVStructure)(item.Value)).ValueChanged += new UAVStructure.ValueChangedHandler(MonitoredDictionary_ValueChanged);
                }
                else
                {
                    item.Value.ValueChanged += new UAVSingleParameter.ValueChangedHandler(Value_ValueChanged);
                    if (ValueChanged != null) ValueChanged(item.Value, false);
                }
            }
        }



        public void Value_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            if (ValueChanged != null) ValueChanged(param, isremote);
        }


        public void Clear()
        {
            lock (SyncObj)
            {
                uavData.Clear();
            }
        }

        public bool Contains(KeyValuePair<string, UAVSingleParameter> item)
        {
            lock (SyncObj)
            {
                return uavData.ContainsKey(item.Key);
            }
        }

        public void CopyTo(KeyValuePair<string, UAVSingleParameter>[] array, int arrayIndex)
        {
            return;
        }

        public int Count
        {

            get
            {
                return uavData.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, UAVSingleParameter> item)
        {
            lock (SyncObj)
            {
                return uavData.Remove(item.Key);
            }
        }

        public IEnumerator<KeyValuePair<string, UAVSingleParameter>> GetEnumerator()
        {
            return uavData.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return uavData.GetEnumerator();
        }

        public UAVSingleParameter SilentUpdate(string key, object value, bool isremote)
        {

            string newpath = "";
            GetKeyPath(ref key, ref newpath);

            if (uavData.ContainsKey(key))
            {
                // Cut away current level
                return uavData[key].SilentUpdate(newpath, value,isremote);
            }
            else
            {
                UAVSingleParameter parameter;
                if (newpath == key)
                { 
                    if (owner is UAVParameter)
                    {
                        parameter = new UAVSingleParameter(key, value);
                    }
                    else
                    {
                        if ((key == "Min") || (key == "Max") || (key == "UpdateRate"))
                        {
                            parameter = new UAVSingleParameter(key, value);
                        }
                        else
                        {
                            if (owner.Name != key)
                            {
                                parameter = new UAVParameter(key, value);
                            }
                            else {
                                
                                return (UAVSingleParameter)owner;
                            }
                            
                        }
                    }

                }
                else
                {
                    parameter = new UAVStructure(key, "");
                     parameter.Parent = owner;
                     parameter.ValueChanged += new UAVSingleParameter.ValueChangedHandler(parameter_ValueChanged);

                    uavData.Add(key, parameter);
                    UAVSingleParameter child = parameter.SilentUpdate(newpath, value, isremote);
                    child.Parent = parameter;
                    child.ValueChanged += new UAVSingleParameter.ValueChangedHandler(parameter_ValueChanged);
                    return child;
                }
                parameter.ValueChanged += new UAVSingleParameter.ValueChangedHandler(parameter_ValueChanged);

                parameter.Parent = owner;
                uavData.Add(key, parameter);
                if (NewValue != null) NewValue(parameter,isremote);
                return parameter;
            }


        }


        public static List<UAVSingleParameter> NormaliseDictionary(IDictionary<string, UAVSingleParameter> uavdata)
        {
            List<UAVSingleParameter> uavData = new List<UAVSingleParameter>();

            foreach (UAVSingleParameter param in uavdata.Values)
            {
                NormaliseStructure(param, uavData);
            }
            return uavData;
        }


        public static List<UAVSingleParameter> NormaliseDictionary(MonitoredDictionary<T1, T2> uavdata)
        {
            List<UAVSingleParameter> uavData = new List<UAVSingleParameter>();

            foreach (UAVSingleParameter param in uavdata.Values)
            {
                NormaliseStructure(param, uavData);
            }
            return uavData;
        }

        private static void NormaliseStructure(UAVSingleParameter param, List<UAVSingleParameter> remotedata)
        {
            if (param is UAVStructure)
            {
                UAVStructure mystructure = ((UAVStructure)param);
                foreach (UAVSingleParameter myparams in mystructure.values.Values)
                {
                   
                        NormaliseStructure(myparams, remotedata);
                        UAVSingleParameter structparam = new UAVSingleParameter( param.GetStringPath(), myparams.Value, myparams.Min, myparams.Max);
                        if (remotedata.Count(m=>m.Name.Equals(structparam.Name))==0) remotedata.Add(structparam);
                   
                }
               // UAVSingleParameter param1 = new UAVSingleParameter(param.Name, param.Value, param.Min, param.Max);
               // remotedata.Add(param1);
          
                }
            else
                if (param is UAVSingleParameter)
                {
                    UAVSingleParameter newparam = new UAVSingleParameter(param.Name, param.Value);
                    newparam.Name = param.GetStringPath();
                    newparam.Min = param.Min;
                    newparam.Max = param.Max;
                    newparam.Parent = param.Parent;
                    newparam.updateRate = param.updateRate;
                    newparam.Value = param.Value;
                    if (remotedata.Count(m => m.Name.Equals(newparam.Name)) == 0) remotedata.Add(newparam);
                }


        }

        public static void GetKeyPath(ref string key, ref string newpath)
        {
            if (key.Contains("\\"))
            {

                if (key.StartsWith("\\")) key = key.Substring(1);
                if (!(key.IndexOf("\\") == -1))
                {

                    newpath = key.Substring(key.IndexOf("\\") + 1);

                    key = key.Substring(0, key.IndexOf("\\"));
                }
                else
                {
                    key = key;
                }
            }
            if (newpath == String.Empty) newpath = key;
        }


        public UAVSingleParameter GetFromPath(string key)
        {

            string newpath = "";
            GetKeyPath(ref key, ref newpath);
            if (key == String.Empty) return null;
            if ((uavData.ContainsKey(key)) && (key == newpath))
            {
                return uavData[key];

            }
            else if (uavData.ContainsKey(key))
            {
                // Cut away current level
                if (uavData[key] is UAVStructure)
                {
                    return ((UAVStructure)uavData[key]).values.GetFromPath(newpath);
                }


            }
            else { 
            // not found return null;
            }
            return null;



        }



        void parameter_ValueChanged(UAVSingleParameter param, bool isremote)
        {
            Value_ValueChanged(param, isremote);
        }
    }
}
