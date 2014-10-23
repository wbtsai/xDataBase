using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Context<T>
    {
        public bool success { get; set; }

        public object UserState { get; set; }

        //T ParseTo(Parser<T> parser);
    }
}
