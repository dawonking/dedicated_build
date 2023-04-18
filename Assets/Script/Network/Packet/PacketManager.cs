using NetworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;


//playerSession�� OnRecvPacket���� ȣ��
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
        //Dictionary�� ����� �͵� �̵� ���� 
        //�ѽ�� , �޽���, ���� ���� -> tcp
        //�÷��̾� ������, ������, ������ ������Ʈ -> udp

        //_makeFunc���� id���� ���� ��Ŷ�� � ������ �ϴ��� ã����
        //��ȯ�� ���� _handler���� ���

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
        //���� ũ��
        ushort size = 0;
        //��ŶID
        ushort id = 0;

        //packetRead�� id�� size �κ��� �����, �������� Packet�� Read,Write������
        packetRead(buffer);

        //��Ŷid�� ���� Dic���� ã�� func�� ��ȯ               
        Func<PacketSession, ArraySegment<byte>, IPacket> func = null;

        if(_makeFunc.TryGetValue(id, out func))
        {
            IPacket packet = func.Invoke(session, buffer);

            //������ ���� �ݹ��� �ִ���
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

    //�м� �ӽ� �ۼ�
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
