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
    public abstract class Packet
    {
        // packet 사이즈와 ID는 기본으로 같이 보내준다.
        // packet 사이즈를 최대한 압축해서 보내는 게 좋다.
        public ushort size;
        public ushort packetID;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);

    }

    class PlayerInfoReq : Packet
    {
        public long playerID;
        public string name;

        public struct SkillInfo
        {
            public int id;
            public short level;
            public float duration;

            public bool Write(Span<byte> span, ref ushort count)
            {
                bool isSuccess = true;

                isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.id);
                count += sizeof(int);
                isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.level);
                count += sizeof(short);
                isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.duration);
                count += sizeof(float);

                return isSuccess;
            }

            public void Read(ReadOnlySpan<byte> span, ref ushort count)
            {
                bool isSuccess = true;
                this.id = BitConverter.ToInt32(span.Slice(count, span.Length - count));
                count += sizeof(int);
                this.level = BitConverter.ToInt16(span.Slice(count, span.Length - count));
                count += sizeof(short);
                this.duration = BitConverter.ToSingle(span.Slice(count, span.Length - count));
                count += sizeof(float);
            }
        }

        public List<SkillInfo> skills = new List<SkillInfo>();

        public PlayerInfoReq()
        {
            packetID = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            // ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += sizeof(ushort);
            // ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += sizeof(ushort);
            //long playerId = BitConverter.ToInt64(s.Array, s.Offset + count); // 계속 충분한 공간이 있는 지 확인해야 한다.\
            //this.playerID = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            this.playerID = BitConverter.ToInt64(span.Slice(count, span.Length - count));
            count += sizeof(long);

            // string 변환
            ushort nameLen = BitConverter.ToUInt16(segment.Array, count);
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(span.Slice(count, nameLen));
            count += nameLen;

            // skill list
            skills.Clear();
            ushort skillLen = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            for (ushort i = 0; i < skillLen; ++i)
            {
                SkillInfo skillInfo = new SkillInfo();
                skillInfo.Read(span, ref count);
                skills.Add(skillInfo);
            }

            Console.WriteLine($"PlayerInfoReq : {this.playerID}");
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);

            // [s][][][][][][][][][c]
            // 멀티쓰레딩 환경에서 한번이라도 실패하면, isSuccess가 false가 되도록 &= 으로 한다. 
            // GetBytes보다 더 속도면에서 좋지만, 개발자가 핸들링할 게 있다. 
            ushort count = 0;
            bool isSuccess = true;
#if true
            Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            // packetID를 byte로 변환하여 저장
            //isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.size);
            count += sizeof(ushort); // ->  이게 원래 packet의 전체 사이즈를 적는 부분인데... 생략되어 있다. size를 알 수 없어서 맨 마지막에 count로 넣어준다.
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.packetID);
            // playerID를 byte로 변환하여 저장
            count += sizeof(ushort);
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.playerID);
            // 전체 size를 byte로 변환하여 저장
            count += sizeof(long);

            // string
            // [NOTE] string 실체와 string 길이를 나누어서 생각해야 한다.
            // string length [2]
            // UTF 16 -> Encoding.Unicode
            //ushort nameLen = (ushort)Encoding.Unicode.GetByteCount(this.name);
            //isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
            //count += sizeof(ushort);
            //Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLen);
            //count += nameLen;           
            ushort nameLen = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort));
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLen);
            count += sizeof(ushort);
            count += nameLen;

            // skill List
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)skills.Count);
            count += sizeof(ushort);
            foreach (SkillInfo skill in skills)
            {
                isSuccess &= skill.Write(span, ref count);
            }

            isSuccess &= BitConverter.TryWriteBytes(span, count); // 여기서 맨 앞에 비워둔 공간에 size를 넣어준다.
#else
            
            isSuccess &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset + count, segment.Count - count), this.packetID);
            count += sizeof(ushort);
            isSuccess &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset + count, segment.Count - count), this.playerID);
            count += sizeof(long);
            isSuccess &= BitConverter.TryWriteBytes(new Span<byte>(segment.Array, segment.Offset, segment.Count), count); // packet.size
#endif

            if (isSuccess == false)
                return null;

            return SendBufferHelper.Close(count);
        }
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
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
                            Console.WriteLine($"Skill : {skill.id},{skill.level},{skill.duration}");
                        }
                    
                    }
                    break;
                case PacketID.PlayerInfoOk:
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
