using System;
using System.Collections.Generic;
using System.Text;

namespace UAVCommons
{
    [Serializable]
    public abstract class HierachyItem
    {
        protected string stringPathCache = null;
        /// <summary>
        /// Name of the HierachyItem e.g. Wert1
        /// </summary>
        public abstract  string Name {
            get;
            set;
        }

        /// <summary>
        /// Parent of this HierachyItem
        /// </summary>
        public abstract  HierachyItem Parent
        {
        get;
        set;
        }

        public String GetStringPath() {  
            if( stringPathCache != null) return stringPathCache;
            string result = GetStringPath(String.Empty);
            if (result == String.Empty) {
                return "\\" + Name;
            }
            stringPathCache = result;
            return result;
        
        }


        public String GetStringPath(string parentstring)
        {
            if (Parent == null) return parentstring;
            string result = Parent.GetStringPath(parentstring);
            result +="\\" + Name;
            return result;
        }

        public static List<UAVSingleParameter> NormaliseUavData(UAVBase core)
        {
            List<UAVSingleParameter> uavData = new List<UAVSingleParameter>();

             foreach (UAVSingleParameter param in core.uavData.Values)
            {
                HierachyItem.NormaliseStructure(param, uavData);
            }
            return uavData;
        }

        public static void NormaliseStructure(UAVSingleParameter param, List<UAVSingleParameter> remotedata)
        {
            if (param is UAVStructure)
            {
                UAVStructure mystructure = ((UAVStructure)param);
                foreach (UAVSingleParameter myparams in mystructure.values.Values)
                {
                    if (myparams is UAVStructure)
                    {
                        NormaliseStructure(myparams, remotedata);
                        UAVSingleParameter temp = new UAVSingleParameter(myparams.GetStringPath(), myparams.Value);
                        temp.Max = myparams.Max;
                        temp.Min = myparams.Min;
                        temp.updateRate = myparams.updateRate;
                        temp.Value = myparams.Value;
                        remotedata.Add(temp);
                    }
                    else
                    {
                        UAVSingleParameter temp = new UAVSingleParameter(myparams.GetStringPath(), myparams.Value);
                        temp.Max = myparams.Max;
                        temp.Min = myparams.Min;
                       // temp.Value = myparams.Value;
                        temp.updateRate = myparams.updateRate;
                        remotedata.Add(temp);
                    }
                }
                //Eigen Wert

                UAVSingleParameter temp2 = new UAVSingleParameter(param.GetStringPath(), param.Value);
                temp2.Max = param.Max;
                temp2.Min = param.Min;
                // temp.Value = myparams.Value;
                temp2.updateRate = param.updateRate;
                remotedata.Add(temp2);

            }
            else
                if (param is UAVSingleParameter)
                {
                    UAVSingleParameter newparam = new UAVSingleParameter(param.Name, param.Value);
                    newparam.Name = param.GetStringPath();
                    newparam.Min = param.Min;
                    newparam.Max = param.Max;
                    //  if (!(param.Parent is UAVBase))
                    //   {
                    //      newparam.Parent = param.Parent;
                    //    }
                    newparam.updateRate = param.updateRate;
                    newparam.Value = param.Value;
                    remotedata.Add(newparam);
                }


        }
        
        
        public static HierachyItem GetItembyPath(UAVBase core,string path)
        {
             List<UAVParameter> uavData = new List<UAVParameter>();
            string key = path;
            string newpath = "";
            MonitoredDictionary<string, UAVParameter>.GetKeyPath(ref key, ref newpath);
            foreach (UAVParameter param in core.uavData.Values)
            {
                  if (param.Name == key){
                   if (newpath == key) return param;
                   if (param is UAVStructure) return GetItembyPath(param,newpath);
                  }
            
             
            }

            return null;
        }

        public static HierachyItem GetItembyPath(UAVParameter param,string path) {
            string key = path;
            string newpath = "";
            MonitoredDictionary<string, UAVParameter>.GetKeyPath(ref key, ref newpath);
          
            if (param is UAVStructure)
            {
                UAVStructure mystructure = ((UAVStructure)param);
                foreach (UAVParameter myparams in mystructure.values.Values)
                {
                    if (myparams.Name == key)
                    {
                        GetItembyPath(myparams, newpath);
                    }
                }
            }
            else
                if (param is UAVParameter)
                {
                    if (param.Name == key)
                    {
                        return param;
                    }
                 //   remotedata.Add(newparam);
                }

            return null;
        
        }

    }
}
