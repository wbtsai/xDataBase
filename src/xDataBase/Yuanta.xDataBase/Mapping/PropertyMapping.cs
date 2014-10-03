using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using Yuanta.xDataBase.Mapping;
using System.ComponentModel;
using System.Globalization;

namespace Yuanta.xDataBase.Mapping
{
    public class PropertyMapping
    {
        public PropertyMapping(PropertyInfo propertyInfo)
        {
            this.Property = propertyInfo;
            //確定是否有設ColumnName
            ColumnNameAttribute[] attrs = (ColumnNameAttribute[])this.Property.GetCustomAttributes(typeof(ColumnNameAttribute));
            if (attrs.Length==0)
            {
                this.Name = this.Property.Name;
            }
            else 
            {
                this.Name = attrs[0].ColumnName;
            }
            
        }
        public PropertyInfo Property { get; private set; }

        public string Name { get; private set; }

        public object GetPropertyValue(DataRow row)
        {
            if (row == null) throw new ArgumentNullException("row");

            object value;
            
            value = row[this.Name];
           
            object convertedValue;
            
            convertedValue = ConvertValue(value, Property.PropertyType);
            
            return convertedValue;
        }

        private object ConvertValue(object value, Type conversionType)
        {
            if (IsNullableType(conversionType))
            {
                return ConvertNullableValue(value, conversionType);
            }
            return ConvertNonNullableValue(value, conversionType);
        }

        private  bool IsNullableType(Type t)
        {
            return t.IsGenericType &&
                   t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private object ConvertNullableValue(object value, Type conversionType)
        {
            if (value != DBNull.Value)
            {
                var converter = new NullableConverter(conversionType);
                return converter.ConvertFrom(value);
            }
            return null;
        }
                
        private static object ConvertNonNullableValue(object value, Type conversionType)
        {
            object convertedValue = null;

            if (value != DBNull.Value)
            {
                convertedValue = Convert.ChangeType(value, conversionType, CultureInfo.CurrentCulture);
            }
            else if (conversionType.IsValueType)
            {
                convertedValue = Activator.CreateInstance(conversionType);
            }

            return convertedValue;
        }

    }
}
