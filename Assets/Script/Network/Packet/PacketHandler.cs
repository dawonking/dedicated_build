using NetworkCore;

//�ѽ�� , �޽���, ���� ���� -> tcp
//�÷��̾� ������, ������, ������ ������Ʈ -> udp

public class PacketHandler
{
    public static void shootGun(PacketSession session , IPacket packet)
    {
        
    }

    public static void MessageRecv(PacketSession session , IPacket packet)
    {

    }    


    public static void playerMoveHandle(PacketSession session, IPacket packet)
    {
        //as -> ������ü ĳ����
        Move movePacket = packet as Move;
        //���⼭ playerSession�� ������ ���������� �������ǹ����� �ɸ�
        playerSession playerSession = session as playerSession;

        if (playerSession.roomManager != null)
        {
            return;
        }


        GameRoomManager roomManager = playerSession.roomManager;

        roomManager.Push(() => roomManager.Move(playerSession, movePacket));
    }

    public static void objMoveHandle(PacketSession session, IPacket packet)
    {

    }



}
