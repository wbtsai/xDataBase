using System;
using System.Data;

namespace Yuanta.xDataBase
{
	public static class DataBaseExtension
	{
        public static TResult ConvertToClass<TResult>(this DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                yield return MapBuilder<TResult>.MapAll().Build();
            }
        }
	}
}

