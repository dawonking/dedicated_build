using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkCore
{
    public class Server
    {
        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public static Dictionary<int, Client> playerDiction = new Dictionary<int, Client>();

        int Port = 10001;

        public static int maxPlay = 30;

        void ServerStart(int MaxPlayer = 30)
        {
            Init();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            //BeginAcceptTcpClient -> tcp비동기 작업
            tcpListener.BeginAcceptTcpClient(tcpConnectCallback, null);


            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(udpReceiveCallback, null);
            Debug.Log("Server_Start Port = " + Port);
            

        }

        private static void tcpConnectCallback(IAsyncResult _reslut)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_reslut);
            tcpListener.BeginAcceptTcpClient(tcpConnectCallback, null);

            for(int i =1; i<= maxPlay; i++)
            {
                if(playerDiction[i].tcp.socket == null)
                {
                    playerDiction[i].tcp.Connect(_client);
                    return;
                }
            }

            Debug.Log("Connection_error");
        }

        private static void udpReceiveCallback(IAsyncResult _result)
        {
            try
            {
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(udpReceiveCallback, null);


                //fix
                if (_data.Length < 4)
                {
                    return;
                }
            }
            catch(Exception e)
            {
                Debug.Log(e.ToString());
            }

        }

        private void Init()
        {
            for (int i = 1; i <= maxPlay; i++)
            {
                playerDiction.Add(i, new Client(i,""));
            }
        }

    }
}