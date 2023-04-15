using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkCore
{
    public class Listener : MonoBehaviour
    {
        Socket _tcplistenSocket;
        Socket _udplistenSocket;
        
        Func<Session> _sessionFunc;

        int maxPlayer = 8;

        int _sessionId = 0;

        public void Init(IPEndPoint endPoint , Func<Session> session)
        {            
            _tcplistenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _udplistenSocket = new Socket(endPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            //delegate등록
            _sessionFunc+= session;

            _tcplistenSocket.Bind(endPoint);
            _udplistenSocket.Bind(endPoint);

            _tcplistenSocket.Listen(8);

            for(int i =0; i < maxPlayer; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
                RegisterAccept(args);
            }

        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            //소켓초기화
            args.AcceptSocket = null;
            //_tcplistenSocket과 연결
            bool pending = _tcplistenSocket.AcceptAsync(args);
            if (!pending)
            {
                Debug.Log("11");

                OnAcceptCompleted(null, args);
            }
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success)
            {
                //등록된 delegate를 실행
                Session session = _sessionFunc.Invoke();
                session.sessionStart(_tcplistenSocket, _udplistenSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
        }

        public void Disconnet()
        {
            _tcplistenSocket.Close();
            _udplistenSocket.Close();
        }

    }
}

