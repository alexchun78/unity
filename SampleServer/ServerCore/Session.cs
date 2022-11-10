using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket _socket;
        int _disconnected = 0;

        public void Start(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // args.UserToken = 1; / / 추가 정보를 넘기고 싶을 때, 
            args.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(args); // 최초로 낚싯대를 휙 던진 거 (비유를 굳이 한다면)
        }

        public void Send(byte[] sendBuff)
        {
            _socket.Send(sendBuff); // 메시지 보낸다.
        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both); // 서버 끊기 전에 미리 양쪽에 공지한다. 
            _socket.Close();
        }

        #region 네트워크 통신

        void RegisterRecv(SocketAsyncEventArgs args)
        {
            

            bool isPending = _socket.ReceiveAsync(args);
            if(isPending == false)
            {
                OnRecvCompleted(null, args);
            }

        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string receiveData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {receiveData}");
                    RegisterRecv(args);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted is failed. {e.ToString()}");
                }
            }
            else
            { 

            }
        }
        #endregion
    }
}
 