﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPP
{
    public class Parser<A>
    {
        public Func<State, Reply<A>> RunParser { get; set; }

        public Parser(Func<State, Reply<A>> runParser)
        {
            this.RunParser = runParser;
        }
    }
    
    public class Reply<A>
    {
        public State State { get; set; }
        public bool Success { get; set; }
        public A Value { get; set; }

        public Func<string> Expected { get; set; }

        public Reply(State state, bool success, A values, Func<string> expected)
        {
            this.State = state;
            this.Success = success;
            this.Value = values;
            this.Expected = expected;
        }
        
    }
    
public class State
    {
        public string Source { get; set; }
        public int Position { get; set; }
        public object UserState { get; set; }

        public State(string source,int position,object userState)
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
        
        public State Seek(int delta)
        {
            return new State(this.Source, this.Position + delta, this.UserState);
        }

        public bool Equals(State src)
        { 
            return this.Source == src.Source && this.Position == src.Position && this.UserState.Equals(src.UserState);
        }
    }

}
