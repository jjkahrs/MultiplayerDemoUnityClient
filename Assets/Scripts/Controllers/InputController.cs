using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    private List<KeyCode> ONLINE_STATE_TOGGLE_KEYS = new List<KeyCode>() {
        KeyCode.W, KeyCode.S, KeyCode.A, KeyCode.D
    };

    private GameMaster _GM;

    void Awake()
    {
        _GM = GameObject.FindObjectOfType<GameMaster>();
    }

    void Update()
    {
        foreach( KeyCode keyCode in ONLINE_STATE_TOGGLE_KEYS )
            SendKeyToggleEvent( keyCode );
    }

    private void SendKeyToggleEvent( KeyCode keyCode )
    {
        if( Input.GetKeyDown( keyCode ) )
        {
            _GM.DispatchEvent( new KeyboardEvent( keyCode, true ) );
        }
        if( Input.GetKeyUp( keyCode ) )
        {
            _GM.DispatchEvent( new KeyboardEvent( keyCode, false ) );
        }
    }

}

