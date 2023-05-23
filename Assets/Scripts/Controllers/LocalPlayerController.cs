using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerController : MonoBehaviour
{
    public GameObject ghost;
    private const int MAX_LOCAL_FRAMES = 10;
    private const float CORRECTION_FACTOR = 0.1f;
    private const float DRIFT_TOLERANCE = 0.05f;

    private GameMaster _GM;
    private bool isOnline = false;

    void Awake()
    {
        _GM = GameObject.FindObjectOfType<GameMaster>();
    }

    void Start()
    {
        _GM.Subscribe( typeof( StateChangeEvent ), OnStateChange, this );
    }

    void OnStateChange( GameEvent gameEvent )
    {
        if( gameEvent is StateChangeEvent sce )
        {
            if( sce.stateName == "OnlineState" )
            {
                isOnline = true;
                transform.position = _GM.GetGameStore().localActor.position;
            }
        }
    }

    void Update()
    {
        if( !isOnline )
            return;

        // Go ahead and optimistically move the local player
        transform.position += ( _GM.GetGameStore().localActor.headingNormal * _GM.GetGameStore().localActor.speed * Time.deltaTime);
        _GM.GetGameStore().localActor.position = transform.position;

        // If we have a new state update from the server, adjust the client position based on a deviation tolerance and
        // a correct factor.
        if( _GM.GetGameStore().lastFrameFromServer != null )
        {
            float driftDistance = Vector3.Distance( _GM.GetGameStore().lastFrameFromServer.localActor.position, transform.position);
            if( driftDistance > DRIFT_TOLERANCE )
            {
                transform.position = Vector3.MoveTowards( 
                    transform.position, 
                    _GM.GetGameStore().lastFrameFromServer.localActor.position, 
                    driftDistance * CORRECTION_FACTOR );
            }

            // Clear the last frame now that it's been evaluated.
            _GM.GetGameStore().lastFrameFromServer = null;
        }

        // Code to update the server "ghost" for testing purposes.
        if( _GM.GetGameStore().GetCurrentFrameSnapshot() != null )
            ghost.transform.position = _GM.GetGameStore().GetCurrentFrameSnapshot().localActor.position;
    }

    void OnDestroy()
    {
        _GM?.UnsubscribeAll( this );
    }
}
