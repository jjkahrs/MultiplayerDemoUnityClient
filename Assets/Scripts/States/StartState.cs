using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StartState : GameState
{
    private NetworkManager _NetworkManager;

    public override void Init()
    {
        _NetworkManager = GameObject.FindObjectOfType<NetworkManager>();
        _GM.Subscribe( typeof( RequestNetworkConnectionEvent ), OnNetworkConnectRequest, this );
        _GM.Subscribe( typeof( NetworkConnectionAcceptedEvent ), OnConnectionAccepted, this );
        _GM.Subscribe( typeof( NetworkConnectionErrorEvent ), OnNetworkError, this );
        InitLocalPlayer();
    }

    private void OnNetworkConnectRequest( GameEvent gameEvent )
    {
        // The local session id is acquired from a login server
        _NetworkManager.Connect( _GM.GetGameStore().localSessionId );
    }

    private void OnConnectionAccepted( GameEvent gameEvent )
    {
        this.SetNextState( new NetworkConnectingState() );
        _GM.ChangeState();
    }

    private void OnNetworkError( GameEvent gameEvent )
    {
        this.SetNextState( new StartState() );
        _GM.ChangeState();
    }

    private void InitLocalPlayer()
    {
        GameStore store = new GameStore();

        Guid uuid = Guid.NewGuid();
        Actor actor = new Actor( uuid.ToString() );
        actor.name = "Jane";
        store.localActor = actor;
        store.localSessionId = _GM.GetRandom().Next(0,1000).ToString();

        _GM.StartGame( store );
    }
}