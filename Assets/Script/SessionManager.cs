using NetworkCore;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// 클라이언트 세션들을 관리
/// </summary>
public class SessionManager 
{
    static SessionManager _sessionManager = new SessionManager();
    public static SessionManager Instance
    {
        get
        {
            return _sessionManager;
        }
    }
    int _sessinId = 0;
    object _lock = new object();
    Dictionary<int, playerSession> _sessions = new Dictionary<int, playerSession>();

    public Session Generate()
    {
        lock (_lock)
        {
            //호출시 session생성
            playerSession session = new playerSession();
            session.SessionId = _sessinId;
            //_sessions 리스트에 등록
            _sessions.Add(session.SessionId, session);
            _sessinId++;
            return session;
        }
    }

    public void Remove(Session session)
    {
        lock (_lock)
        {            
            _sessions.Remove(session.SessionId);
        }
    }



}
