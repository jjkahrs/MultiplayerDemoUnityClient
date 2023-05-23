
public class StateChangeEvent : GameEvent {
    public string stateName;

    public StateChangeEvent(GameState state) {
        this.stateName = state.GetStateName();
    }

    public override string ToString()
    {
        return this.GetType()+":"+this.stateName;
    }
}