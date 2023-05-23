using System.Collections;
using UnityEngine;
using System;

public abstract class GameState {

    protected GameState _NextState = null;
    protected GameMaster _GM;
    //protected SoundManager _SM;

    public GameState() {
        _GM = UnityEngine.Object.FindObjectOfType<GameMaster>();
        //_SM = UnityEngine.Object.FindObjectOfType<SoundManager>();
        _GM.Subscribe(typeof(StateChangeEvent), OnStateChangeEvent, this);
    }

    public virtual GameState ChangeState() {
        _GM.UnsubscribeAll(this);
        return _NextState;
    }

    public virtual string GetStateName() {
        return this.GetType().Name;
    }

    public virtual void SetNextState(GameState state) {
        _NextState = state;
    }

    public virtual void Tick() {
    }

    public virtual void OnFrame()
    {
        // This is called during Unity's Update() call.
    }

    public virtual void Init() {
        // This is called prior to state change event
    }

    public virtual void PostInit() {
        // This is called after the state change event has been dispatched
        // We do this to handle weirdness with Event Dispatches in state change
    }

    void OnStateChangeEvent(GameEvent gameEvent) {
        if(gameEvent is StateChangeEvent) {
            StateChangeEvent sce = (StateChangeEvent)gameEvent;
            if(sce.stateName == GetStateName()) {
                PostInit();
            }
        }
    }

    private void Log(string s) {
        Debug.Log(DateTime.Now.ToString("[hh:mm:ss.fff] ")+s);
    }

}
