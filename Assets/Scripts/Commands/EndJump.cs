using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EndJump : MovementCommand
{
    public EndJump(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessEndJump();
    }
}
