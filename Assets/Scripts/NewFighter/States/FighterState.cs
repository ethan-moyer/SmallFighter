public abstract class FighterState
{
    protected NewFighter fighter;

    public FighterState(NewFighter fighter)
    {
        this.fighter = fighter;
    }

    public virtual void OnStateEnter() { }

    public virtual void Update(InputData currentInput) { }

    public virtual void OnStateExit() { }
}
