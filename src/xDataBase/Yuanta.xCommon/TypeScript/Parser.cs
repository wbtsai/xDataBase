using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Parser
    {
        public Reply<A> Parse<A>(Parser<A> parser, State state)
        {
            return parser.RunParser(state);
        }
        public State State(string source, object userState, int position = 0)
        {
            return new State(source, position, userState);
        }

        public Reply<A> OK<A>(State state, A values)
        {
            return new Reply<A>(state, true, values, null);
        }

        public Reply<A> Error<A>(State state, Func<string> expected)
        {
            return new Reply<A>(state, false, default(A), expected);
        }
    }

    public class Parser<A>
    {
        public Func<State, Reply<A>> RunParser { get; set; }

        public Parser(Func<State, Reply<A>> runParser)
        {
            this.RunParser = runParser;
        }
    }
}
