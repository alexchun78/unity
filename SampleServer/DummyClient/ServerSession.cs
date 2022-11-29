using ServerCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DummyClient
{
	class ServerSession : PacketSession
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
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            //Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
