using NetworkCore;
using System.Collections;
using System.Collections.Generic;

public class PacketHandler 
{
    public static void C_MoveHandler(PacketSession session, IPacket packet)
    {
        C_Move movePacket = packet as C_Move;
        playerSession playerSession = session as playerSession;

        if(playerSession.roomManager != null)
        {
            return;
        }


        GameRoomManager roomManager = playerSession.roomManager;

        roomManager.Push(() => roomManager.Move(playerSession, movePacket));


    }


}
