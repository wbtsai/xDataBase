using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class Parser<A>
    {
        public Func<State, Reply<A>> RunParser { get; set; }

        public Parser(Func<State, Reply<A>> runParser)
        {
            this.RunParser = runParser;
        }
    }
}
