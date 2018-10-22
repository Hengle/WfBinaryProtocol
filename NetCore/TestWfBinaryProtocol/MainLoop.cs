using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace GameServer
{
    public class MainLoop
    {
        public bool m_StopServer =false;
        public bool m_UseTcp = true;
        public void Start()
        {
            if (m_UseTcp)
            {
                ServerNet.SetEnableDebug(false);
                ServerNet.Init();
            }
            else
            {
                KCPNet.Init();
            }
        }
        public void Update()
        {

        }
        public void Stop()
        {

        }
    }
}
