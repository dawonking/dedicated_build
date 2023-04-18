using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using NetworkCore;

/*
 ���۰���
 update - 
 LateUpdate - ���� x ����ġ Ȯ���� ����, ������ ���� ����
 FixedUpdate - ���� ���� ����, ĳ������ �������̳� ������Ʈ ��ȣ���迡 ���� ���

 */


public class NetworkManager : MonoBehaviour
{   
    static Listener _listener = new Listener();
    GameRoomManager roomManager = new GameRoomManager();
    float _lastTickTime = 0.0f;

    private void Awake()
    {
        
    }


    private void Start()
    {
        //���� ��� ������Ʈ �߻� �ֱ�
        //�� 0.02�� ���� FixedTimeStep���� �ǹ��Ѵ�.
        Time.fixedDeltaTime = 0.02f;
        //Time.fixedDeltaTime = 1.0f / 60.0f; 
        Time.maximumDeltaTime = 0.1f;
        //�ִ� �������� ����
        Application.targetFrameRate = 60;

        // Disable VSync for dedicated servers
        QualitySettings.vSyncCount = 0;

        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 10001);


        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });   
    }

    private void FixedUpdate()
    {
        //Debug.Log(Time.fixedDeltaTime);
        //�������� ����
        roomManager.Push(() => roomManager.Flush());


    }

    void Update()
    {
        //���⼭ ��� ����?
        //���� �������꿡 ���� ó������� ���� ��� ��� ����
    }

    private void LateUpdate()
    {
        
    }

    public void Message(object msg)
    {
        Debug.Log(msg.ToString());
    }


    private void OnApplicationQuit()
    {
        _listener.Disconnet();
    }

}
