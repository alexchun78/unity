using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    // 수동으로 관리, 해당 패킷이 다 되었으면, 무엇을 호출할 지 결정해준다.
    public static void S_BroadcastEnterGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastEnterGame chatPacket = packet as S_BroadcastEnterGame;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerID == 1)
            //Console.WriteLine(chatPacket.chat);
    }

    public static void S_BroadcastLeaveGameHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastLeaveGame chatPacket = packet as S_BroadcastLeaveGame;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerID == 1)
        //Console.WriteLine(chatPacket.chat);
    }

    public static void S_PlayerListHandler(PacketSession session, IPacket packet)
    {
        S_PlayerList chatPacket = packet as S_PlayerList;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerID == 1)
        //Console.WriteLine(chatPacket.chat);
    }

    public static void S_BroadcastMoveHandler(PacketSession session, IPacket packet)
    {
        S_BroadcastMove chatPacket = packet as S_BroadcastMove;
        ServerSession serverSession = session as ServerSession;

        //if (chatPacket.playerID == 1)
        //Console.WriteLine(chatPacket.chat);
    }
}
 