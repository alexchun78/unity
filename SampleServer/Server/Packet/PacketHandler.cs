using Server;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    // 수동으로 관리, 해당 패킷이 다 되었으면, 무엇을 호출할 지 결정해준다.
    public static void C_LeaveGameHandler(PacketSession session, IPacket packet)
    {
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        // 실행 시점이 나중이므로, 변수에 참조로 임시 저장한 후 실행시킨다.
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Leave(clientSession)
        );
    }

    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;

        if (clientSession.Room == null)
            return;

        //Console.WriteLine($"X : {movePacket.PosX}, Y : {movePacket.PosY}, Z : {movePacket.PosZ}");

        // 실행 시점이 나중이므로, 변수에 참조로 임시 저장한 후 실행시킨다.
        GameRoom room = clientSession.Room;
        room.Push(
            () => room.Move(clientSession, movePacket)
        );
    }
}

 