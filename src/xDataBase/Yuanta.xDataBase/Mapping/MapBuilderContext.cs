using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Collections;
using System.Linq.Expressions;

namespace Yuanta.xDataBase.Mapping
{
    public class MapBuilderContext<TResult>
    {
        private Dictionary<PropertyInfo, PropertyMapping> propertyMapping;

        private Func<DataRow, TResult> mapper;

        public MapBuilderContext()
        {
            propertyMapping = new Dictionary<PropertyInfo, PropertyMapping>();

            var properties =
                from property in typeof(TResult).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where IsAutoMappableProperty(property)
                select property;
            
            foreach (var property in properties)
            {
                propertyMapping.Add(property, new PropertyMapping(property));
            }

            //Get Reflection Structure
            ReflectionRowMapper();
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
            // string implements IEnumerable, but for our purposes we don't consider it a collection.
            if (type == typeof(string)) return false;

            var interfaces = from inf in type.GetInterfaces()
                             where inf == typeof(IEnumerable) ||
                                 (inf.IsGenericType && inf.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                             select inf;
            return interfaces.Count() != 0;
        }
        
        private void ReflectionRowMapper()
        {
            if (propertyMapping == null) throw new ArgumentNullException("propertyMappings");

                var parameter = Expression.Parameter(typeof(DataRow), "dr");
                var newExpr = Expression.New(typeof(TResult));
                var ConvertValue =GetMethodInfo<PropertyMapping>(pm => pm.GetPropertyValue(null));
                var bindings =
                    propertyMapping.Select(kvp => (MemberBinding)
                        Expression.Bind(
                            kvp.Key,
                            Expression.Convert(
                                Expression.Call(Expression.Constant(kvp.Value), ConvertValue, new Expression[] { parameter }),
                                kvp.Key.PropertyType)));

                var body = Expression.MemberInit(newExpr, bindings);
                var lambda = Expression.Lambda<Func<DataRow,TResult>>(body, parameter);
                this.mapper = lambda.Compile();
            
        }
                

        private MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfo((LambdaExpression)expression);
        }

        private MethodInfo GetMethodInfo(LambdaExpression lambda)
        {
            var call = (MethodCallExpression)lambda.Body;
            return call.Method;
        }

        public TResult MapRow(DataRow row)
        {
            return mapper(row);
        }

        public void GetPameters(Dictionary<string,object> dic, TResult obj)
        {
            foreach (PropertyMapping p in propertyMapping.Values)
            {
                dic.Add(p.Name, p.Property.GetValue(obj));
            }
        }
    }
}
