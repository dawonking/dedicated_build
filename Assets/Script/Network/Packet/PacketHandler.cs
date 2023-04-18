using NetworkCore;

//총쏘기 , 메시지, 입장 관련 -> tcp
//플레이어 움직임, 아이템, 나머지 오브젝트 -> udp

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
        //as -> 하위객체 캐스팅
        Move movePacket = packet as Move;
        //여기서 playerSession을 가지고 있지않으면 다음조건문에서 걸림
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
