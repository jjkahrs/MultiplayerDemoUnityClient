using System;

public class Subscription<T> where T : GameEvent {
    public string eventType;
    public object subscriber;
    public Action<T> onEvent;

    public Subscription(Type et, Action<T> onEvent, object sub) {
        this.eventType = et.Name;
        this.onEvent = onEvent;
        this.subscriber = sub;
    }
}