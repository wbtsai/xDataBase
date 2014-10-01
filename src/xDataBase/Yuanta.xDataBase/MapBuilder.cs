using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Yuanta.xDataBase.RowMap;
using System.Collections;

namespace Yuanta.xDataBase
{
    public static class MapBuilder<TResult> where TResult:new()
    {
		public static IMapBuilderContext<TResult> Build()
        {
            IMapBuilderContext<TResult> context = new MapBuilderContext();

            var properties =
                from property in typeof(TResult).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where IsAutoMappableProperty(property)
                select property;

            foreach (var property in properties)
            {
                context = context.Map(property);
            }
            return context;
        }

        private static bool IsAutoMappableProperty(PropertyInfo property)
        {
            return property.CanWrite
              && property.GetIndexParameters().Length == 0
              && !IsCollectionType(property.PropertyType)
            ;
        }

        private static bool IsCollectionType(Type type)
        {
             if (type == typeof(string)) return false;

            var interfaces = from inf in type.GetInterfaces()
                             where inf == typeof(IEnumerable) ||
                                 (inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                             select inf;
            return interfaces.Count() != 0;
        }
        
		private class MapBuilderContext:IMapBuilderContext<TResult>
        {
			public Dictionary<PropertyInfo, PropertyMapping> mapping;

			private Func<IMapBuilderContext<TResult>> mapper;

            public MapBuilderContext()
            {
                this.mapping = new Dictionary<PropertyInfo, PropertyMapping>(); 
            }



            internal MapBuilderContext<TResult> Map(PropertyInfo property)
            {
                this.mapping.Add(property, new PropertyMapping(property));

                return this;
            }

            public TResult Build()
            {
                return default(TResult);
            }

			public TResult MapRow (System.Data.DataTable dt)
			{
				return default(TResult);
			}

			public void Map(PropertyInfo info)
			{
				mapping.Add (info, new PropertyMapping (info));
			}
        }
	}
}
