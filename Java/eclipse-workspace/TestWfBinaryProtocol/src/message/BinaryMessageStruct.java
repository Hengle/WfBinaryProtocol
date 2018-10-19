package message;

import io.netty.buffer.ByteBuf;
import io.netty.buffer.PooledByteBufAllocator;

public class BinaryMessageStruct 
{
	private ByteBuf m_ByteBuf;
	protected void read(ByteBuf buffer)
	{
		m_ByteBuf = buffer;
		read();
	}
	

	protected void writeInt(int data)
	{
		m_ByteBuf.writeInt(data);
	}
	protected void writeString(String data)
	{
		byte[] bytes = data.getBytes();
		int length = bytes.length;
		m_ByteBuf.writeInt(length);
		m_ByteBuf.writeBytes(bytes);
	}
	protected int readInt()
	{
		return m_ByteBuf.readInt();
	}
	protected String readString() 
	{
		int length = m_ByteBuf.readInt();
		byte[] strBytes = new byte[length];
		m_ByteBuf.readBytes(strBytes);
		String result = new String(strBytes);
		return result;
	}
	protected void write()
	{
		PooledByteBufAllocator allocator = new PooledByteBufAllocator(false);
        m_ByteBuf = allocator.heapBuffer(15);
	}
	protected void read()
	{

	}
}
