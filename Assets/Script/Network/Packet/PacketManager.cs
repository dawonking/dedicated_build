using NetworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//playerSession�� OnRecvPacket���� ȣ��
public class PacketManager : MonoBehaviour
{
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }

    PacketManager()
    {
        Register();
    }

    public void Register()
    {
        //Dictionary�� ����� �͵� �̵� ���� 
    }

    public void OnRecvPacket(PacketSession session , ArraySegment<byte> buffer , Action<PacketSession , IPacket> OnRecvCallback = null)
    {
        packetRead(buffer);
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

        id = BitConverter.ToInt32(segment.Array, segment.Offset + count);
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
