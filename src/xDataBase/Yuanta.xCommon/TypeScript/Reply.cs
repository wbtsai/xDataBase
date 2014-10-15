using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Reply<A,U>
    {
        public State<U> State { get; set; }
        public bool Success { get; set; }
        public A Value { get; set; }

        public Func<string> Expected { get; set; }

        public Reply(State<U> state, bool success, A values, Func<string> expected)
        {
            this.State = state;
            this.Success = success;
            this.Value = values;
            this.Expected = expected;
        }
        
    }
}
