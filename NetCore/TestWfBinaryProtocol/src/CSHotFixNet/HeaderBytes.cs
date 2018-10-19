using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HeaderBytes:PooledClassObject
{
    public byte[] Bytes = new byte[WfPacket.GetHeaderLength()];
}

