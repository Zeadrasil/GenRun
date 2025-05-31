using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BeginGrapple : MovementCommand
{
    public BeginGrapple(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessBeginGrapple();
    }
}
