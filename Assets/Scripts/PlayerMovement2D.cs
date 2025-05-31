using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;
public class PlayerMovement2D : MonoBehaviour, IMovable
{
    [SerializeField] private VoidEvent resetEvent;
    private bool grappling = false;
    private float grappleCooldown = 0;
    private bool facingRight = true;
    private Vector3 grappleCenter;
    private float grappleVelocity;
    int jumpsRemaining = 2;
    float coyoteJump = 0;
    private Vector3 velocity = new Vector3();
    private Vector3 lastPosition = Vector3.zero;
    [SerializeField] BoolVariable paused;
    [SerializeField] BoolVariable notInMenus;
    [SerializeField] float width = 0.5f;
    bool jumped = false;
    bool lockedOnWall = false;
    bool movingRight = false;
    bool movingLeft = false;
    // Start is called before the first frame update
    void Start()
    {
        resetEvent?.Subscribe(resetToStart);
    }

    void Update()
    {
        if (!paused && notInMenus)
        {
            grappleCooldown -= Time.deltaTime;
            
            if(onGround())
            {
                jumpsRemaining = 2;
                coyoteJump = 0;
            }
            else
            {
                if (coyoteJump < 0.2f)
                {
                    coyoteJump += Time.deltaTime;
                }
                else
                {
                    jumpsRemaining = math.min(jumpsRemaining, 1);
                }
            }
            float modifier = 10 / (MathF.Abs(velocity.x) + 0.1f);
            if((movingLeft ^ movingRight) && !lockedOnWall)
            {
                velocity.x += 10 * modifier * Time.deltaTime * (movingRight ? 1 : -1);
                facingRight = movingRight;
            }

            float rate = onGround() ? 0.75f : 0.5f;
            velocity.y = rate == 0.75f ? math.max(0, velocity.y) : velocity.y - (20f * Time.deltaTime);


            velocity.x -= velocity.x > 0 ? ((velocity.x + 10) * rate * Time.deltaTime) - 10 * rate * Time.deltaTime : ((velocity.x - 10) * rate * Time.deltaTime) + 10 * rate * Time.deltaTime;
            RunMovement(velocity.magnitude * Time.deltaTime);
            lastPosition = transform.position;
        }
    }

    private bool onGround()
    {
        return Physics2D.RaycastAll(transform.position, new Vector2(0, -1), 0.6f, LayerMask.GetMask("Ground")).Length > 0 || OnWall() != 0;
    }

    private int OnWall()
    {
        int result = 0;
        result = Physics2D.Raycast(transform.position + new Vector3(0, width), new Vector3(1, 0), width + 0.01f, LayerMask.GetMask("WallJump")) ? 1 : 0;
        if(result == 0)
        {
            result = Physics2D.Raycast(transform.position + new Vector3(0, width), new Vector3(-1, 0), width + 0.01f, LayerMask.GetMask("WallJump")) ? 3 : 0;
            if(result == 0)
            {
                result = Physics2D.Raycast(transform.position + new Vector3(0, -width), new Vector3(1, 0), width + 0.01f, LayerMask.GetMask("WallJump")) ? 2 : 0;
                if (result == 0)
                {
                    result = Physics2D.Raycast(transform.position + new Vector3(0, -width), new Vector3(-1, 0), width + 0.01f, LayerMask.GetMask("WallJump")) ? 4 : 0;
                }
            }
        }
        return result;
    }

    private void resetToStart()
    {
        transform.position = new Vector3(0, -5);
        transform.rotation = Quaternion.identity;
        grappling = false;
        facingRight = true;
        lockedOnWall = false;
        velocity = Vector3.zero;
    }
    private void drawLine(Vector3 start, Vector3 end, Color startColor, Color endColor, float duration = 0.005f, float width = 0.1f)
    {
        GameObject gameObj = new GameObject();
        LineRenderer lineRenderer = gameObj.AddComponent<LineRenderer>();
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
        //solution for the magenta line issue from https://forum.unity.com/threads/cant-set-color-for-linerenderer-always-comes-out-as-magenta-or-black.968447/
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        //end solution
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        Destroy(gameObj, duration);
    }

    private void RunCollision(RaycastHit2D hit, float additionalMovement)
    {
        if(hit.normal.x != 0)
        {
            velocity.x *= -0.9f;
        }
        else
        {
            velocity.y *= -0.1f;
        }
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("WallClimb"))
        {
            if(velocity.y > 0)
            {
                velocity.y += math.abs(velocity.x) * 0.1f;
            }
            else
            {
                velocity.y -= math.abs(velocity.x) * 0.1f;
            }
            velocity.x = 0;
            lockedOnWall = true;
        }
        RunMovement(additionalMovement);
    }

    private void RunMovement(float movement)
    {
        if (grappling)
        {
            Vector3 nextPosition = RotateAround(transform.position, grappleCenter, grappleVelocity * (facingRight ? 1 : -1) * Time.deltaTime * (360 / (2 * Mathf.PI * Vector3.Distance(transform.position, grappleCenter))));
            float distance = Vector3.Distance(transform.position, nextPosition);
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, width, nextPosition - transform.position, distance, LayerMask.GetMask("Ground", "WallClimb", "Grapple"));
            if (hit)
            {
                grappling = false;
                jumped = false;
                jumpsRemaining = 1;
                velocity = (RotateAround(grappleCenter, transform.position, facingRight ? -90 : 90) - transform.position).normalized * grappleVelocity;
                transform.position += (nextPosition - transform.position).normalized * (hit.distance - width);
                RunCollision(hit, distance - hit.distance + width);
            }
            else
            {
                grappleVelocity = math.max(grappleVelocity, 15f);
                drawLine(transform.position, grappleCenter, Color.white, Color.white);
                //velocity = (transform.position - lastPosition) / Time.deltaTime;
                transform.position = nextPosition;
            }
        }
        else
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, width, velocity.normalized, movement, LayerMask.GetMask("Ground", "WallClimb", "Grapple"));
            if (hit)
            {
                transform.position += velocity.normalized * (hit.distance - width);
                RunCollision(hit, movement - hit.distance + width);
            }
            else
            {
                transform.position += velocity.normalized * movement;
            }
        }
    }

    private Vector3 RotateAround(Vector3 original, Vector3 center, float degrees)
    {
        Vector3 originalPosition = (original - center);
        float angle = math.radians(degrees);
        Vector3 nextPosition = new Vector3(originalPosition.x * MathF.Cos(angle) - originalPosition.y * MathF.Sin(angle), originalPosition.x * Mathf.Sin(angle) + originalPosition.y * MathF.Cos(angle));
        nextPosition += center;
        return nextPosition;
    }

    public void ProcessBeginJump()
    {
        if (jumpsRemaining > 0)
        {
            jumpsRemaining--;
            float force = jumpsRemaining == 0 ? 10 : 15;
            if (lockedOnWall)
            {
                float x = OnWall() < 3 ? 1 : -1;
                velocity.y = math.max(velocity.y, 0);
                velocity += new Vector3(x, 1).normalized * force;
                lockedOnWall = false;
            }
            else
            {
                velocity.y = velocity.y > 0 ? velocity.y + force : force;
            }
        }
    }

    public void ProcessEndJump()
    {
        if (!grappling && jumped)
        {
            velocity.y *= velocity.y > 0 ? 0.5f : 1; new NotImplementedException();
        }
    }

    public void ProcessBeginGrapple()
    {
        if (grappleCooldown <= 0)
        {
            lockedOnWall = false;
            grappleCooldown = 0.25f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + (new Vector3(facingRight ? 1 : -1, 1, 0)), new Vector2(facingRight ? 1 : -1, 1));
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Grapple"))
            {
                drawLine(transform.position, hit.point, Color.green, Color.green, 5);
                grappling = true;
                grappleCenter = hit.point;
                grappleVelocity = velocity.magnitude;
                Vector3 perp = Vector3.Cross(grappleCenter - transform.position, Vector3.forward) * (facingRight ? 1 : -1);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, MathF.Atan2(perp.x, perp.y)));

            }
            else if (hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("Grapple"))
            {
                drawLine(transform.position, hit.point, Color.yellow, Color.yellow, 5);
            }
            else
            {
                drawLine(transform.position, transform.position + new Vector3(facingRight ? 1 : -1, 1, 0) * 1000, Color.red, Color.red, 5);
            }
        }
    }

    public void ProcessEndGrapple()
    {
        if (grappling)
        {
            grappling = false;
            jumped = false;
            jumpsRemaining = math.max(jumpsRemaining, 1);
            velocity = (RotateAround(grappleCenter, transform.position, facingRight ? -90 : 90) - transform.position).normalized * grappleVelocity;
        }
    }

    public void ProcessBeginLeft()
    {
        movingLeft = true;
    }

    public void ProcessEndLeft()
    {
        movingLeft = false;
    }

    public void ProcessBeginRight()
    {
        movingRight = true;
    }

    public void ProcessEndRight()
    {
        movingRight = false;
    }
}
