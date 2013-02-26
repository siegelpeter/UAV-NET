using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace UAVCommons.Datamodel
{
    public class DictionaryPropertyDescriptor : PropertyDescriptor
    {

        //PropertyDescriptor provides 3 constructors. We want the one that takes a string and an array of attributes:

        public MonitoredDictionary<string, UAVSingleParameter> _dictionary;
        public string _key;
        private UAVSingleParameter elem;

        public DictionaryPropertyDescriptor(MonitoredDictionary<string, UAVSingleParameter> d, string key)
            : base(key.ToString(), null)
        {
            _dictionary = d;
            _key = key;
            elem = _dictionary.GetFromPath(_key);
            if (elem == null) throw new Exception("Element not found Key:"+key);
        }

        //The attributes are used by PropertyGrid to organise the properties into categories, to display help text and so on. We don't bother with any of that at the moment, so we simply pass null.

        //The first interesting member is the PropertyType property. We just get the object out of the dictionary and ask it:

        public override string Description
        {
            get
            {
                return elem.GetType().Name;
            }
        }

        public override Type PropertyType
        {
            get
            {
                try
                {
         
                    if (elem is UAVStructure)
                    {
                        return elem.GetType();

                    }
                    else
                    {
                        return elem.Value.GetType();
                    }
                }
                catch (Exception ex)
                {
                    return typeof(String);
                }
            }

        }

        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }
        //If you knew that all of your values were strings, for example, you could just return typeof(string).

        //Then we implement SetValue and GetValue:

        public override void SetValue(object component, object value)
        {
            elem.Value = value;
        }

        public override object GetValue(object component)
        {

            return elem.Value;
        }

        public override string DisplayName
        {
            get
            {
                return elem.Name;    
            }
        }

        //The component parameter passed to these two methods is whatever value was returned from ICustomTypeDescriptor.GetPropertyOwner. If it weren't for the fact that we need the dictionary object in PropertyType, we could avoid using the _dictionary member, and just grab it using this mechanism.

        //And that's it for interesting things. The rest of the class looks like this:

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type ComponentType
        {
            get { return null; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
     
    }

}
