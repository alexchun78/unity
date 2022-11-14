using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    class Knight
    {
        public int hp;
        public int attack;
    }

    class GameSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            Knight knight = new Knight() { hp = 100, attack = 10 };

            // [100] [10]
            //byte[] sendBuffer = new byte[4096];
            //byte[] buffer_hp = BitConverter.GetBytes(knight.hp);
            //byte[] buffer_attack = BitConverter.GetBytes(knight.attack);
            //Array.Copy(buffer_hp, 0, sendBuffer, 0, buffer_hp.Length);
            //Array.Copy(buffer_attack, 0, sendBuffer, buffer_hp.Length, buffer_attack.Length);

            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer_hp = BitConverter.GetBytes(knight.hp);
            byte[] buffer_attack = BitConverter.GetBytes(knight.attack);
            Array.Copy(buffer_hp, 0, openSegment.Array, openSegment.Offset, buffer_hp.Length);
            Array.Copy(buffer_attack, 0, openSegment.Array, openSegment.Offset + buffer_hp.Length, buffer_attack.Length);
            ArraySegment<byte> sendBuffer = SendBufferHelper.Close(buffer_hp.Length + buffer_attack.Length);



            //byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            Send(sendBuffer);

            Thread.Sleep(1000);
            DisConnect();
        }

        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string receiveData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {receiveData}");
            return buffer.Count;
        }

        public override void OnSend(int numOfBytes)
        {
            Console.WriteLine($"Transferred bytes : {numOfBytes}");
        }
    }

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
            _listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening....");

            while (true)
            {

            }
        }
    }
}
