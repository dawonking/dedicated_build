using NetworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public enum PacketID
{
    Connect = 1,
    C_Move = 2,
    C_playerSpawn = 3,
    C_itemSpawn = 4,
    S_BroadcastMove = 5
}

public interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);

    ArraySegment<byte> Write();
}

//public class Connect : IPacket
//{
//    public ushort Protocol { get { return (ushort)PacketID.Connect; } }

//    public void Read(ArraySegment<byte> segment)
//    {
        
//    }

//    public ArraySegment<byte> Write()
//    {
        
//    }
//}

public class S_BroadcastMove : IPacket
{
    public int playerId;
    public float posX;
    public float posY;
    public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.S_BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + count);
        count += sizeof(int);
        this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);
        this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);
        this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);
    }

    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.playerId), 0, segment.Array, segment.Offset + count, sizeof(int));
        count += sizeof(int);
        Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
        count += sizeof(float);
        Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
        count += sizeof(float);
        Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
        count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}


public class C_Move : IPacket
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;


    public ushort Protocol { get { return (ushort)PacketID.C_Move; } }
    public void Read(ArraySegment<byte> segment)
    {
        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);
        this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);
        this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + count);
        count += sizeof(float);
    }


    public ArraySegment<byte> Write()
    {
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;

        //C_Move만큼 입력
        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));

        count += sizeof(ushort);
        Array.Copy(BitConverter.GetBytes(this.posX), 0, segment.Array, segment.Offset + count, sizeof(float));
        count += sizeof(float);
        Array.Copy(BitConverter.GetBytes(this.posY), 0, segment.Array, segment.Offset + count, sizeof(float));
        count += sizeof(float);
        Array.Copy(BitConverter.GetBytes(this.posZ), 0, segment.Array, segment.Offset + count, sizeof(float));
        count += sizeof(float);

        Array.Copy(BitConverter.GetBytes(count), 0, segment.Array, segment.Offset, sizeof(ushort));

        return SendBufferHelper.Close(count);
    }
}   

