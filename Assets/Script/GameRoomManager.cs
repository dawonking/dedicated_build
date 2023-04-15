using NetworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 게임방에서 입장, 움직임, 퇴장 등의 이벤트를 처리하는 클래스

public class GameRoomManager : MonoBehaviour ,IJobQueue
{

    JobQue _jobQue = new JobQue();
    List<playerSession> _playerSession = new List<playerSession>();
    List<ArraySegment<byte>> quelist = new List<ArraySegment<byte>>();


    public void Push(Action job)
    {
        _jobQue.Push(job);
    }

    //NetWortManger의 FixedManager에서 사용
    public void Flush()
    {
        foreach(playerSession s in _playerSession)
        {
            //s.Send(quelist);
        }
    }


    //tcp만사용
    public void EnterGame(playerSession _session)
    {
        //초기 연결시 추가
        _playerSession.Add(_session);
        _session.roomManager = this;

        if(_playerSession.Count == 8)
        {
            //gameStart

        }

    }

    //Enter, Move에서 마지막에 호출
    //quelist에 삽입
    void BroadCast(ArraySegment<byte> arr)
    {
        quelist.Add(arr);
    }

    void gameCounter()
    {

    }

    public void Move(playerSession session , C_Move packet)
    {
        //플레이어좌표를 패킷 좌표로
        session.posX = packet.posX;
        session.posY = packet.posY;
        session.posZ = packet.posZ;

        //이후 다른 플레이어들에게 전달


        

    }

}
