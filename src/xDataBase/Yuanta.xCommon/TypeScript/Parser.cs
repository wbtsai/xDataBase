using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Parser
    {


        public Parser()
        { 
        
        }

        public State<U> State<U>(string source, U userState, int position = 0)
        {
            return new State<U>(source, position, userState);
        }

        public Reply<A, U> OK<A, U>(State<U> state, A values)
        {
            return new Reply<A, U>(state, true, values, null);
        }

        public Reply<A, U> Error<A, U>(State<U> state, Func<string> expected)
        {
            return new Reply<A, U>(state, false, default(A), expected);
        }
    }

    public class Parser<A,U>
    {
        public Parser(Func<State<U>, Reply<A, U>> runparser)
        {
            this.RunParser = runparser;
        }
        public Func<State<U>, Reply<A, U>> RunParser{ get; set; }
    }
}
