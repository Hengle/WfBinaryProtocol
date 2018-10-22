using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameServer
{
    public class PacketRegister
    {
        public static void Init(DP_Msg data)
        {
            if (Program.m_MainLoop.m_UseTcp)
            {
                TestTcp.Init(data);
            }
            else
            {
                TestUdp.Init();
            }
        }
    }
}
