using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public partial class Program
    {

        static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            { 
                Session session = new Session();
                session.Start(clientSocket);
            
                // 메시지 보낸다.
                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                session.Send(sendBuffer);

                Thread.Sleep(1000);
                session.DisConnect();
                session.DisConnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            // DNS(Domain Name System)
            string host = Dns.GetHostName();

            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // 식당 주소
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // 식당 문            

            // 문지기
            _listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening....");
            
            while (true)
            {

            }
        }
    }
}