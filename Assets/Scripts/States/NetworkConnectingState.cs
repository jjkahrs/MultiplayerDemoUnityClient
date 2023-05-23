using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkConnectingState : GameState
{
    private const int MAX_READS_PER_FRAME = 10;

    private NetworkManager _NetworkManager;
    private bool isSessionEstablished = false;

    public override void Init()
    {
        Debug.Log("NetworkConnectingState.Init()");
        _NetworkManager = GameObject.FindObjectOfType<NetworkManager>();
    }

    public override void OnFrame()
    {
        if( !isSessionEstablished && _NetworkManager.HasPendingData() )
        {
            // string[] messages = _NetworkManager.Receive();
            List<string> messages = _NetworkManager.ReadMessages();

            foreach( string data in messages )
            {
                Debug.Log($"NetworkConnectingState data: {data}");

                if( "SessionReady" == data ) 
                {
                    Debug.Log($"Session ready going to online state");
                    isSessionEstablished = true;
                    _GM.DispatchEvent( new SessionConnectEvent( _GM.GetGameStore().localSessionId ) );
                }
            }
        }

        if( isSessionEstablished )
        {
            this.SetNextState( new LoadingRemoteState() );
            _GM.ChangeState();
        }
        
    }    
}