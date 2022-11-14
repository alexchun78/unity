using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        object _lock = new object();

        SocketAsyncEventArgs _sendArgc = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgc = new SocketAsyncEventArgs();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);

        public abstract void OnSend(int numOfBytes);

        public abstract void OnDisconnected(EndPoint endPoint);

        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgc.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            RegisterRecv(); // 최초로 낚싯대를 휙 던진 거 (비유를 굳이 한다면)

            _sendArgc.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
        }

        public void Send(ArraySegment<byte> sendBuff)
        {
            lock(_lock)
            {
                // 멀티쓰레드 환경에서 간섭이 있을 수 있으므로, send Complete 되는 시점에만 해당 메시지를 보낼 수 있게
                // 일단, queue를 만들어 저장한다.
                _sendQueue.Enqueue(sendBuff);
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
#if false
            // // _socket.Send(sendBuff);
            //SocketAsyncEventArgs sendArgc = new SocketAsyncEventArgs();
            //sendArgc.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            //_sendArgc.SetBuffer(sendBuff, 0, sendBuff.Length);
            //RegisterSend();
#endif
        }

        public void DisConnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
                return;

            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both); // 서버 끊기 전에 미리 양쪽에 공지한다. 
            _socket.Close();
        }

#region 네트워크 통신
        void RegisterSend()
        {
#if true // 한번에 리스트 단위로 넣는 작업
            //_pendingList.Clear();

            while(_sendQueue.Count > 0)
            {
                ArraySegment<byte> tempElement = _sendQueue.Dequeue();
                _pendingList.Add(tempElement);
                //[NOTE] 이렇게 바로 넣는 것은 안된다!! -> 잠재적 버그 : _sendArgc.BufferList.Add(new ArraySegment<byte>(tempElement, 0, tempElement.Length));
            }
            _sendArgc.BufferList = _pendingList;
#else // 1개씩 꺼내서 리스트에 넣는 작업
            _isPending = true;
            byte[] buffer_temp = _sendQueue.Dequeue();
            _sendArgc.SetBuffer(buffer_temp, 0, buffer_temp.Length);
#endif
            bool isPending = _socket.SendAsync(_sendArgc);
            if (isPending == false)
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
                        _sendArgc.BufferList = null;
                        _pendingList.Clear();

                        OnSend(_sendArgc.BytesTransferred);

                        // 예약을 하는 동안에 누군가 또 메시지를 보냈으면, 
                        if (_sendQueue.Count > 0)
                            RegisterSend();
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

        void RegisterRecv()
        {
            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgc.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool isPending = _socket.ReceiveAsync(_recvArgc);
            if(isPending == false)
            {
                OnRecvCompleted(null, _recvArgc);
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // write 커서 이동 
                    if(_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        DisConnect();
                        return;
                    }
                    // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는 지 받는다.
                    int processLen = OnRecv(_recvBuffer.ReadSegment);
                    if( processLen < 0 || _recvBuffer.DataSize < processLen)
                    {
                        DisConnect();
                        return;
                    }

                    // Read 커서 이동
                    if(_recvBuffer.OnRead(processLen) == false)
                    {
                        DisConnect();
                        return;
                    }

                    //OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    RegisterRecv();
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
 