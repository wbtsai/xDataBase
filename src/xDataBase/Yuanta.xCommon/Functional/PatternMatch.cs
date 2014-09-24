using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.Functional
{
    public class PatternMatch<T, TResult>
    {
        private readonly T value;
        private readonly List<Tuple<Func<T,bool>, Func<T, TResult>>> cases  = new List<Tuple<Func<T,bool>, Func<T, TResult>>>();
        private Func<T, TResult> elseFunc;

        internal PatternMatch(T value)
        {
            this.value = value;
        }

        public PatternMatch<T, TResult> With(Func<T,bool> condition, Func<T, TResult> result)
        {
            cases.Add(Tuple.Create(condition, result));
            return this;
        }

        public PatternMatch<T, TResult> Else(Func<T, TResult> result)
        {
            if (elseFunc != null)
                throw new InvalidOperationException("Cannot have multiple else cases");

            elseFunc = result;
            return this;
        }

        public TResult Do()
        {
            if (elseFunc != null)
                cases.Add(
                    Tuple.Create<Func<T,bool>, Func<T, TResult>>(x=>true, elseFunc));
            foreach (var item in cases)
            {
                if (item.Item1(value))
                    return item.Item2(value);
            }

            throw new MatchNotFoundException("Incomplete pattern match");
        }
    }

    public class MatchNotFoundException : Exception
    {
        public MatchNotFoundException(string message) : base(message) { }
    }

    public class PatternMatchContext<T>
    {
        private readonly T value;
        internal PatternMatchContext(T value)
        {
            this.value = value;
        }

        public PatternMatch<T, TResult> With<TResult>(
            Func<T,bool> condition,
            Func<T, TResult> result)
        {
            var match = new PatternMatch<T, TResult>(value);
            return match.With(condition, result);
        }
    }

    public static class PatternMatchExtensions
    {
        public static PatternMatchContext<T> Match<T>(this T value)
        {
            return new PatternMatchContext<T>(value);
        }
    }
}
