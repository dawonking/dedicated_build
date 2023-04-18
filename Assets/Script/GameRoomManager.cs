using NetworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ӹ濡�� ����, ������, ���� ���� �̺�Ʈ�� ó���ϴ� Ŭ����

public class GameRoomManager : MonoBehaviour ,IJobQueue
{

    JobQue _jobQue = new JobQue();
    List<playerSession> _playerSession = new List<playerSession>();
    List<ArraySegment<byte>> quelist = new List<ArraySegment<byte>>();


    public void Push(Action job)
    {
        _jobQue.Push(job);
    }

    //NetWortManger�� FixedManager���� ���
    public void Flush()
    {
        foreach(playerSession s in _playerSession)
        {
            //s.Send(quelist);
        }
    }


    //tcp�����
    public void EnterGame(playerSession _session)
    {
        //�ʱ� ����� �߰�
        _playerSession.Add(_session);
        _session.roomManager = this;

        if(_playerSession.Count == 8)
        {
            //gameStart

        }

    }

    //Enter, Move���� �������� ȣ��
    //quelist�� ����
    void BroadCast(ArraySegment<byte> arr)
    {
        quelist.Add(arr);
    }

    void gameCounter()
    {

    }

    public void Move(playerSession session , C_Move packet)
    {
        //�÷��̾���ǥ�� ��Ŷ ��ǥ��
        session.posX = packet.posX;
        session.posY = packet.posY;
        session.posZ = packet.posZ;

        //���� �ٸ� �÷��̾�鿡�� ����


        

    }

}
