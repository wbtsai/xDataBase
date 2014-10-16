using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public interface IContext<A,U>
    {
        bool success { get; set; }

        U UserState { get; set; }

        Func<Parser<A, U>, U> Parse { get; set; }
    }
}
