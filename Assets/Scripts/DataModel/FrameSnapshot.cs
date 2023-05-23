using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameSnapshot
{
    public long tick;
    public long timestamp;
    public float tickInterval;
    public long transitTimeMillis;
    public Actor localActor;
    public List<Actor> remoteActors = new List<Actor>();

    public Actor FindRemoteActorById( string id )
    {
        foreach( Actor actor in remoteActors )
        {
            if( actor.id == id )
                return actor;
        }
        
        return null;
    }
}