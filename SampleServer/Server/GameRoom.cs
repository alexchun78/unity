using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        object _lock = new object();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();


        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Flush()
        {
            // N ^2 -> N으로 : 패킷 모아보내기로 부하를 줄여주어야 한다. 
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items.");
            _pendingList.Clear();
        }
        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        //public void Broadcast(ClientSession session, string chat)
        //{
        //    S_Chat packet = new S_Chat();
        //    packet.playerID = session.SessionID;
        //    packet.chat = $"{chat} I am {packet.playerID}";

        //    ArraySegment<byte> segment = packet.Write();

        //    _pendingList.Add(segment);
        //}

        public void Enter(ClientSession session)
        {
            // 플레이어를 추가한다. 
            _sessions.Add(session);
            session.Room = this;

            // 새로 온 플레이어에게 현재 모든 플레이어의 목록을 제공한다.
            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in _sessions)
            {
                players.players.Add(
                    new S_PlayerList.Player()
                    {
                        isSelf = (s == session)? true : false,
                        playerID = s.SessionID,
                        PosX = s.PosX,
                        PosY = s.PosY,
                        PosZ = s.PosZ,
                    }
                );
            }
            session.Send(players.Write());

            // 모두에게 새로 온 플레이어에 대해 알린다. 
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame()
            {
                playerID = session.SessionID,
                PosX = 0,
                PosY = 0,
                PosZ = 0,
            };
            Broadcast(enter.Write()); 
        }

        public void Leave(ClientSession session)
        {
            // 플레이어를 제거하고
            _sessions.Remove(session);

            // 플레이어가 나간 걸 알려준다.
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame() { playerID = session.SessionID };
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            //좌표 바꿔주고
            session.PosX = packet.PosX;
            session.PosY = packet.PosY;
            session.PosZ = packet.PosZ;

            //모두에게 알린다.
            S_BroadcastMove move = new S_BroadcastMove()
            {
                playerID = session.SessionID,
                PosX = session.PosX,
                PosY = session.PosY,
                PosZ = session.PosZ,
            };
            Broadcast(move.Write());
        }
    }
}
