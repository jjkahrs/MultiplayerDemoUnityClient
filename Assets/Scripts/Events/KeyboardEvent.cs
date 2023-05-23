using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardEvent : GameEvent
{
    public KeyCode keyCode;
    public bool isDown;
    
    public KeyboardEvent( KeyCode keyCode, bool isDown )
    {
        this.keyCode = keyCode;
        this.isDown = isDown;
    }

    public KeyboardEvent( KeyCode keyCode )
    {
        this.keyCode = keyCode;
        isDown = true;
    }
}