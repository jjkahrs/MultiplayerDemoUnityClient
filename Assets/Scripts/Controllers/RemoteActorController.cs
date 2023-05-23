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

    // private Vector3 previousPosition = Vector3.zero;
    // private Vector3 currentPosition = Vector3.zero;
    // private float distance;
    // private float currentSpeed;
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
        lastFrameRendered = -1;
        localTickInterval = GameStore.STATE_UPDATE_INTERVAL;
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
        // NOTE: This is temporary. We're testing this before moving this logic to remote Actor controllers
        // Lerp between the previous frame and the most recent frame from the server.
        //this.transform.position = _GM.GetGameStore().localActor.position;

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

            // Normally we'd look up Actor based on ID. Here we know we're the local player
            lastFrameRendered = currentFrame.tick;
            lerpTimer = 0;
            lerpStartTime = Time.time;
            //previousPosition = previousFrame.localActor.position;
            //currentPosition = currentFrame.localActor.position;
            //currentSpeed = previousFrame.localActor.speed;
            duration = currentFrame.tickInterval;
            localTickInterval = currentFrame.tickInterval;

            //Debug.Log($"TBF={currentFrame.timestamp - previousFrame.timestamp} position={transform.position}  {currentFrame.transitTimeMillis} {currentFrame.tick} currentPosition={currentPosition}  {previousFrame.transitTimeMillis} {previousFrame.tick} previousPosition={previousPosition}");
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

                //if( obj.transform.position != Vector3.zero && currentPosition != Vector3.zero )
                    distance = Vector3.Distance( obj.transform.position, currentPosition );

                if( distance != 0 )
                {
                    float timeLeft = lerpTimer / duration; 
                    Vector3 newPosition = Vector3.Lerp( previousPosition, currentPosition, timeLeft );

                    lerpTimer += Time.deltaTime;
                    obj.transform.position = newPosition;
                    //Debug.Log($"@@@ Distance Remaining {Vector3.Distance( transform.position, currentPosition )}");
                }
            }
        }
    }

    void OnDestroy()
    {
        _GM?.UnsubscribeAll( this );
    }
}
