using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yuanta.xCommon.Functional
{
    /// <summary>
    /// Pipe 的用法資料
    /// </summary>
    public static class PipeExtension
    {
        public static void Pipe<T>(this T val, Action<T> action) where T : class
        {
            if (val != null) 
                action(val); 
        }

        public static R Pipe<T, R>(this T val, Func<T, R> func)
            where T : class
            where R : class
        { 
            return val != null ? func(val) : null; 
        }
    }
}
