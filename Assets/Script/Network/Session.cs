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
                //���������� ū��
                if (buffer.Count < HeaderSize)
                {
                    break;
                }

                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                {
                    break;
                }

                //���⼭ ���� ��Ŷ���� ����
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, dataSize);

            }

            return processLen;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);

    }


    //Ŭ���̾�Ʈ�� ���� 
    //
    public abstract class Session
    {
        //Ŭ���̾�Ʈ�� ���� ���̵�
        public int SessionId { get; set; }
        int connectcheck = 0;
        object _lock = new object();
        

        #region tcp���� ����
        Socket _tcpsocket;        
        SocketAsyncEventArgs _tcpsendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _tcprecvArgs = new SocketAsyncEventArgs();
        Queue<ArraySegment<byte>> _tcpsendQue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _tcppendingList = new List<ArraySegment<byte>>();
        RecvBuffer _tcprecvBuffer = new RecvBuffer(65535);

        #endregion

        #region udp���� ����

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
            //RegisterRecv -> ���� tcp�� �״�� ó���ϵ�, udp������ ��� ����?
            //tcp������ Ŀ�ؼǰ� ä��? �� ó���ϰ�, udp������ ���ӿ����� �������� ó���Ѵ�.
            RegistertcpRecv();
            RegisterudpRecv();
        }
       
        public void Disconnect()
        {

        }

        
              
        #region TCP , �ʱ� ����� Ȯ��

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
        

        //�� ����
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

        #region UDP �÷��̾�,������Ʈ �̵�

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
                    //���� üũ
                    if(_udprecvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        //�������
                        return;
                    }

                    //ó�� Ȯ��

                    int processLen = OnRecv(_udprecvBuffer.ReadSegment);

                    if (processLen <= 0 || _udprecvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // Read Ŀ�� �̵�
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
