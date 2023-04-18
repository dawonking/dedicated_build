using NetworkCore;
using System;
using System.Net;
using UnityEngine;

/*
 전송관련
 update - 
 LateUpdate - 전송 x 불일치 확률이 높음, 렌더링 관련 연산
 FixedUpdate - 물리 관련 연산, 캐릭터의 움직임이나 오브젝트 상호관계에 따른 결과

 */


public class NetworkManager : MonoBehaviour
{
    static NetworkManager _instance;
    public static NetworkManager Instance { get { return _instance; } }


    static Listener _listener = new Listener();
    GameRoomManager roomManager = new GameRoomManager();
    float _lastTickTime = 0.0f;



    private void Awake()
    {

    }


    private void Start()
    {
        //물리 기반 업데이트 발생 주기
        //즉 0.02초 마다 FixedTimeStep값을 의미한다.
        Time.fixedDeltaTime = 0.02f;
        //Time.fixedDeltaTime = 1.0f / 60.0f; 
        Time.maximumDeltaTime = 0.1f;
        //최대 프레임율 제한
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
        //물리연산 전송
        roomManager.Push(() => roomManager.Flush());


    }

    void Update()
    {
        //여기서 결과 전송?
        //위의 물리연산에 대한 처리결과에 따른 경기 결과 전송
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




public class setM : MonoBehaviour
{

    static setM _instance = new setM();
    public static setM Instance { get { return _instance; } }



    public void Msg(string msg)
    {
        Debug.Log(msg);
    }

    public void Msg(ArraySegment<byte> buffer)
    {
        Debug.Log(buffer.Count);

        for (int i = 0; i < buffer.Count; i++)
        {
            Debug.Log(i + " = " + buffer[i]);
        }

    }
}