using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public static class ParserCT
    {
        public static State NewState(string source, int position = 0, object userState = null)
        {
            return NewState(source, position, userState);
        }

        public static Reply<A> OK<A>(State state,A Value)
        {
            return new Reply<A>(state, true, Value, null);
        }

        public static Reply<A> Error<A>(State state,Func<string> expected)
        {
            return new Reply<A>(state,false,default(A),expected );
        }

        public static Reply<A> Parse<A>(Parser<A> parser,State state)
        {
            return parser.RunParser(state);
        }

        public static Parser<T> Choice<T>(Parser<T>[] ps)
        {
            Func<State, Reply<T>> choiceParser = (state) => {
                
                var sts = new List<Reply<T>>();

                foreach (Parser<T> p in ps)
                {
                    var st = Parse<T>(p, state);

                    if(st.Success||st.State.Position!=state.Position)
                    {
                        return st;
                    }
                    sts.Add(st);
                }
                return Error<T>(state, () => "one of"+sts.Select(s=>s.Expected()).DefaultIfEmpty().Aggregate((current,next)=>current+","+next));
            };

            return new Parser<T>(choiceParser);
            
        }

        public Parser<T> Or<T>(params Parser<T>[] parsers)
        {
            return Choice<T>(parsers);
        }

        public static Parser<T[]> Repeat<T>(int min, int max, Parser<T> p)
        { 
            Func<State,Reply<T[]>> repeatParser=(state)=>{
                var xs = new List<T>();
                var st = OK<T>(state, default(T));

                for (int i = 0; i < max; i++)
                {
                    var _st = Parse<T>(p, st.State);

                    if (_st.Success)
                    {
                        if (_st.State.Position == st.State.Position && max == Int16.MaxValue)
                        {
                            throw new Exception("many combinator is applied to a parser that accepts an empty string.");
                        }
                        else
                        {
                            st = _st;
                            xs.Add(st.Value);
                        }
                    }
                    else if (st.State.Position < _st.State.Position)
                    {
                        st = _st;
                    }
                    else if (i < min)
                    {
                        st = _st;
                    }
                    else
                    {
                        break;
                    }
                }
                return OK<T[]>(st.State, xs.ToArray());
            };

            return new Parser<T[]>(repeatParser);
        }

        public static Parser<T[]> Count<T>(int n,Parser<T> p)
        {
            return Repeat<T>(n, n, p);
        }

        public static Parser<T[]> Many<T>(Parser<T> p)
        {
            return Repeat<T>(0, int.MaxValue, p);
        }

        public static Parser<T[]> Many1<T>(Parser<T> p)
        {
            return Repeat<T>(1, int.MaxValue, p);
        }

        public static Parser<T[]> Array<T>(Parser<T>[] ps)
        {
            Func<State, Reply<T[]>> arrayParser = (state) =>
            {
                var values=new List<T>();

                var st = OK<T>(state,default(T));

                for(var i = 0; i < ps.Length; i++){
                    
                    st =Parse<T>(ps[i], st.State);

                    if (!st.Success)
                    {
                        return Error<T[]>(st.State,st.Expected);
                    }
                        
                    values.Add(st.Value);
                }
                
                return OK<T[]>(st.State,values.ToArray());
            };

            return new Parser<T[]>(arrayParser);
        }

        public static Parser<T[]> Series<T>(params Parser<T>[] ps)
        {
            return Array<T>(ps);
        }

        public static Parser<T> Head<T>(Parser<T> p, params Parser<T>[] ps)
        {
            Func<State, Reply<T>> headParser = (state) =>
            {

                var st = Parse<T>(p,state);
                var Value= st.Value;
                for(var i = 0; i < ps.Length && st.Success; i++)
                {
                    st = Parse<T>(ps[i], st.State);
                }
                return st.Success ? OK<T>(st.State,Value) : st;
            };

            return new Parser<T>(headParser);
        }

        public static Parser<T> Seq<T>(Func<Context<T>,T> f)
        {
            Func<State, Reply<T>> seqParser = (state) =>
            {

               var st = OK<T>(state, default(T));
            
                Func<Parser<T>,T> contextFunction=p=> {
                    if(st.Success)
                    {
                        st =Parse<T>(p, st.State);
                        context.success = st.success;
                        return st.value; 
                    }                
                };
            var context: Context<U> = <Context<U>> contextFunction;
            context.success = true;
            context.userState = st.state._userState;
            var value: A = f(context);
            st.state._userState = context.userState;
            return context.success ? ok(st.state, value) : st;
            };

            return new Parser<T>(headParser);
        }
    }
}
