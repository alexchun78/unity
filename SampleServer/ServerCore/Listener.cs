using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Listener
    {
        Socket _listenSocket;
        Action<Socket> _onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            // 문지기
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _onAcceptHandler = onAcceptHandler;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            _listenSocket.Listen(10); // backlog : 최대 대기수 -> 문지기가 안내할 때까지 몇 명이 대기하게 할 것인지..

            //blocking 코드를 없애기 위해 -> 비동기로 만들기 위해
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args); // 최초로 낚싯대를 휙 던진 거 (비유를 굳이 한다면)
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            // 꼭 초기화를 시켜주어야 한다.
            args.AcceptSocket = null;


            bool isPending = _listenSocket.AcceptAsync(args); // 잡힐 수도 있고 아닐 수 있고
            if (isPending == false) // 바로 잡혔다.
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                // TODO 
                _onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args); // 물고기를 빼놓았으면, 다시 낚싯대를 던져놓는다.
        }
    }
}
