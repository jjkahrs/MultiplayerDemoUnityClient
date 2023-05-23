using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionConnectEvent : GameEvent
{
    public string sessionId;

    public SessionConnectEvent( string sessionId )
    {
        this.sessionId = sessionId;
    }
}