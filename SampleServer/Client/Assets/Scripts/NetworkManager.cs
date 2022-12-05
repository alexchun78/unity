using DummyClient;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession _session = new ServerSession();
    void Start()
    {
        // DNS(Domain Name System)
        string host = Dns.GetHostName();

        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0]; // 식당 주소
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 식당 문

        Connector connector = new Connector();
        connector.Connect(endPoint,
            () => { return _session; },
            1);
    }

    void Update()
    {
        List<IPacket> pckList = PacketQueue.Instance.PopAll();
//        IPacket packet = PacketQueue.Instance.Pop();
        foreach( IPacket packet in pckList)
        {
                PacketManager.Instance.HandlePacket(_session, packet);
        }
    }

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }
}
