using NetworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;


//playerSession의 OnRecvPacket에서 호출
public class PacketManager : MonoBehaviour
{
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }

    Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>> _makeFunc = new Dictionary<ushort, Func<PacketSession, ArraySegment<byte>, IPacket>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();

    PacketManager()
    {
        Register();
    }

    public void Register()
    {
        //Dictionary에 사용할 것들 이동 생성 
        //총쏘기 , 메시지, 입장 관련 -> tcp
        //플레이어 움직임, 아이템, 나머지 오브젝트 -> udp

        //_makeFunc에서 id값을 통해 패킷이 어떤 역할을 하는지 찾은뒤
        //반환된 값을 _handler에서 사용

        _makeFunc.Add((ushort)PacketID.Move, MakePacket<Move>);
        _makeFunc.Add((ushort)PacketID.objMove, MakePacket<objMove>);
        _makeFunc.Add((ushort)PacketID.shot, MakePacket<shot>);

        _handler.Add((ushort)PacketID.Move, PacketHandler.playerMoveHandle);
        _handler.Add((ushort)PacketID.objMove, PacketHandler.objMoveHandle);
        _handler.Add((ushort)PacketID.shot, PacketHandler.shootGun);
        
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer, Action<PacketSession, IPacket> OnRecvCallback = null)
    {        
        ushort count = 0;
        //버퍼 크기
        ushort size = 0;
        //패킷ID
        ushort id = 0;

        //packetRead에 id와 size 부분은 여기로, 나머지는 Packet의 Read,Write쪽으로
        packetRead(buffer);

        //패킷id를 통해 Dic에서 찾아 func로 반환               
        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;

        if(_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);

            //사전에 받은 콜백이 있는지
            if(OnRecvCallback != null)
            {
                OnRecvCallback.Invoke(session, packet);
            }
            else
            {
                HandlePacket(session, packet);
            }

        }

    }

    T MakePacket<T>(PacketSession session , ArraySegment<byte> buffer) where T : IPacket , new()
    {
        T pkt = new T();
        pkt.Read(buffer);
        return pkt;
    }

    public void HandlePacket(PacketSession session , IPacket packet)
    {
        Action<PacketSession, IPacket> action = null;
        if(_handler.TryGetValue(packet.Protocol, out action))
        {
            action.Invoke(session, packet);
        }

    }

    //분석 임시 작성
    public void packetRead(ArraySegment<byte> segment)
    {
        int id = 0;
        int packetnum = 0;
        ushort count = 0;

        float x = 0, y = 0, z = 0;

        bool jump = false;

        count += sizeof(ushort);

        packetnum = BitConverter.ToInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        id = BitConverter.ToInt16(segment.Array, segment.Offset + count);
        count += sizeof(ushort);

        x = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);

        y = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);

        z = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);

        jump = BitConverter.ToBoolean(segment.Array, segment.Offset + count);
        count += sizeof(bool);

        Debug.Log($"id = {id} , packetNum = {packetnum} / {x} {y} {z}  / {jump}");
    }


}
