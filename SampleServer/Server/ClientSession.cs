using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{

	public enum PacketID
	{
		PlayerInfoReq = 1,
		Test = 2,

	}


	class PlayerInfoReq
	{

		public byte testByte;


		public long playerID;


		public string name;


		public class Skill
		{

			public int id;


			public short level;


			public float duration;


			public class Attribute
			{

				public int att;


				public void Read(ReadOnlySpan<byte> span, ref ushort count)
				{

					this.att = BitConverter.ToInt32(span.Slice(count, span.Length - count));
					count += sizeof(int);

				}

				public bool Write(Span<byte> span, ref ushort count)
				{
					bool isSuccess = true;

					isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.att);
					count += sizeof(int);

					return isSuccess;
				}
			}
			public List<Attribute> attributes = new List<Attribute>();


			public void Read(ReadOnlySpan<byte> span, ref ushort count)
			{

				this.id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
				count += sizeof(int);


				this.level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
				count += sizeof(short);


				this.duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
				count += sizeof(float);


				this.attributes.Clear();
				ushort attributeLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
				count += sizeof(ushort);
				for (ushort i = 0; i < attributeLen; ++i)
				{
					Attribute attribute = new Attribute();
					attribute.Read(span, ref count);
					attributes.Add(attribute);
				}

			}

			public bool Write(Span<byte> span, ref ushort count)
			{
				bool isSuccess = true;

				isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.id);
				count += sizeof(int);


				isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
				count += sizeof(short);


				isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.duration);
				count += sizeof(float);


				isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)this.attributes.Count);
				count += sizeof(ushort);
				foreach (Attribute attribute in this.attributes)
				{
					isSuccess &= attribute.Write(span, ref count);
				}

				return isSuccess;
			}
		}
		public List<Skill> skills = new List<Skill>();


		public void Read(ArraySegment<byte> segment)
		{
			ushort count = 0;

			ReadOnlySpan<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);
			count += sizeof(ushort);
			count += sizeof(ushort);

			this.testByte = (byte)segment.Array[segment.Offset + count];
			count += sizeof(byte);


			this.playerID = BitConverter.ToInt64(span.Slice(count, span.Length - count));
			count += sizeof(long);


			ushort nameLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
			count += sizeof(ushort);
			this.name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
			count += nameLen;


			this.skills.Clear();
			ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
			count += sizeof(ushort);
			for (ushort i = 0; i < skillLen; ++i)
			{
				Skill skill = new Skill();
				skill.Read(span, ref count);
				skills.Add(skill);
			}

		}

		public ArraySegment<byte> Write()
		{
			ArraySegment<byte> segment = SendBufferHelper.Open(4096);

			ushort count = 0;
			bool isSuccess = true;

			Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);
			count += sizeof(ushort);
			isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.PlayerInfoReq);
			count += sizeof(ushort);

			segment.Array[segment.Offset + count] = (byte)this.testByte;
			count += sizeof(byte);


			isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.playerID);
			count += sizeof(long);


			ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
			isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
			count += sizeof(ushort);
			count += nameLen;


			isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)this.skills.Count);
			count += sizeof(ushort);
			foreach (Skill skill in this.skills)
			{
				isSuccess &= skill.Write(span, ref count);
			}


			isSuccess &= BitConverter.TryWriteBytes(span, count);
			if (isSuccess == false)
				return null;

			return SendBufferHelper.Close(count);
		}
	}


	class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

          //  PlayerInfoReq packet = new PlayerInfoReq() { packetID = 10 };
#if false
            // [100] [10]
            //byte[] sendBuffer = new byte[4096];
            //byte[] buffer_hp = BitConverter.GetBytes(packet.hp);
            //byte[] buffer_attack = BitConverter.GetBytes(packet.attack);
            //Array.Copy(buffer_hp, 0, sendBuffer, 0, buffer_hp.Length);
            //Array.Copy(buffer_attack, 0, sendBuffer, buffer_hp.Length, buffer_attack.Length);

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            //byte[] buffer_hp = BitConverter.GetBytes(packet.size);
            //byte[] buffer_attack = BitConverter.GetBytes(packet.packetID);
            //Array.Copy(buffer_hp, 0, openSegment.Array, openSegment.Offset, buffer_hp.Length);
            //Array.Copy(buffer_attack, 0, openSegment.Array, openSegment.Offset + buffer_hp.Length, buffer_attack.Length);
            //ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer_hp.Length + buffer_attack.Length);

            ////byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            //Send(sendBuffer);
#endif
            Thread.Sleep(5000);
            DisConnect();
        }

        // [][][][][][][][][][][][]
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += sizeof(ushort);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq playerInfoReq = new PlayerInfoReq();
                        playerInfoReq.Read(buffer);

                        //long playerId = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
                        //count += 8;
                        Console.WriteLine($"PlayerInfoReq : {playerInfoReq.playerID},PlayerName : {playerInfoReq.name}");
                    
                        foreach(var skill in playerInfoReq.skills)
                        {
                            Console.WriteLine($"Skill : ({skill.id}),({skill.level}),({skill.duration})");
                        }
                    
                    }
                    break;
               // case PacketID.PlayerInfoOk:
                    break;
            }

            Console.WriteLine($"RecvPacketID : {id}, Size : {size}");
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
