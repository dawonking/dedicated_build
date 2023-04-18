using NetworkCore;
using System;

public enum PacketID
{
    Connect = 1,
    Move = 2,
    shot = 3,

    itemSpawn = 4,
    objMove = 5,
    BroadcastMove = 6,

    Message = 7

}

public interface IPacket
{
    ushort Protocol { get; }
    void Read(ArraySegment<byte> segment);

    ArraySegment<byte> Write();
}


public class BroadcastMove : IPacket
{
    public int playerId;
    public float posX;
    public float posY;
    public float posZ;

    public ushort Protocol { get { return (ushort)PacketID.BroadcastMove; } }

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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.BroadcastMove), 0, segment.Array, segment.Offset + count, sizeof(ushort));
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


public class Move : IPacket
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;


    public ushort Protocol { get { return (ushort)PacketID.Move; } }
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
        Array.Copy(BitConverter.GetBytes((ushort)PacketID.Move), 0, segment.Array, segment.Offset + count, sizeof(ushort));

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

public class shot : IPacket
{
    public ushort Protocol { get { return (ushort)PacketID.Move; } }

    public void Read(ArraySegment<byte> segment)
    {

    }

    public ArraySegment<byte> Write()
    {
        throw new NotImplementedException();
    }
}

public class itemSpawn : IPacket
{
    public ushort Protocol { get { return (ushort)PacketID.Move; } }

    public void Read(ArraySegment<byte> segment)
    {

    }

    public ArraySegment<byte> Write()
    {
        throw new NotImplementedException();
    }
}

public class objMove : IPacket
{
    public ushort Protocol { get { return (ushort)PacketID.Move; } }

    public void Read(ArraySegment<byte> segment)
    {

    }

    public ArraySegment<byte> Write()
    {
        throw new NotImplementedException();
    }
}

public class Message : IPacket
{
    public ushort Protocol { get { return (ushort)PacketID.Move; } }

    public void Read(ArraySegment<byte> segment)
    {

    }

    public ArraySegment<byte> Write()
    {
        throw new NotImplementedException();
    }
}
