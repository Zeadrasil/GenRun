using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BeginRight : MovementCommand
{
    public BeginRight(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessBeginRight();
    }
}
