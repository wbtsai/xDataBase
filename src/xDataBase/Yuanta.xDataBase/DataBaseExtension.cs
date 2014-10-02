using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Yuanta.xDataBase.RowMap;

namespace Yuanta.xDataBase
{
	public static class DataBaseExtension
	{              
        public static List<T> ToMapping<T>(this DataTable dt)
		{
			return MapByRow<T>(dt).ToList();
		}

		private static IEnumerable<T> MapByRow<T>(DataTable dt)
		{
            var context = new MapBuilderContext<T>();
            
			foreach (DataRow row in dt.Rows) {
			
				yield return context.MapRow(row);
			}
		}

	}
}

