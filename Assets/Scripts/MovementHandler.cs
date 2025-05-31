using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [SerializeField] private PlayerMovement2D mover;
    private MovementCommand[] commands;
    [SerializeField] private int controlsId;
    // Start is called before the first frame update
    void Start()
    {
        commands = new MovementCommand[] {new BeginLeft(mover), new EndLeft(mover), new BeginRight(mover), new EndRight(mover), new BeginJump(mover), new EndJump(mover), new BeginGrapple(mover), new EndGrapple(mover) };
    }

    // Update is called once per frame
    void Update()
    {
        switch(InputManager.Instance.GetMovement(controlsId))
        {
            case -1:
                commands[0].Execute(); 
                commands[3].Execute();
                break;
            case 1:
                commands[2].Execute();
                commands[1].Execute();
                break;
            default:
                commands[1].Execute();
                commands[3].Execute();
                break;
        }
        if(InputManager.Instance.GetStartedJump(controlsId))
        {
            commands[4].Execute();
        }
        if(InputManager.Instance.GetEndedJump(controlsId))
        {
            commands[5].Execute();
        }
        if(InputManager.Instance.GetStartedGrapple(controlsId))
        {
            commands[6].Execute();
        }
        if(InputManager.Instance.GetEndedGrapple(controlsId))
        {
            commands[7].Execute();
        }
    }
}
