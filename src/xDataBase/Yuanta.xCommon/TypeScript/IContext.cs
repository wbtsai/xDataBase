using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public interface IContext<T>
    {
        bool success { get; set; }

        object UserState { get; set; }

        T ParseTo(Parser<T> parser);
    }
}
