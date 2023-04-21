using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//|

[RequireComponent(typeof(PlayerController))]
public class Jump : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField, Range(0f, 100f)] float downwardMovementMultiplier = 3f;
    [SerializeField, Range(0f, 100f)] float upwardMovementMultiplier = 1.7f;
    [SerializeField] float maxFallingVelocity = -50f;

    [Header("Jumping")]
    [SerializeField, Range(0, 10)] int maxAirJumps = 0;
    [SerializeField] float jumpTime = 0;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] AnimationCurve downCurve;
    [SerializeField] AnimationCurve pogoCurve;
    [SerializeField] float jumpCurveMultiplier = 1;
    [SerializeField] float downJumpCurveMultiplier = 1;
    [SerializeField] float maxSpeedForDownCurve = -30;

    [Header("WallJumping")]
    [SerializeField] bool wallJumpEnabled = true;
    [SerializeField] Vector2 sideOffset = new Vector2(0.4f, 0f);
    [SerializeField] float wallCircleRadius = 0.2f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float wallJumpCooldown = 0.5f;
    [SerializeField] float wallJumpSideSteppingDesiredDuration = 0.3f;
    [SerializeField] float wallJumpSideSteppingSpeed = 10;
    [SerializeField] float maxWallSlidingSpeed = -4;

    PlayerController playerController;
    PlayerMovement playerMovement;
    Rigidbody2D rb;
    Ground ground;

    [Header("Debugging")]
    [SerializeField] Vector2 velocity;
    [SerializeField] bool curveJump;
    [SerializeField] bool curvePogo;
    [SerializeField] bool downCurveJump;
    int jumpPhase;
    float defaultGravityScale;
    bool desiredJump;
    [SerializeField] bool onGround;
    [SerializeField] bool onWall;
    [SerializeField] bool onCealing;
    bool isDashing;
    bool lastFrameOnGround;
    bool lastFrameOnCealing;
    bool collidingRightWall;
    bool collidingLeftWall;
    bool isWallSliding;
    bool isWallSlidingRight;
    bool isWallSlidingLeft;
    bool onWallJumpCooldown;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
        playerController = GetComponent<PlayerController>();

        defaultGravityScale = 1f;
        rb.gravityScale = 0;
    }

    void Update()
    {
        desiredJump |= playerController.RetrieveJumpInput();

        collidingRightWall = (Physics2D.OverlapCircle((Vector2)transform.position + sideOffset, wallCircleRadius, groundLayerMask));
        collidingLeftWall = (Physics2D.OverlapCircle((Vector2)transform.position - sideOffset, wallCircleRadius, groundLayerMask));

        jumpTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        SetGetMethods();
        
        if (isDashing) 
        {
            curveJump = false;
            curvePogo = false;
            downCurveJump = false;

            rb.gravityScale = 0;

            velocity = rb.velocity;
            velocity.y = 0;
            rb.velocity = velocity;

            return;
        }

        velocity = rb.velocity;

        if (wallJumpEnabled) ProcessWallSliding(); //sets wall sliding bools

        ApplyFallingSpeeds(); // modifies falling speeds

        ProcessJumpActivating(); // activates jumps
        ProcessCurveJumping(); // moves depending on jumps
        ProcessLanding(); // deactivates jumps when colliding

        rb.velocity = velocity;
    }

    private void SetGetMethods()
    {
        onGround = ground.GetOnGround();
        onWall = ground.GetOnWall();
        onCealing = ground.GetOnCealing();
        isDashing = playerMovement.GetIsDashing();
    }

    private void ProcessLanding()
    {
        // stops curve jumps if on ground or cealing
        if (lastFrameOnGround != onGround && onGround)
        {
            StopJump();
        }
        if (onCealing)
        {
            StopJump();
        }
        lastFrameOnGround = onGround;
        lastFrameOnCealing = onCealing;
    }

    public void StopJump() 
    {
        if (onGround || onCealing)
        {
            curvePogo = false;
            curveJump = false;
            downCurveJump = false;
        } 
    }

    private void ProcessJumpActivating()
    {
        if (onGround)
        {
            jumpPhase = 0;
        }

        if (desiredJump)
        {
            desiredJump = false;

            if (onGround || jumpPhase < maxAirJumps)
            {
                JumpAction();
            }
            else if (isWallSliding)
            {
                WallJumpAction();
            }
        }
    }

    private void ProcessCurveJumping()
    {
        if (curveJump)
        {
            velocity.y = jumpCurve.Evaluate(jumpTime) * jumpCurveMultiplier;
            StopJumpWhenSpacebarUp();
        }
        if (curvePogo)
        {
            velocity.y = pogoCurve.Evaluate(jumpTime) * jumpCurveMultiplier;
        }

        if (downCurveJump || curvePogo && velocity.y > maxSpeedForDownCurve)
        {
            velocity.y += downCurve.Evaluate(jumpTime) * downJumpCurveMultiplier;
        }
    }

    private void StopJumpWhenSpacebarUp()
    {
        if (playerController.RetrieveJumpDown() == false && velocity.y > 0)
        {
            jumpTime = 0;
            curveJump = false;
            downCurveJump = true;
            velocity.y = 1;
        }
    }

    private void JumpAction()
    {
        jumpTime = 0;
        curveJump = true;
        curvePogo = false;
        jumpPhase += 1;
    }

    public void PogoJumpAction()
    {
        jumpTime = 0;
        curvePogo = true;
        curveJump = false;
        jumpPhase += 1;
    }

    private void ProcessWallSliding()
    {
        // if on ground dont wallslide and return
        if (onGround || onWallJumpCooldown)
        {
            DisableWallSliding();
            return;
        }

        if (collidingRightWall && playerController.RetrieveMoveInput() == 1)
        {
            isWallSliding = true;
            isWallSlidingRight = true;
            isWallSlidingLeft = false;
        }
        else if (collidingLeftWall && playerController.RetrieveMoveInput() == -1)
        {
            isWallSliding = true;
            isWallSlidingLeft = true;
            isWallSlidingRight = false;
        }
        // if not wanting to wallslide
        else
        {
            DisableWallSliding();
        }

        if (isWallSliding && velocity.y < maxWallSlidingSpeed && onWallJumpCooldown == false)
        {
            curvePogo = false;
            curveJump = false;
            downCurveJump = false;
        }
    }

    private void DisableWallSliding()
    {
        isWallSlidingRight = false;
        isWallSlidingLeft = false;
        isWallSliding = false; 
    }

    private void WallJumpAction()
    {
        JumpAction();
        StartCoroutine(AddWallJumpCooldown());
        wallJumpSideStepping();
    }

    private IEnumerator AddWallJumpCooldown()
    {
        onWallJumpCooldown = true;
        yield return new WaitForSeconds(wallJumpCooldown);
        onWallJumpCooldown = false;
    }

    private void wallJumpSideStepping()
    {
        if (isWallSlidingRight)
        {
            StartCoroutine(playerMovement.WallJumpSideMovement(-1, wallJumpSideSteppingDesiredDuration, wallJumpSideSteppingSpeed));
        }
        else if (isWallSlidingLeft)
        {
            StartCoroutine(playerMovement.WallJumpSideMovement(1, wallJumpSideSteppingDesiredDuration, wallJumpSideSteppingSpeed));
        }
    }

    private void ApplyFallingSpeeds()
    {
        if(isWallSliding && velocity.y < maxWallSlidingSpeed)
        {
            velocity.y = maxWallSlidingSpeed;
        }

        if (velocity.y > 0)
        {
            rb.gravityScale = upwardMovementMultiplier;
        }
        else if (velocity.y < 0)
        {
            rb.gravityScale = downwardMovementMultiplier;
        }
        else if (velocity.y == 0)
        {
            rb.gravityScale = defaultGravityScale;
        }

        if (velocity.y <= maxFallingVelocity)
        {
            velocity.y = maxFallingVelocity;
        }
    }

    public bool GetIsWallSliding()
    {
        return isWallSliding;  // Baguette T:Juho
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireSphere(transform.position + new Vector3(sideOffset.x, 0, 0), wallCircleRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(-sideOffset.x, 0, 0), wallCircleRadius);
    }
}
