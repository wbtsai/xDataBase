using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.TypeScript
{
    public class State<U>
    {
        public string Source { get; set; }
        public int Position { get; set; }
        public U UserState { get; set; }

        public State(string source,int position,U userState)
        {
            if (position < 0 || position > source.Length + 1)
            {
                throw new Exception("_position: out of range: " + position);
            }

            this.Source = source;
            this.Position = position;
            this.UserState = userState;
        }

        public int[] GetRowColumn() {
            
            var lines = this.Source.Split('\n');
            var position = 0;
            var raw = 0;
            while(position < this.Position){
                if(this.Position <= position + lines[raw].Length)
                {
                    break;
                }
                position += lines[raw].Length + 1;
                raw++;
            }

            var column = this.Position - position;
            return new int[]{raw,column};
        }
        
        public State<U> Seek(int delta)
        {
            return new State<U>(this.Source, this.Position + delta, this.UserState);
        }

        public bool Equals(State<U> src)
        { 
            return this.Source == src.Source && this.Position == src.Position && this.UserState.Equals(src.UserState);
        }
    }
}
