using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class GameEventDispatcher : MonoBehaviour
{
    private static float TICK_INTERVAL = 0.5f;

    //private GameEventBroadcaster _EventBroadcaster;
    private float timer = 0f;
    private ConcurrentDictionary<Type,HashSet<Subscription<GameEvent>>> _Subscribers;
    private Queue<KeyValuePair<Type,GameEvent>> _EventQueue;

    protected void InitEventBrodcasting() {
        //_EventBroadcaster = new GameEventBroadcaster();
        _EventQueue = new Queue<KeyValuePair<Type,GameEvent>>();
        _Subscribers = new ConcurrentDictionary<Type,HashSet<Subscription<GameEvent>>>();

    }

    void Update() {
        ProcessEvents();
        timer += Time.deltaTime;
        if(timer >= TICK_INTERVAL) {
            timer = 0;
            StartCoroutine(HandleTick());
        }
        OnFrame();
    }

    protected virtual IEnumerator HandleTick() {
        //_CurrentState.Tick();
        yield return null;
    }

    protected virtual void OnFrame()
    {
    }

    private void ProcessEvents() {
        while(_EventQueue.Count != 0) {            
            KeyValuePair<Type,GameEvent> gameEvent = _EventQueue.Dequeue();
            Log("ProcessEvents _EventQueue "+gameEvent.Key.Name);
            NotifySubscribers(gameEvent.Value);
        }
    }

    public void DispatchEvent(GameEvent gameEvent) {
        Log("GameEventDispatcher.DispatchEvent "+gameEvent.GetType().Name);
        _EventQueue.Enqueue(new KeyValuePair<Type,GameEvent>(gameEvent.GetType(),gameEvent));
    }

    private void NotifySubscribers(GameEvent gameEvent) {
        Log("------ Total Subscribers "+_Subscribers.Keys.Count);
        foreach(Type key in _Subscribers.Keys) {            
            if(gameEvent.GetType() == key) {
                Log("------ "+gameEvent.GetType().Name);
                HashSet<Subscription<GameEvent>> subs = _Subscribers[key];
                Log("------ Notifying "+subs.Count+" subs");
                HashSet<Subscription<GameEvent>> subsCopy = new HashSet<Subscription<GameEvent>>(subs);
                foreach(Subscription<GameEvent> sub in subsCopy) {
                    sub.onEvent(gameEvent);
                    Log("------ Notify "+sub.onEvent.Method.Name);
                }
            }
        }
    }

    public void Subscribe(Type eventType, Action<GameEvent> action, object owner) {
        //Debug.Log("Subscribing to event "+eventType);
        if(_Subscribers.ContainsKey(eventType)) {
            HashSet<Subscription<GameEvent>> list = _Subscribers[eventType];
            list.Add(new Subscription<GameEvent>(eventType,action,owner));
        }
        else {
            HashSet<Subscription<GameEvent>> list = new HashSet<Subscription<GameEvent>>();
            list.Add(new Subscription<GameEvent>(eventType,action,owner));
            _Subscribers.TryAdd(eventType, list);
        }
    }

    public void UnsubscribeAll(object owner) {
        foreach(Type key in _Subscribers.Keys) {
            HashSet<Subscription<GameEvent>> list = _Subscribers[key];
            HashSet<Subscription<GameEvent>> cloned = new HashSet<Subscription<GameEvent>>();
            foreach(Subscription<GameEvent> sub in list) {
                if(sub.subscriber != owner) {
                    cloned.Add(sub);
                }
            }
            _Subscribers[key] = cloned;
        }
    }

    public void UnsubscribeAllFromEverything() {
        _Subscribers.Clear();
        _EventQueue.Clear();
    }

    private void Log(string s) {
        //Debug.Log(DateTime.Now.ToString("[hh:mm:ss.fff] ")+s);
    }

}
