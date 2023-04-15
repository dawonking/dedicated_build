using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;


namespace NetworkCore
{

    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;

        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            int packetCount = 0;

            while (true)
            {
                //헤더사이즈보다 큰지
                if (buffer.Count < HeaderSize)
                {
                    break;
                }

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                {
                    break;
                }

                //여기서 부터 패킷조립 가능
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, dataSize);

            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);

    }


    //클라이언트별 세션 
    //
    public abstract class Session
    {
        //클라이언트별 세션 아이디
        public int SessionId { get; set; }
        int connectcheck = 0;
        object _lock = new object();
        

        #region tcp소켓 관련
        Socket _tcpsocket;        
        SocketAsyncEventArgs _tcpsendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _tcprecvArgs = new SocketAsyncEventArgs();
        Queue<ArraySegment<byte>> _tcpsendQue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _tcppendingList = new List<ArraySegment<byte>>();
        RecvBuffer _tcprecvBuffer = new RecvBuffer(65535);

        #endregion

        #region udp소켓 관련

        Socket _udpsocket;
        SocketAsyncEventArgs _udpsendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _udprecvArgs = new SocketAsyncEventArgs();
        Queue<ArraySegment<byte>> _udpsendQue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _udppendingList = new List<ArraySegment<byte>>();
        RecvBuffer _udprecvBuffer = new RecvBuffer(65535);

        #endregion

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);



        public void sessionStart(Socket tcpsocket, Socket udpsocket)
        {
            _tcpsocket = tcpsocket;
            _udpsocket = udpsocket;

            _tcpsendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OntcpsendCompleted);
            _tcprecvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OntcpRecvCompleted);

            _udprecvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnudpRecvCompleted);
            _udpsendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnudpsendCompleted);

            //Todo
            //RegisterRecv -> 기존 tcp는 그대로 처리하되, udp에서는 어떻게 할지?
            //tcp에서는 커넥션과 채팅? 만 처리하고, udp에서는 게임에서의 움직임을 처리한다.
            RegistertcpRecv();
            RegisterudpRecv();
        }
       
        public void Disconnect()
        {

        }

        
              
        #region TCP , 초기 연결시 확인

        void RegistertcpRecv()
        {
            if (connectcheck == 1)
            {
                return;
            }

            _tcprecvBuffer.Clean();

            ArraySegment<byte> segment = _tcprecvBuffer.WriteSegment;
            _tcprecvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _tcpsocket.ReceiveAsync(_tcprecvArgs);
                if (pending == false)
                    OntcpRecvCompleted(null, _tcprecvArgs);
            }
            catch (Exception ex)
            {

            }

        }

        void OntcpRecvCompleted(object sender, SocketAsyncEventArgs arg)
        {
            if (arg.BytesTransferred != 0 && arg.SocketError == SocketError.Success)
            {
                try
                {

                }
                catch (Exception e)
                {

                }

            }


        }
        

        //방 진입
        void RegistertcpSend()
        {
            if (connectcheck == 1)
                return;

            while (_tcpsendQue.Count > 0)
            {
                ArraySegment<byte> buff = _tcpsendQue.Dequeue();
                _tcppendingList.Add(buff);
            }

            _tcpsendArgs.BufferList = _tcppendingList;

            try
            {
                bool pending = _tcpsocket.SendAsync(_tcpsendArgs);
                if (pending == false)
                    OntcpsendCompleted(null, _tcpsendArgs);
            }
            catch (Exception e)
            {
                
            }
        }


        void OntcpsendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _tcpsendArgs.BufferList = null;
                        _tcppendingList.Clear();

                        if (_tcpsendQue.Count > 0)
                            RegistertcpSend();
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    //Disconnect
                }


            }
        }



        #endregion

        #region UDP 플레이어,오브젝트 이동

        void RegisterudpRecv()
        {
            if (connectcheck == 1)
            {
                return;
            }

            _udprecvBuffer.Clean();

            ArraySegment<byte> segment = _udprecvBuffer.WriteSegment;
            _udprecvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = _udpsocket.ReceiveAsync(_udprecvArgs);
                if (pending == false)
                    OntcpRecvCompleted(null, _udprecvArgs);
            }
            catch (Exception e)
            {
                
            }
        }

        void OnudpRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    //버퍼 체크
                    if(_udprecvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        //연결끊기
                        return;
                    }

                    //처리 확인

                    int processLen = OnRecv(_udprecvBuffer.ReadSegment);

                    if (processLen <= 0 || _udprecvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // Read 커서 이동
                    if (_udprecvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }

                    RegisterudpRecv();
                }
                catch (Exception e) { 

                }
            }
        }

        void RegisterudpSend()
        {

        }

        void OnudpsendCompleted(object send, SocketAsyncEventArgs e)
        {

        }

        

        #endregion




    }

}
