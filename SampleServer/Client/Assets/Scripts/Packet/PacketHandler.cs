using DummyClient;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

class PacketHandler
{
    // 수동으로 관리, 해당 패킷이 다 되었으면, 무엇을 호출할 지 결정해준다.
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;

        if(chatPacket.playerID == 1)
        {
            Debug.Log(chatPacket.chat);

            GameObject go = GameObject.Find("Player");
            if(go == null)
            {
                Debug.Log("Player not found");
            }
            else
            {
                Debug.Log("Player found");
            }
        }
          
        //if (chatPacket.playerID == 1)
            //Console.WriteLine(chatPacket.chat);
    }

}
 