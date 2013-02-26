using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
namespace UAVCommons.Datamodel
{
    public class UAVSingleParameterObjectConverter : ExpandableObjectConverter
    {

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            List<PropertyDescriptor> properties = new List<PropertyDescriptor>();

            PropertyDescriptorCollection props = new PropertyDescriptorCollection(properties.ToArray());

            return props;

        }
    }
}
