using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BeginLeft : MovementCommand
{
    public BeginLeft(IMovable subject) : base(subject)
    {
    }

    public override void Execute()
    {
        subject.ProcessBeginLeft();
    }
}
