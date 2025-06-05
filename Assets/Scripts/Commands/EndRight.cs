using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EndRight : MovementCommand
{
    public EndRight(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessEndRight();
    }
}
