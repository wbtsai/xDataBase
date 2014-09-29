using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using Yuanta.xDataBase.RowMap;

namespace Yuanta.xDataBase.RowMap
{
    public class PropertyMapping
    {
        public PropertyMapping(PropertyInfo propertyInfo)
        {
            this.Property = propertyInfo;
            //確定是否有設ColumnName
            ColumnNameAttribute[] attrs = (ColumnNameAttribute[])this.Property.GetCustomAttributes(typeof(ColumnNameAttribute));
            if (attrs == null)
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
            return row[this.Name];
        }

        public void Map(object instance, DataRow row)
        {
            object convertedValue = GetPropertyValue(row);

            SetValue(instance, convertedValue);
        }

        protected void SetValue(object instance, object value)
        {
            Property.SetValue(instance, value, new object[0]);
        }
    }
}
