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
        SocketAsyncEventArgs _sendArgc = new SocketAsyncEventArgs();
        Queue<byte[]> _sendQueue = new Queue<byte[]>();
        bool _isPending = false;
        object _lock = new object();

        public void Start(Socket socket)
        {
            _socket = socket;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // args.UserToken = 1; / / 추가 정보를 넘기고 싶을 때, 
            args.SetBuffer(new byte[1024], 0, 1024);
            RegisterRecv(args); // 최초로 낚싯대를 휙 던진 거 (비유를 굳이 한다면)

            _sendArgc.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        }

        public void Send(byte[] sendBuff)
        {
            lock(_lock)
            {
                // 멀티쓰레드 환경에서 간섭이 있을 수 있으므로, send Complete 되는 시점에만 해당 메시지를 보낼 수 있게
                // 일단, queue를 만들어 저장한다.
                _sendQueue.Enqueue(sendBuff);
                if (_isPending == false)
                    RegisterSend();
            }

            // // _socket.Send(sendBuff);
            //SocketAsyncEventArgs sendArgc = new SocketAsyncEventArgs();
            //sendArgc.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            //_sendArgc.SetBuffer(sendBuff, 0, sendBuff.Length);
            //RegisterSend();
        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            _socket.Shutdown(SocketShutdown.Both); // 서버 끊기 전에 미리 양쪽에 공지한다. 
            _socket.Close();
        }

        #region 네트워크 통신
        void RegisterSend()
        {
            _isPending = true;
            byte[] buffer_temp = _sendQueue.Dequeue();
            _sendArgc.SetBuffer(buffer_temp, 0, buffer_temp.Length);
            
            bool isPending = _socket.SendAsync(_sendArgc);
            if(isPending == false)
            {
                OnSendCompleted(null, _sendArgc);
            }
        }

        void OnSendCompleted(object obj, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        // 예약을 하는 동안에 누군가 또 메시지를 보냈으면, 
                        if (_sendQueue.Count > 0)
                            RegisterSend();
                        else
                            _isPending = false;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendOnComplefe is failed. {e.ToString()}");
                    }
                }
                else
                {
                    DisConnect();
                }
            }
        }

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
                DisConnect();
            }
        }
        #endregion
    }
}
 