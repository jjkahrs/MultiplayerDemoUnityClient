using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRemoteActorEvent : GameEvent
{
    public string id;

    public SpawnRemoteActorEvent( string inId )
    {
        this.id = inId;
    }
}