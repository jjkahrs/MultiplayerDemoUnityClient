using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PreloadState : GameState {
    public override void Init()
    {
        _GM.Subscribe(typeof(BootstrapEvent), OnBootstrap, this);
        _GM.StartGame(new GameStore());
    }

    void OnBootstrap(GameEvent gameEvent) {
        Debug.Log("Bootstrapping...");
        if(gameEvent is BootstrapEvent) {
            BootstrapEvent be = (BootstrapEvent) gameEvent;
            if(be.sceneToLoad == null) {
                Debug.LogError("BootstrapEvent missing scene to load");
                return;
            }

            if(SceneManager.GetActiveScene().name != be.sceneToLoad) {
                SceneManager.LoadScene(sceneName: be.sceneToLoad);
            }            
        }
    }

}