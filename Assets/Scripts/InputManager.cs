using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>, IObserver<(int, int, KeyCode)>
{

    [SerializeField] private Controls[] playerControls;
    [SerializeField] private GameManager controlUpdater;
    private bool[] holdingJump;
    private bool[] lastFrameHeldJump;
    private bool[] holdingGrapple;
    private bool[] lastFrameHeldGrapple;
    private float[] movement;
    private void Update()
    {
        for (int i = 0; i < playerControls.Length; i++)
        {
            lastFrameHeldJump[i] = holdingJump[i];
            holdingJump[i] = Input.GetKey(playerControls[i].jump);
            lastFrameHeldGrapple[i] = holdingGrapple[i];
            holdingGrapple[i] = Input.GetKey(playerControls[i].grapple);
            movement[i] = 0;
            movement[i] += Input.GetKey(playerControls[i].right) ? 1 : 0;
            movement[i] -= Input.GetKey(playerControls[i].left) ? 1 : 0;
        }
    }
    private void Start()
    {
        holdingJump = new bool[playerControls.Length];
        lastFrameHeldJump = new bool[playerControls.Length];
        holdingGrapple = new bool[playerControls.Length];
        lastFrameHeldGrapple = new bool[playerControls.Length];
        movement = new float[playerControls.Length];
        controlUpdater.Subscribe(this);
    }

    public float GetMovement(int index = 0)
    {
        return index > -1 && index < playerControls.Length ? movement[index] : 0;
    }

    public bool GetStartedJump(int index = 0)
    {
        return index > -1 && index < playerControls.Length && (holdingJump[index] && !lastFrameHeldJump[index]);
    }

    public bool GetEndedJump(int index = 0)
    {
        return index > -1 && index < playerControls.Length && !holdingJump[index] && lastFrameHeldJump[index];
    }

    public bool GetJumping(int index = 0)
    {
        return index > -1 && index < playerControls.Length && holdingJump[index];
    }

    public bool GetGrappling(int index = 0)
    {
        return index > -1 && index < playerControls.Length && holdingGrapple[index];
    }

    public bool GetEndedGrapple(int index = 0)
    {
        return index > -1 && index < playerControls.Length && !holdingGrapple[index] && lastFrameHeldGrapple[index];
    }
    
    public bool GetStartedGrapple(int index = 0)
    {
        return index > -1 && index < playerControls.Length && holdingGrapple[index] && !lastFrameHeldGrapple[index];
    }

    public void Observe((int, int, KeyCode) newValue)
    {
        if (newValue.Item1 > -1 && newValue.Item1 < playerControls.Length)
        {
            switch (newValue.Item2)
            {
                case 0:
                    {
                        playerControls[newValue.Item1].left = newValue.Item3;
                        break;
                    }
                case 1:
                    {
                        playerControls[newValue.Item1].right = newValue.Item3;
                        break;
                    }
                case 3:
                    {
                        playerControls[newValue.Item1].grapple = newValue.Item3;
                        break;
                    }
                default:
                    {
                        playerControls[newValue.Item1].jump = newValue.Item3;
                        break;
                    }
            }
        }
    }
}
