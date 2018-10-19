using System;
using System.Threading;

namespace TestWfBinaryProtocol
{
    class Program
    {
        static void Main(string[] args)
        {
            Test app = new Test();
            app.Start();
            while(true)
            {
                bool run = app.Update();
                if(run == false )
                {
                    break;
                }
                Thread.Sleep(1000);
            }

            app.OnDestroy();
        }
    }
}
