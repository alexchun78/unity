using System;
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

        public PlayerInfoReq()
        {
            packetID = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ushort count = 0;

            ReadOnlySpan<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            // ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            count += 2;
            // ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            //long playerId = BitConverter.ToInt64(s.Array, s.Offset + count); // 계속 충분한 공간이 있는 지 확인해야 한다.\
            //this.playerID = BitConverter.ToInt64(new ReadOnlySpan<byte>(s.Array, s.Offset + count, s.Count - count));
            this.playerID = BitConverter.ToInt64(span.Slice(count, span.Length - count));
            count += 8;
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
            count += sizeof(ushort);
            isSuccess &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.packetID);
            // playerID를 byte로 변환하여 저장
            count += sizeof(ushort);
            isSuccess &= BitConverter.TryWriteBytes(segment.Slice(count, span.Length - count), this.playerID);
            // 전체 size를 byte로 변환하여 저장
            count += sizeof(long);
            isSuccess &= BitConverter.TryWriteBytes(span, count);
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
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2; 

            switch((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    {
                        PlayerInfoReq playerInfoReq = new PlayerInfoReq();
                        playerInfoReq.Read(buffer);

                        //long playerId = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
                        //count += 8;
                        Console.WriteLine($"PlayerInfoReq : {playerInfoReq.playerID}");
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
