﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{


    public partial class Program
    {

        static Listener _listener = new Listener();

        static void Main(string[] args)
        {
            // DNS(Domain Name System)
            string host = Dns.GetHostName();

            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // 식당 주소
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 식당 문            

            // 문지기
            _listener.Init(endPoint, () => { return new ClientSession(); });
            Console.WriteLine("Listening....");

            while (true)
            {

            }
        }
    }
}
