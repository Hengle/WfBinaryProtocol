package message;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.PooledByteBufAllocator;
import io.netty.channel.ChannelHandlerContext;

public class BinaryMessage 
{
	private ByteBuf m_ByteBuf;
	private ChannelHandlerContext m_Context;
	
	public int getMsgId()
	{
	    return 0;
    }
	
	protected void read(ByteBuf buffer) 
	{
		m_ByteBuf = buffer;
		read();
	}
	public void write(ChannelHandlerContext ctx)
	{
		m_Context = ctx;
		m_ByteBuf = m_Context.alloc().buffer(1024*20);
		write();
	}
	
	public void Flush() 
	{
		m_Context.write(m_ByteBuf);
		m_Context.flush();
	}
	protected void write()
	{

	}
	
	protected void writeShort(short data)
	{
		m_ByteBuf.writeShort(data);
	}
	protected void writeInt(int data)
	{
		m_ByteBuf.writeInt(data);
	}
	
	protected void read()
	{
			
	}
	protected short readShort() 
	{
		return m_ByteBuf.readShort();
	}
	protected long readLong() {
		return m_ByteBuf.readLong();
	}
	protected String readString() {
		int length = m_ByteBuf.readInt();
		byte[] strBytes = new byte[length];
		m_ByteBuf.readBytes(strBytes);
		String result = new String(strBytes);
		return result;
	}
	
	protected int readInt() 
	{
		return m_ByteBuf.readInt();
	}
	protected void writeString(String data)
	{
		byte[] bytes = data.getBytes();
		int length = bytes.length;
		m_ByteBuf.writeInt(length);
		m_ByteBuf.writeBytes(bytes);
	}	
	protected void writeLong(long data)
	{
		m_ByteBuf.writeLong(data);
	}
	
	
}
