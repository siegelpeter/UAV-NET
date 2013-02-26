using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Text;
using System.Collections;
using UAVCommons;
using UAVCommons.Datamodel;

namespace UAVCommons.Datamodel
{
    public class DictionaryPropertyGridAdapter : ICustomTypeDescriptor
    {
        public MonitoredDictionary<string, UAVSingleParameter> _dictionary;
        public List<string> selectedProperties;

        public DictionaryPropertyGridAdapter(MonitoredDictionary<string, UAVSingleParameter> d)
        {
            _dictionary = d;
        }

        public DictionaryPropertyGridAdapter(MonitoredDictionary<string, UAVSingleParameter> monitoredDictionary, List<string> selectedProperties)
        {
            // TODO: Complete member initialization
            this._dictionary = monitoredDictionary;
            this.selectedProperties = selectedProperties;
        }

        //Three of the ICustomTypeDescriptor methods are never called by the property grid, but we'll stub them out properly anyway:

        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        //Then there's a whole slew of methods that are called by PropertyGrid, but we don't need to do anything interesting in them:

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return _dictionary;
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }

        PropertyDescriptorCollection
            System.ComponentModel.ICustomTypeDescriptor.GetProperties()
        {
            ArrayList properties = new ArrayList();
            foreach (KeyValuePair<string, UAVSingleParameter> e in _dictionary)
            {
                bool usethis = true;
                if (this.selectedProperties != null)
                {
                    if (!this.selectedProperties.Contains(e.Key)) usethis = false;
                }

                if (usethis)
                {
                    DictionaryPropertyDescriptor descriptor = new DictionaryPropertyDescriptor(_dictionary, e.Value.GetStringPath());


                    properties.Add(descriptor);
                }
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
           // return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }


        private bool SelectedContains(string key) {
            if (key == "") return false;
            foreach (string s in selectedProperties){
                if (s.Contains(key)) return true; 
            }
            return false;
        }


        //Then the interesting bit. We simply iterate over the IDictionary, creating a property descriptor for each entry:

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            ArrayList properties = new ArrayList();
            foreach (KeyValuePair<string, UAVSingleParameter> e in _dictionary)
            {
                bool usethis = true;
                if (this.selectedProperties != null)
                {
                    if (SelectedContains(e.Key))
                    {

                        DictionaryPropertyDescriptor descriptor = new DictionaryPropertyDescriptor(_dictionary, e.Value.GetStringPath());


                        properties.Add(descriptor);
                    }
                }
            }

            PropertyDescriptor[] props =
                (PropertyDescriptor[])properties.ToArray(typeof(PropertyDescriptor));

            return new PropertyDescriptorCollection(props);
        }

        
                 
        
        }

    }
