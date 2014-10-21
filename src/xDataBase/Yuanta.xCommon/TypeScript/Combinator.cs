using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Combinator
    {
        public Parser<A> Choice<A>(List<Parser<A>> ps)
        {
            Func<State, Reply<A>> choiceParser = state =>
            {
                List<Reply<A>> sts = new List<Reply<A>>();
                
                Parser pa = new Parser();

                foreach (var s in ps)
                {
                    Reply<A> st =new Parser().Parse(s, state);

                    if (st.Success || st.State.Position != state.Position)
                    {
                        return st;
                    }

                    sts.Add(st);
                }

                return pa.Error<A>(state, () => { return "one of string error"; });

            };
            
            return new Parser<A>(choiceParser);
        }
    }
}
