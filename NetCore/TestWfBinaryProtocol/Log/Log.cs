using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    public  class Log
    {
        public static void print(string log,params object[] data)
        {
            log = string.Format(log, data);
            Console.WriteLine("{0}:{1}", DateTime.UtcNow, log);
        }
    }
}
