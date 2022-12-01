using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        // [size(2)][packetID(2)][...][size(2)][packetID(2)][...]
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;
            
            while(true)
            {
                // 최소한 헤드는 파싱할 수 있는 지 검사
                if (buffer.Count < HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는 지 검사
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 여기까지 왔으면 패킷 조립 가능함
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;
                // buffer를 업데이트
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);

                // 다음 패킷으로 이동함
                processLen += dataSize;
            }
            if (packetCount > 1)
                Console.WriteLine($"패킷 모아보내기 : {packetCount}");

            
            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }

    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(65535); // 1024

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
            _sendArgc.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
			
            RegisterRecv(); // 최초로 낚싯대를 휙 던진 거 (비유를 굳이 한다면)
        }

        void Clear()
        {
            lock(_lock)
            {
                _sendQueue.Clear();
                _pendingList.Clear();
            }
        }

        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
                return;

            lock (_lock)
            {
                foreach(ArraySegment<byte> sendBuff in sendBuffList)
                    _sendQueue.Enqueue(sendBuff);
                 
                if (_pendingList.Count == 0)
                    RegisterSend();
            }
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
            Clear();
        }

#region 네트워크 통신
        void RegisterSend()
        {
#if true // 한번에 리스트 단위로 넣는 작업
            //_pendingList.Clear();
            if (_disconnected == 1)
                return;

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
            try
            {
                bool isPending = _socket.SendAsync(_sendArgc);
                if (isPending == false)
                {
                    OnSendCompleted(null, _sendArgc);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"RegisterSend() Failed {e}");
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
            if (_disconnected == 1)
                return;

            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgc.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool isPending = _socket.ReceiveAsync(_recvArgc);
                if (isPending == false)
                {
                    OnRecvCompleted(null, _recvArgc);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"RegisterRecv() Failed {e}");
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
 