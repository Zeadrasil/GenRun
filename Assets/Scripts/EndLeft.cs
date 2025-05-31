using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EndLeft : MovementCommand
{
    public EndLeft(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessEndLeft();
    }
}
