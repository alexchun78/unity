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
                // 메시지 받는다.
                byte[] recvBuffer = new byte[1024];
                int recievedBytes = clientSocket.Receive(recvBuffer);
                // // 문자열 변환
                string receiveData = Encoding.UTF8.GetString(recvBuffer, 0, recievedBytes);
                Console.WriteLine($"[From Client] {receiveData}");

                // 메시지 보낸다.
                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                int sendBytes = clientSocket.Send(sendBuffer);

                // 쫓아낸다.
                clientSocket.Shutdown(SocketShutdown.Both); // 서버 끊기 전에 미리 양쪽에 공지한다. 
                clientSocket.Close();
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