using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.ComponentModel;
namespace UAVCommons.Datamodel
{
    public class UAVStructureObjectConverter : ExpandableObjectConverter
    {
        public override bool IsValid(ITypeDescriptorContext context, object value)
        {
            return base.IsValid(context, value);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();
            if (!(context.Instance is DictionaryPropertyGridAdapter))
            {
                foreach (KeyValuePair<string, UAVSingleParameter> keyval in ((UAVStructure)((MonitoredDictionary<string, UAVSingleParameter>)((DictionaryPropertyDescriptor)context.PropertyDescriptor)._dictionary).GetFromPath(context.PropertyDescriptor.Name)).values)
                {
                 //   if (((DictionaryPropertyGridAdapter)context.Instance).selectedProperties.Contains(keyval.Value.GetStringPath()))
                    {
                        properties.Add(new DictionaryPropertyDescriptor((((MonitoredDictionary<string, UAVSingleParameter>)((DictionaryPropertyDescriptor)context.PropertyDescriptor)._dictionary)), keyval.Value.GetStringPath()));
                    }

                }
            }else{
            foreach (KeyValuePair<string, UAVSingleParameter> keyval in ((UAVStructure)((DictionaryPropertyGridAdapter)context.Instance)._dictionary.GetFromPath(context.PropertyDescriptor.Name)).values)
            {
                if (((DictionaryPropertyGridAdapter)context.Instance).selectedProperties.Contains(keyval.Value.GetStringPath()))
                {
                    properties.Add(new DictionaryPropertyDescriptor((((DictionaryPropertyGridAdapter)context.Instance)._dictionary), keyval.Value.GetStringPath()));
                }
            }
            }
            PropertyDescriptorCollection props = new PropertyDescriptorCollection(properties.ToArray());

            return props;

        }

        TypeConverter actualConverter = null;

        private void InitConverter(ITypeDescriptorContext context)
        {
            //if (actualConverter == null)
            //{
            //    ((UAVStructure)((MonitoredDictionary<string, UAVSingleParameter>)((DictionaryPropertyDescriptor)context.PropertyDescriptor)._dictionary).GetFromPath(context.PropertyDescriptor.Name))
            //    TypeConverter parentConverter = TypeDescriptor.GetConverter(context.Instance);
            //    PropertyDescriptorCollection coll = parentConverter.GetProperties(context.Instance);
            //    PropertyDescriptor pd = coll[context.PropertyDescriptor.Name];

            //    if (pd.PropertyType == typeof(object))
            //        actualConverter = TypeDescriptor.GetConverter(pd.GetValue(context.Instance));
            //    else
            //        actualConverter = this;
            //}
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
  //          InitConverter(context);
            return true;
            return actualConverter.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
         //   InitConverter(context); // I guess it is not needed here
            double val = 0;
            if (Double.TryParse(value.ToString(), out val))
            {
                return val;
            }
            else
            {
                return "";
            }

        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value.ToString();

        }
    }
}
