using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum PlayerState
{
    wait = 1,
    Search,
    Ready,
    Playing,
    End
}

public class Client
{
    public static int buffsize = 1024;

    public PlayerState playerState;

    public int id;
    public string name;
    public Tcp tcp;
    public Udp udp;

    public Client(int _clientid , string getname)
    {
        id = _clientid;
        name = getname;
        tcp = new Tcp(_clientid);
        udp = new Udp(_clientid);
        playerState = PlayerState.wait;
    }

    public class Tcp
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream netStream;        
        private byte[] receiveBuffer;

        public Tcp(int _id)
        {
            id = _id;

        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = buffsize;
            socket.SendBufferSize = buffsize;
            netStream = socket.GetStream();            
            receiveBuffer = new byte[buffsize];

            netStream.BeginRead(receiveBuffer, 0, buffsize, ReceiveCallback, null);

            //sendData

        }

        private void ReceiveCallback(IAsyncResult _result)
        {

        }

    }

    public class Udp
    {
        private int id;

        public Udp(int _id)
        {
            id = _id;    
        }
    }

}