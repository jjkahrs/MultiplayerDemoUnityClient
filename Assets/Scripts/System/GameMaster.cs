using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;
using System.Reflection;

public class GameMaster : GameEventDispatcher
{    
    public string initialState; 

    private GameState _CurrentState;
    private GameStore _GameStore;
    private System.Random _Random;
    private bool isInitialized = false;

    public void Awake() {
        if(!isInitialized) {
            _Random = new System.Random();
            InitEventBrodcasting();
            LoadInitialState();
        }
    }

    void LoadInitialState() {
            Assembly assembly = Assembly.GetCallingAssembly();
            Type firstState = null;
            foreach(Type t in assembly.GetTypes()) {
                if(t.Name == initialState) {
                    firstState = t;
                    break;
                }
            }
            if(firstState != null) {
                _CurrentState = (GameState)Activator.CreateInstance(firstState);
                _CurrentState.Init();
            }
            else {
                Debug.LogError("Initial State of "+initialState+" not valid class");
            }

    }

    protected override IEnumerator HandleTick() {
        _CurrentState.Tick();
        yield return null;
    }

    protected override void OnFrame()
    {
        _CurrentState.OnFrame();
    }

    public void ChangeState() {
        _CurrentState = _CurrentState.ChangeState();
        _CurrentState.Init();
        DispatchEvent(new StateChangeEvent(_CurrentState));

    }

    public System.Random GetRandom() {
        return _Random;
    }

    public void StartGame(GameStore store) {
        _GameStore = store;
    }


    public GameStore GetGameStore() {
        return _GameStore;
    }

}
