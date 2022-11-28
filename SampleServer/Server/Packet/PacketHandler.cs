using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;

class PacketHandler
{
    // 수동으로 관리, 해당 패킷이 다 되었으면, 무엇을 호출할 지 결정해준다.
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq playerInfoReq = packet as C_PlayerInfoReq;

        Console.WriteLine($"PlayerInfoReq : {playerInfoReq.playerID},PlayerName : {playerInfoReq.name}");

        foreach (var skill in playerInfoReq.skills)
        {
            Console.WriteLine($"Skill : ({skill.id}),({skill.level}),({skill.duration})");
        }
    }
}

