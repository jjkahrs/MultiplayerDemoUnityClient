using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteActorController : MonoBehaviour
{
    [SerializeField] private GameObject remotePrefab;
    private GameMaster _GM;
    private float tickTimer = 0;
    private float lerpTimer = 0;
    private float lerpStartTime = 0;

    private float duration;
    private long lastFrameRendered;

    private Dictionary<string,GameObject> remoteActors = new Dictionary<string,GameObject>();
    private FrameSnapshot previousFrame;
    private FrameSnapshot currentFrame;


    private float localTickInterval;

    void Awake()
    {
        _GM = GameObject.FindObjectOfType<GameMaster>();
    }

    void Start()
    {
        _GM.Subscribe( typeof( StateChangeEvent ), OnStateChange, this );
        _GM.Subscribe( typeof( SpawnRemoteActorEvent ), OnSpawnRemoteActor, this );
        _GM.Subscribe( typeof( RemoveRemoteActorEvent ), OnRemoveRemoteActor, this );
        lastFrameRendered = -1;
        localTickInterval = GameStore.STATE_UPDATE_INTERVAL;
    }

    void OnRemoveRemoteActor( GameEvent gameEvent )
    {
        if( gameEvent is RemoveRemoteActorEvent removeEvent )
        {
            GameObject obj = remoteActors[ removeEvent.id ];
            remoteActors.Remove( removeEvent.id );
            GameObject.Destroy( obj );
        }
    }

    void OnSpawnRemoteActor( GameEvent gameEvent )
    {
        if( gameEvent is SpawnRemoteActorEvent spawnEvent )
        {
            GameObject obj = Instantiate( remotePrefab, this.transform );
            remoteActors.Add( spawnEvent.id, obj );
        }
    }

    void OnStateChange( GameEvent gameEvent )
    {
        if( gameEvent is StateChangeEvent sce )
        {
            if( sce.stateName == "OnlineState" )
            {
                ResetAllActorsToDefaultPosition();
            }
        }
    }

    private void ResetAllActorsToDefaultPosition()
    {
        if(_GM.GetGameStore().remoteActors.Count == 0 )
            return;
            
        foreach( Actor actor in _GM.GetGameStore().remoteActors)
        {
            GameObject obj;
            if( !remoteActors.TryGetValue( actor.id, out obj) )
            {
                // Unknown remote actor. We should do something about this.
                continue;
            }

            obj.transform.position = actor.position;
        }
    }

    void Update()
    {
        // Lerp between the previous frame and the most recent frame from the server.

        tickTimer += Time.deltaTime;

        if( tickTimer >= localTickInterval )
        {
            tickTimer = 0;
            previousFrame = _GM.GetGameStore().GetPreviousFrameSnapshot();
            currentFrame = _GM.GetGameStore().GetCurrentFrameSnapshot();

            if( previousFrame == null || currentFrame.tick <= lastFrameRendered )
            {
                return;
            }

            lastFrameRendered = currentFrame.tick;
            lerpTimer = 0;
            lerpStartTime = Time.time;
            duration = currentFrame.tickInterval;
            localTickInterval = currentFrame.tickInterval;
        }
        
        if( currentFrame != null && previousFrame != null )
        {
            foreach( Actor previousActor in previousFrame.remoteActors )
            {
                float distance = 0f;

                Actor currentActor = currentFrame.FindRemoteActorById( previousActor.id );
                if( currentActor == null )
                    continue;

                Vector3 currentPosition = currentActor.position;
                Vector3 previousPosition = previousActor.position;

                GameObject obj;
                if(! remoteActors.TryGetValue( previousActor.id, out obj ) )
                    continue;

                // Adjust Facing
                Vector3 newFacing = Vector3.ProjectOnPlane( previousActor.facing, Vector3.up );
                obj.transform.rotation =  Quaternion.LookRotation( newFacing );

                distance = Vector3.Distance( obj.transform.position, currentPosition );

                if( distance != 0 )
                {
                    float timeLeft = lerpTimer / duration; 
                    Vector3 newPosition = Vector3.Lerp( previousPosition, currentPosition, timeLeft );

                    lerpTimer += Time.deltaTime;
                    obj.transform.position = newPosition;
                }
            }
        }
    }

    void OnDestroy()
    {
        _GM?.UnsubscribeAll( this );
    }
}
