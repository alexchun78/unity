using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory, int register = 10, int backlog = 100)
        {
            // 문지기
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            // 문지기 교육
            _listenSocket.Bind(endPoint);

            // 영업 시작
            _listenSocket.Listen(backlog); // backlog : 최대 대기수 -> 문지기가 안내할 때까지 몇 명이 대기하게 할 것인지..

            for(int i =0; i < register; ++i) // 낚싯대를 여러개 만들 때 
            {
                //blocking 코드를 없애기 위해 -> 비동기로 만들기 위해
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args); // 최초로 낚싯대를 휙 던진 거 (비유를 굳이 한다면)
            }
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
            // [NOTE] 여기는 Async로 멀티 쓰레드로 코드가 작동된다. (ThreadPool)
            // 동시 다발적으로 같은 데이터를 건들 가능성이 있다. Race condition 발생 가능함.
            // 나중에 Lock을 걸어주는 게 필요함.
            if(args.SocketError == SocketError.Success)
            {
                // TODO 
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                //_onAcceptHandler.Invoke(args.AcceptSocket);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccept(args); // 물고기를 빼놓았으면, 다시 낚싯대를 던져놓는다.
        }
    }
}
