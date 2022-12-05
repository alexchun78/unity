using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager _network;
    void Start()
    {
        StartCoroutine("CoSendPacket");
        _network = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    void Update()
    {
        
    }

    IEnumerator CoSendPacket()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);

            C_Move movePacket = new C_Move()
            {
                PosX = UnityEngine.Random.Range(-50, 50),
                PosY = 0,
                PosZ = UnityEngine.Random.Range(-50, 50),
            };

            _network.Send(movePacket.Write());
        }
    }
}
