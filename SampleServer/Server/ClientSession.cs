﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Packet
    {
        // packet 사이즈와 ID는 기본으로 같이 보내준다.
        // packet 사이즈를 최대한 압축해서 보내는 게 좋다.
        public ushort size;
        public ushort packetID;
    }

    class PlayerInfoReq : Packet
    {
        public long playerId;
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

            Packet packet = new Packet() { size = 100, packetID = 10 };

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

            Thread.Sleep(5000);
            DisConnect();
        }

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
                        long playerId = BitConverter.ToInt32(buffer.Array, buffer.Offset + count);
                        count += 8;
                        Console.WriteLine($"PlayerInfoReq : {playerId}");
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
