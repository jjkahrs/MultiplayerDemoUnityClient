using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootstrapEvent : GameEvent {
    public string sceneToLoad { get; private set;}
    public BootstrapEvent(string sceneToLoad) {
        this.sceneToLoad = sceneToLoad;
    }
}