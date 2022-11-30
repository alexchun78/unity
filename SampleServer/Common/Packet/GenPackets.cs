using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum PacketID
{
    C_Chat = 1,
	S_Chat = 2,
	
}

interface IPacket
{
	ushort Protocol { get; }
	void Read (ArraySegment<byte> segment);
	ArraySegment<byte> Write();

}


class C_Chat : IPacket
{
    
	public string chat;
	

	public ushort Protocol { get { return (ushort)PacketID.C_Chat;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		ushort chatLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(span.Slice(count,chatLen));
		count += chatLen;
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool isSuccess = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.C_Chat);
        count += sizeof(ushort);
        
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
		

        isSuccess &= BitConverter.TryWriteBytes(span, count);
        if (isSuccess == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}

class S_Chat : IPacket
{
    
	public int playerID;
	
	public string chat;
	

	public ushort Protocol { get { return (ushort)PacketID.S_Chat;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        ReadOnlySpan<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        count += sizeof(ushort);
        
		this.playerID = BitConverter.ToInt32(span.Slice(count, span.Length - count));
		count += sizeof(int);
		
		ushort chatLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
		count += sizeof(ushort);
		this.chat = Encoding.Unicode.GetString(span.Slice(count,chatLen));
		count += chatLen;
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);

        ushort count = 0;
        bool isSuccess = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);
        count += sizeof(ushort);
        isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.S_Chat);
        count += sizeof(ushort);
        
		isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.playerID);
		count += sizeof(int);
		
		ushort chatLen = (ushort)Encoding.Unicode.GetBytes(this.chat, 0, this.chat.Length, segment.Array, segment.Offset + count + sizeof(ushort));
		isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), chatLen);
		count += sizeof(ushort);
		count += chatLen;
		

        isSuccess &= BitConverter.TryWriteBytes(span, count);
        if (isSuccess == false)
            return null;

        return SendBufferHelper.Close(count);
    }
}

