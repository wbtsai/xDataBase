using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Parser
    {
        public Reply<A,U> Parse<A,U>(Parser<A,U> parser,State<U> state )
        {
            return parser.RunParser(state);
        }

        public Parser<A, U> Choice<A,U>(List<Parser<A, U>> ps)
        {
            Func<State<U>, Reply<A, U>> choiceParser = state =>
            {
               List<Reply<A,U>> sts =new List<Reply<A,U>>();
                
                foreach(var s in ps)
                {
                    Reply<A,U> st=this.Parse(s,state);

                    if(st.Success||st.State.Position!=state.Position)
                    {
                        return st;
                    }

                    sts.Add(st);
                }

                return Error(state, () => { return "one of string error"; });
                
            };

            return new Parser<A, U>(choiceParser);
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
       public Func<State<U>, Reply<A, U>> RunParser{get;set;}
       public Parser(Func<State<U>, Reply<A, U>> runparser)
        {
            this.RunParser = runparser;
        }
       
    }
}
