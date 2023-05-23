using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LoadingRemoteState : GameState
{
    private const string REQUEST_PLAYER_STATE_COMMAND = "ReqPlayerState";
    private const string REQUEST_WORLD_STATE_COMMAND = "ReqWorldState";
    private const string PLAYER_STATE_MSG = "PlayerState";
    private const string WORLD_STATE_MSG = "WorldState";

    bool hasPlayerLoaded = false;
    bool hasWorldLoaded = false;

    private NetworkManager _NetworkManager;

    public override void Init()
    {
        string timestamp = Utils.CurrentTimeMillis().ToString();
        Debug.Log("LoadingRemoteState.Init()");
        _NetworkManager = GameObject.FindObjectOfType<NetworkManager>();

        Debug.Log("===> Requesting Player State");
        _NetworkManager.Send( $"{timestamp}|{REQUEST_PLAYER_STATE_COMMAND}" );

        Debug.Log("===> Requesting World State");
        _NetworkManager.Send( $"{timestamp}|{REQUEST_WORLD_STATE_COMMAND}" );

    }

    public override void OnFrame()
    {

        if( _NetworkManager.HasPendingData() )
        {
            //string[] messages = _NetworkManager.Receive();
            List<string> messages = _NetworkManager.ReadMessages();

            foreach( string data in messages )
            {
                if( data == "")
                    break;

                Debug.Log($"<=== Loading Remote State data: {data}");
                processIncomingMessage( data );
            }
        }

        if( hasPlayerLoaded && hasWorldLoaded )
        {
            Debug.Log($"%%% updateLocalPlayerState _GM.GetGameStore().localActor.position={_GM.GetGameStore().localActor.position}");
            this.SetNextState( new OnlineState() );
            _GM.ChangeState();
        }
    }

    private void processIncomingMessage( string data )
    {
        string[] parts = data.Split("|");

        if( parts.Length < 3 )
        {
            Debug.LogError("Malformed message");
            return;
        }
            

        string timeStampString = parts[0];
        string messageType = parts[1];
        string payload = parts[2];
//Debug.Log($"messageType {messageType}");
        switch( messageType )
        {
            case PLAYER_STATE_MSG:
                updateLocalPlayerState( payload );
                break;
            case WORLD_STATE_MSG:
                updateWorldState( payload );
                break;
            default:
                break;
        }
    }

    private void updateWorldState( string payload )
    {
        _GM.GetGameStore().remoteActors.Clear();

        string[] segments = payload.Split("^");

        foreach( string segment in segments)
        {
            Actor actor = Actor.Parse( segment );

            if( actor.id == _GM.GetGameStore().localSessionId )
                continue;

            _GM.GetGameStore().remoteActors.Add( actor );
            _GM.DispatchEvent( new SpawnRemoteActorEvent( actor.id ) );
        }
        this.hasWorldLoaded = true;
    }


    private void updateLocalPlayerState( string payload )
    {
        Actor actor = Actor.Parse( payload );

        _GM.GetGameStore().localActor.position = actor.position;
        _GM.GetGameStore().localActor.headingNormal = actor.headingNormal.normalized;
        _GM.GetGameStore().localActor.speed = actor.speed;

        Debug.Log($"### updateLocalPlayerState _GM.GetGameStore().localActor.position={_GM.GetGameStore().localActor.position}");

        this.hasPlayerLoaded = true;
    }

}