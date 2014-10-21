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

        public Parser<List<T>> Repeat<T>(int min,int max,Parser<T> parser)
        {
            Func<State,List<T>> repeatParser=state=>{
                List<T> xs=new List<T>();
                
                Reply<T> st=new Parser().OK<T>(state,default(T));

                for(int i=0;i<max;i++)
                {
                    var st1=new Parser().Parse<T>(parser,st.State);

                    if(st1.Success)
                    {
                        if(st1.State.Position == st.State.Position && max ==int.MaxValue)
                        {
                            throw new Exception("many combinator is applied to a parser that accepts an empty string.");
                        }
                        else
                        {
                            st=st1;
                            xs.Add(st.Value);
                        }
                    }
                    else if(st.State.Position < st1.State.Position)
                    {
                        return st1;
                    }else if(i < min){
                        return st1;
                    }else{
                        break;
                    }
                }

                return new Parser().OK<T>(st.State, xs);
            };
            return new Parser<T>(repeatParser);
        }
        
        
    }
    }
}
