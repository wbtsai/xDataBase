using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;


namespace Yuanta.xDataBase
{
	public static class DataBaseExtension
	{
		public static List<T> ToMapping<T>(this DataTable dt)
		{
			return MapByRow<T> (dt).ToList ();
		}

		private static IEnumerable<T> MapByRow<T>(DataTable dt,Func<IMapBuilderContext<T>> RowMapping)
		{
			foreach (DataRow row in dt.rows) {

				yield return RowMapping ().MapRow (row);
			}
		}


		private static IEnumerable<T> MapByRow<T>(DataTable dt,Func<IMapBuilderContext<T>> RowMapping)
		{
			foreach (DataRow row in dt.rows) {
			
				yield return RowMapping ().MapRow (row);
			}
		}




	}
}

