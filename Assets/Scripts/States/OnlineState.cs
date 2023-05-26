using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class OnlineState : GameState
{
    private const float INPUT_SEND_INTERVAL = 0.1f;
    private const string HEADING_COMMAND = "Heading";
    private const string WORLD_DELTA_MESSAGE_TYPE = "WorldDelta";
    private const string NEW_REMOTE_ACTOR_MESSAGE_TYPE = "NewRemoteActor";
    private const string REMOVE_REMOTE_ACTOR_MESSAGE_TYPE = "RemoveRemoteActor";

    private NetworkManager _NetworkManager;
    private float inputTimer;
    private float stateTimer;

    public override void Init()
    {
        Debug.Log("OnlineState.Init()");
        _NetworkManager = GameObject.FindObjectOfType<NetworkManager>();
        _GM.Subscribe( typeof( KeyboardEvent ), OnKeyboard, this );
        inputTimer = 0f;
        stateTimer = 0f;
        _GM.GetGameStore().gameTick = 0;
    }

    private void OnKeyboard( GameEvent gameEvent )
    {

        if( gameEvent is KeyboardEvent ke )
        {
            if( ke.keyCode == KeyCode.W )
                _GM.GetGameStore().localActor.headingNormal += ke.isDown ? Vector3.forward : Vector3.back;
            else if( ke.keyCode == KeyCode.S )
                _GM.GetGameStore().localActor.headingNormal += ke.isDown ? Vector3.back : Vector3.forward;
            else if( ke.keyCode == KeyCode.A )
                _GM.GetGameStore().localActor.headingNormal += ke.isDown ? Vector3.left : Vector3.right;
            else if( ke.keyCode == KeyCode.D )
                _GM.GetGameStore().localActor.headingNormal += ke.isDown ? Vector3.right : Vector3.left;
        }
    }

    public override void OnFrame()
    {
        inputTimer += Time.deltaTime;

            PlayerInputFrame inputFrame = new PlayerInputFrame();
            inputFrame.inputHeading = _GM.GetGameStore().localActor.headingNormal;
            inputFrame.inputSpeed = _GM.GetGameStore().localActor.speed;
            inputFrame.inputTick = _GM.GetGameStore().gameTick;
            inputFrame.inputPosition = _GM.GetGameStore().localActor.position;
            inputFrame.inputDuration = Convert.ToInt64(Time.deltaTime * 1000);

            long timestamp = _GM.GetGameStore().AddPlayerInputFrame( inputFrame ).inputTimestamp;

        if( inputTimer >= INPUT_SEND_INTERVAL )
        {
            inputTimer = 0f;
            _GM.GetGameStore().gameTick++;

            PlayerInputFrame totalFrame = new PlayerInputFrame();
            Vector3 direction = Vector3.zero;
            long totalDeltaTime = 0;

            foreach( PlayerInputFrame frame in _GM.GetGameStore().playerInputFrames )
            {
                direction += frame.inputHeading;
                totalDeltaTime += frame.inputDuration;
            }
            
            long duration = totalDeltaTime;
            inputFrame.inputDuration = duration;

            string message = inputFrame.BuildSendMessage();
            _GM.GetGameStore().playerInputFrames.Clear();

            Debug.Log($"===> {message}");
            _NetworkManager.Send( message );
        }

        stateTimer += Time.deltaTime;

        if( stateTimer >= GameStore.STATE_UPDATE_INTERVAL )
        {
            // Send the current state if it has changed.
            stateTimer = 0f;
        }

        if( _NetworkManager.HasPendingData() )
        {
            List<string> messages = _NetworkManager.ReadMessages();

            foreach( string data in messages )
            {
                if( data == "")
                    continue;

                Debug.Log($"<=== Online State data: {data}");

                string[] parts = data.Split("|");
                long msgTimestamp = long.Parse(parts[0]);
                string msgType = parts[1];

                switch( msgType )
                {
                    case WORLD_DELTA_MESSAGE_TYPE:
                        UpdateWorldState( data );
                        break;
                    case NEW_REMOTE_ACTOR_MESSAGE_TYPE:
                        SpawnNewRemoteActor( parts[2] );
                        break;
                    case REMOVE_REMOTE_ACTOR_MESSAGE_TYPE:
                        RemoveRemoteActor( parts[2] );
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void RemoveRemoteActor( string payload )
    {
        string[] keyVal = payload.Split(':');
        _GM.DispatchEvent( new RemoveRemoteActorEvent( keyVal[1] ) );
    }

    private void SpawnNewRemoteActor( string payload )
    {
        Actor actor = Actor.Parse( payload );
        _GM.DispatchEvent( new SpawnRemoteActorEvent( actor.id ) );
    }

    private void UpdateWorldState( string data )
    {
        string[] parts = data.Split("|");
        long timestamp = long.Parse(parts[0]);
        string msgType = parts[1];
        float tickInterval = float.Parse(parts[2]);
        long tick = Convert.ToInt64( parts[3] );
        string payload = (parts.Length == 5) ? parts[4] : "";

        long latency = Utils.CurrentTimeMillis() - timestamp;

        FrameSnapshot frameSnapshot = new FrameSnapshot();
        frameSnapshot.tick = tick;
        frameSnapshot.timestamp = timestamp;
        frameSnapshot.tickInterval = tickInterval;
        string[] segments = payload.Split("^");
        
        for( int i = 0; i < segments.Length; i++ )
        {
            string segment = segments[i];
            Actor actor = Actor.Parse( segment );

            if( actor.id == _GM.GetGameStore().localSessionId )
            {
                frameSnapshot.localActor = actor;
            }
            else
            {
                frameSnapshot.remoteActors.Add( actor );                
            }
        }

        _GM.GetGameStore().AddFrameSnapshot( frameSnapshot );
        _GM.GetGameStore().lastFrameFromServer = frameSnapshot;
    }
}