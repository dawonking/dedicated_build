using NetworkCore;
using System;
using System.Collections.Generic;
using UnityEngine;

// ���ӹ濡�� ����, ������, ���� ���� �̺�Ʈ�� ó���ϴ� Ŭ����

public class GameRoomManager : MonoBehaviour, IJobQueue
{
    static GameRoomManager _instance = new GameRoomManager();
    public static GameRoomManager Instance { get { return _instance; } }


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
        foreach (playerSession s in _playerSession)
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

        setM.Instance.Msg($"ID : {_session.SessionId} Connect");

        if (_playerSession.Count == 8)
        {
            setM.Instance.Msg("Game_Start");
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

    public void Move(playerSession session, Move packet)
    {
        //�÷��̾���ǥ�� ��Ŷ ��ǥ��
        session.posX = packet.posX;
        session.posY = packet.posY;
        session.posZ = packet.posZ;

        //���� �ٸ� �÷��̾�鿡�� ����




    }

}
