using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BeginJump : MovementCommand
{
    public BeginJump(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessBeginJump();
    }
}
