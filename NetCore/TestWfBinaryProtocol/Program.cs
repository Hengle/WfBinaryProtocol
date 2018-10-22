using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameServer;
namespace GameServer
{
    class Program
    {
        public static MainLoop m_MainLoop = new MainLoop();
        static void Main(string[] args)
        {
            m_MainLoop.Start();
            while (!m_MainLoop.m_StopServer)
            {
                m_MainLoop.Update();
            }
            m_MainLoop.Stop();
            m_MainLoop = null;
        }
    }
}
