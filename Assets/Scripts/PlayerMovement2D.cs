using System;
using Unity.Mathematics;
using UnityEngine;
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
            
            if(onGround() || OnWall())
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

            if((movingLeft ^ movingRight))
            {
                facingRight = movingRight;
                //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - width * 1.5f), Color.red, 1);
                //Debug.DrawLine(new Vector3(transform.position.x + width * (facingRight ? 0.5f : -0.5f), transform.position.y), new Vector3(transform.position.x + width * (facingRight ? 0.5f : -0.5f), transform.position.y - width * 2.25f), Color.red, 1);
                Vector3 addedVelocity = Vector3.zero;
                RaycastHit2D hit1 = Physics2D.Raycast(transform.position, Vector2.down, width * 1.5f, LayerMask.GetMask("Ground"));
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position + new Vector3(width * (facingRight ? 1 : -1), 0), Vector2.down, width * 3, LayerMask.GetMask("Ground"));
                if (hit1 && hit2)
                {
                    addedVelocity = (hit2.point - hit1.point).normalized * 10 * modifier * Time.deltaTime;
                    addedVelocity.y *= 2.5f;
                }
                else
                {
                    addedVelocity = new Vector3(10 * modifier * Time.deltaTime * (movingRight ? 1 : -1), 0);
                }
                velocity += addedVelocity;
            }

            float rate = onGround() ? 0.75f : 0.5f;
            velocity.y = rate == 0.75f ? math.max(0, velocity.y) : velocity.y - (20f * Time.deltaTime);


            velocity.x -= velocity.x > 0 ? ((velocity.x + 10) * rate * Time.deltaTime) - 10 * rate * Time.deltaTime : ((velocity.x - 10) * rate * Time.deltaTime) + 10 * rate * Time.deltaTime;
            RunMovement(velocity.magnitude * Time.deltaTime);
            lastPosition = transform.position;
            Debug.Log(jumpsRemaining);
        }
    }

    private bool onGround()
    {
        return Physics2D.RaycastAll(transform.position, new Vector2(0, -1), 0.6f, LayerMask.GetMask("Ground")).Length > 0 || Physics2D.RaycastAll(new Vector3(transform.position.x - width, transform.position.y), new Vector2(0, -1), 0.6f, LayerMask.GetMask("Ground")).Length > 0 || Physics2D.RaycastAll(new Vector3(transform.position.x + width, transform.position.y), new Vector2(0, -1), 0.6f, LayerMask.GetMask("Ground")).Length > 0;
    }

    private bool OnWall()
    {
        //Debug.DrawLine(new Vector2(transform.position.x - width * 1.5f, transform.position.y), new Vector2(transform.position.x + width * 1.5f, transform.position.y), Color.red);
        return Physics2D.OverlapCircleAll(transform.position, width * 1.5f, LayerMask.GetMask("WallClimb")).Length > 0;
    }

    private void resetToStart()
    {
        transform.position = new Vector3(0, -5);
        transform.rotation = Quaternion.identity;
        grappling = false;
        facingRight = true;
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

    private void RunMovement(float movement, Vector2 direction = new Vector2())
    {
        if(direction.magnitude == 0)
        {
            direction = velocity;
        }
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
                Vector3 AMovement = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * hit.normal.normalized;
                Vector3 BMovement = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1)) * hit.normal.normalized;
                float ADot = Vector3.Dot(direction.normalized, AMovement);
                float BDot = Vector3.Dot(direction.normalized, BMovement);
                if (ADot > BDot)
                {
                    velocity = AMovement * (movement - hit.distance + 0.01f) * ADot;
                    RunMovement((movement - hit.distance + 0.01f) * ADot, AMovement);
                }
                else
                {
                    velocity = BMovement * (movement - hit.distance + 0.01f) * BDot;
                    RunMovement((movement - hit.distance + 0.01f) * BDot, BMovement);
                }
                
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
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, width, direction.normalized, movement, LayerMask.GetMask("Ground", "WallClimb", "Grapple"));
            if (hit)
            {
                transform.position += new Vector3(direction.x, direction.y).normalized * (hit.distance - 0.01f);
                if (hit.distance > 0.01f)
                {
                    Vector3 AMovement = Quaternion.AngleAxis(90, new Vector3(0, 0, 1)) * hit.normal.normalized;
                    Vector3 BMovement = Quaternion.AngleAxis(-90, new Vector3(0, 0, 1)) * hit.normal.normalized;
                    float ADot = Vector3.Dot(direction.normalized, AMovement);
                    float BDot = Vector3.Dot(direction.normalized, BMovement);
                    if (ADot > BDot)
                    {
                        velocity = AMovement * (movement - hit.distance + 0.01f) * ADot;
                        RunMovement((movement - hit.distance + 0.01f) * ADot, AMovement);
                    }
                    else
                    {
                        velocity = BMovement * (movement - hit.distance + 0.01f) * BDot;
                        RunMovement((movement - hit.distance + 0.01f) * BDot, BMovement);
                    }
                }
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
        if (jumpsRemaining > 0 && !grappling)
        {
            jumpsRemaining--;
            float force = jumpsRemaining == 0 ? 10 : 15;
            velocity.y = velocity.y > 0 ? velocity.y + force : force;
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
