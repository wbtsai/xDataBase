using System;
using System.Data;

namespace Yuanta.xDataBase
{
	public interface IMapBuilderContext<TResult>
	{
		TResult MapRow(DataTable dt);
	}
}

