using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRemoteActorEvent : GameEvent
{
    public string id;

    public RemoveRemoteActorEvent( string inId )
    {
        this.id = inId;
    }
}