using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityEngine
{
    public class Debug
    {
        public static void Log(object log)
        {
            Console.WriteLine(log);
        }

        public static void LogException(object log)
        {
            Console.WriteLine(log);
        }

        public static void LogError(object log)
        {
            Console.WriteLine(log);
        }
    }
}
