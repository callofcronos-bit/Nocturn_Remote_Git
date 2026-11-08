// Abstraction: base ของ State ผู้เล่น
public abstract class PlayerState
{
    protected PlayerController player;
    protected PlayerState(PlayerController p) { player = p; }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}
