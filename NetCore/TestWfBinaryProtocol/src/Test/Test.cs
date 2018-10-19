using System.Collections;
using System.Collections.Generic;

public class Test
{

    private MessageHandler m_Handler;

	// Use this for initialization
	public void Start ()
    {
        NetworkManager.Init();
        NetworkManager.StartNetwork(NetworkProtol.Tcp);

        NetworkManager.InitTcpNet((int)NetConSvrType.loginSvr);
        NetworkManager.ConnectTcpNet("127.0.0.1", 40000);

        m_Handler = new MessageHandler();
        m_Handler.Reg();
    }
	
	// Update is called once per frame
	public bool Update ()
    {
        NetworkManager.Update();

        if (m_Handler != null)
        {
            if(m_Handler.IsReceived())
            {
                m_Handler.ReqStopMove(0);
            }
        }

        return true;
    }
    public void OnDestroy()
    {
        m_Handler.UnReg();
        NetworkManager.Destroy();
    }
}
