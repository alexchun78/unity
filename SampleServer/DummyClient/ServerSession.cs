using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
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


	class ServerSession : Session
    {
        // c#에서 c++의 포인터 형태를 빌려와서 메모리를 접근할 때의 방식
#if false
        static void ToBytes(byte[] array, int offset, ulong value)
        {
            unsafe
            {
                fixed (byte* ptr = &array[offset])
                {
                    *(ulong*)ptr = value;
                }
            }
        }
#endif

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { playerID = 1001, name = "ABCD" };
			var skill = new PlayerInfoReq.Skill() { id = 101, level = 1, duration = 3.0f };
			skill.attributes.Add(new PlayerInfoReq.Skill.Attribute() { att = 77 });
            packet.skills.Add(skill);

            //packet.skills.Add(new PlayerInfoReq.Skill() { 
            //    id=2003,
            //    level = 1,
            //    duration = 100
            //});
            packet.skills.Add(new PlayerInfoReq.Skill()
            {
                id = 2008,
                level = 14,
                duration = 200
            });
            packet.skills.Add(new PlayerInfoReq.Skill()
            {
                id = 2010,
                level = 16,
                duration = 110
            });


            {
                ArraySegment<byte> sendBuffer = packet.Write();
                if (sendBuffer != null)
                    Send(sendBuffer);
            }
#if false
            //  메시지 보낸다.
            //for (int i = 0; i < 5; ++i)
            {
                //ArraySegment<byte> segment = SendBufferHelper.Open(4096);

                //// [s][][][][][][][][][c]
                //// 멀티쓰레딩 환경에서 한번이라도 실패하면, isSuccess가 false가 되도록 &= 으로 한다. 
                //// GetBytes보다 더 속도면에서 좋지만, 개발자가 핸들링할 게 있다. 
                //ushort count = 0;
                //bool isSuccess = true;

                //count += 2;
                //isSuccess &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset+ count, segment.Count- count), packet.packetID);
                //count += 2;
                //isSuccess &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset + count, segment.Count - count), packet.playerID);
                //count += 8;
                //isSuccess &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset, segment.Count), count); // packet.size

                //ArraySegment<byte> sendBuffer = SendBufferHelper.Close(count);//(packet.size);

                //if(isSuccess)
                //    Send(sendBuffer);

                byte[] size = BitConverter.GetBytes(packet.size); // 2 byte
                byte[] packetID = BitConverter.GetBytes(packet.packetID); // 2 byte
                byte[] playerID = BitConverter.GetBytes(packet.playerID); // 8 byte                
                Array.Copy(size, 0, segment.Array, segment.Offset, 2); //size.Length);
                count += 2;
                Array.Copy(packetID, 0, segment.Array, segment.Offset + count, 2);// size.Length, packetID.Length);
                count += 2;
                Array.Copy(playerID, 0, segment.Array, segment.Offset + count, 8);//  size.Length, packetID.Length);
                count += 8;

        }
#endif
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        // 이동 패킷 =>  (3,2) 좌표로 이동하고 싶다.
        // 15  3 2
        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string receiveData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {receiveData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
