using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using NetworkCore;
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

public class setM : MonoBehaviour
{
    public void Msg(string msg)
    {
        Debug.Log(msg);
    }

    public void Msg(ArraySegment<byte> buffer) 
    {
        Debug.Log(buffer.Count);

        for(int i = 0; i< buffer.Count; i++)
        {
            Debug.Log(i + " = " + buffer[i]);
        }
        
    }
}

