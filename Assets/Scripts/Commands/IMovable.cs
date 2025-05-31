using UnityEngine;

public interface IMovable
{
    public void ProcessBeginJump();
    public void ProcessEndJump();
    public void ProcessBeginLeft();
    public void ProcessEndLeft();
    public void ProcessBeginRight();
    public void ProcessEndRight();
    public void ProcessBeginGrapple();
    public void ProcessEndGrapple();
}
