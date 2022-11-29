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
	class ClientSession : PacketSession
    {
        public int SessionID { get; set; }
        public GameRoom Room { get; set; }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");
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
            // TODO : chat service
            Program.Room.Push(() => Program.Room.Enter(this));
        }

        // [][][][][][][][][][][][]
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);

            if(Room != null)
            {
                // 실행 시점이 나중이므로, 변수에 참조로 임시 저장한 후 실행시킨다.
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }

            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }
}
