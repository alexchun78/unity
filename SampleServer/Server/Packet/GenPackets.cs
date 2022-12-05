using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

public enum PacketID
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read (ArraySegment<byte> segment);
	ArraySegment<byte> Write();

}

public class S_BroadcastEnterGame : IPacket
{
    public int playerID;
	public float PosX;
	public float PosY;
	public float PosZ;
	
	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEnterGame;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int)); 
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_LeaveGame : IPacket
{
    
	public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_BroadcastLeaveGame : IPacket
{
    public int playerID;
	
	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int)); 
		count += sizeof(int);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_PlayerList : IPacket
{
    public class Player
	{
	    public bool isSelf;
		public int playerID;
		public float PosX;
		public float PosY;
		public float PosZ;
		
	
	    public void Read(ArraySegment<byte> segment, ref ushort count)
	    {
	        this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
			count += sizeof(bool);
			this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
			count += sizeof(int);
			this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
			count += sizeof(float);
			
	    }
	
	    public bool Write(ArraySegment<byte> segment, ref ushort count)
	    {
	        bool isSuccess = true;
	        Array.Copy(BitConverter.GetBytes(this.isSelf), 0, segment.Array, segment.Offset + count, sizeof(bool)); 
			count += sizeof(bool);
			Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int)); 
			count += sizeof(int);
			Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float)); 
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float)); 
			count += sizeof(float);
			Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float)); 
			count += sizeof(float);
			
	        return isSuccess;
	    }
	}
	public List<Player> players = new List<Player>();
	
	public ushort Protocol { get { return (ushort)PacketID.S_PlayerList;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.players.Clear();
		ushort playerLen = BitConverter.ToUInt16(segment.Array, segment.Offset + count);
		count += sizeof(ushort);
		for(ushort i = 0; i < playerLen; ++i)
		{
		    Player player = new Player();
		    player.Read(segment, ref count);
		    players.Add(player);
		}
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerList), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.players.Count), 0, segment.Array, segment.Offset + count, sizeof(ushort)); 
		count += sizeof(ushort);
		foreach(Player player in this.players)
		{
		    player.Write(segment, ref count);
		}
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class C_Move : IPacket
{
    public float PosX;
	public float PosY;
	public float PosZ;
	
	public ushort Protocol { get { return (ushort)PacketID.C_Move;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}
public class S_BroadcastMove : IPacket
{
    public int playerID;
	public float PosX;
	public float PosY;
	public float PosZ;
	
	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastMove;  } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;

        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerID = BitConverter.ToInt32(segment.Array, segment.Offset + count);
		count += sizeof(int);
		this.PosX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		this.PosZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
		count += sizeof(float);
		
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerID), 0, segment.Array, segment.Offset + count, sizeof(int)); 
		count += sizeof(int);
		Array.Copy(BitConverter.GetBytes(this.PosX), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosY), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		Array.Copy(BitConverter.GetBytes(this.PosZ), 0, segment.Array, segment.Offset + count, sizeof(float)); 
		count += sizeof(float);
		
        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}

