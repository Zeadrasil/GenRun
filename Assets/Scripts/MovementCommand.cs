using UnityEngine;

public abstract class MovementCommand : ICommandInterface
{
    protected IMovable subject;
    public MovementCommand(IMovable subject)
    {
        this.subject = subject;
    }
    public abstract void Execute();
}
