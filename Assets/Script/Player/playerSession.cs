using NetworkCore;
using System;
using System.Net;
using UnityEngine;

public class playerSession : PacketSession
{
    public int SessionId { get; set; }
    public GameRoomManager roomManager { get; set; }
    public float posX { get; set; }
    public float posY { get; set; }
    public float posZ { get; set; }

    public override void OnConnected(EndPoint endPoint)
    {
        GameRoomManager.Instance.EnterGame(this);
    }

    public override void OnDisconnected(EndPoint endPoint)
    {

    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        //РќДо
        //setM setM = new setM();
        //setM.Msg(buffer);
        PacketManager.Instance.OnRecvPacket(this, buffer);
    }

    public override void OnSend(int numOfBytes)
    {

    }
}


