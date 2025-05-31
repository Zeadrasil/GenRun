using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EndGrapple : MovementCommand
{
    public EndGrapple(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessEndGrapple();
    }
}
