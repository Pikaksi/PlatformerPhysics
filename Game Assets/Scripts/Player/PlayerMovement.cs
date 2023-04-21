using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float maxSpeed = 4f;
    [SerializeField, Range(0f, 100f)] private float maxAcceleration = 35f;
    [SerializeField, Range(0f, 100f)] private float maxAirAcceleration = 20f;
    [SerializeField] int playerDirection = 1; // -1 = left, 1 = right
    [SerializeField] bool hasDash;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 0.4f;
    [SerializeField] float dashCooldown = 0.5f;

    private PlayerController playerController;
    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private Ground ground;
    private Jump jump;

    private float maxSpeedChange;
    private float acceleration;
    private bool onGround;
    private bool isWallJumping;
    private bool onDashDuration;
    [SerializeField] bool isDashing;
    bool desiredDash;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        ground = GetComponent<Ground>();
        playerController = GetComponent<PlayerController>();
        jump = GetComponent<Jump>();
    }

    private void Update()
    {
        desiredDash |= playerController.RetrieveShiftInput();
        direction.x = playerController.RetrieveMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(maxSpeed - ground.GetFriction(), 0f);
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        velocity = body.velocity;

        ProcessMovementOptions();

        body.velocity = velocity;

        if (velocity.x > 0)
        {
            playerDirection = 1;
        }
        else if (velocity.x < 0)
        {
            playerDirection = -1;
        }
    }

    private void ProcessMovementOptions()
    {
        if (desiredDash)
        {
            desiredDash = false;
            if (hasDash && playerController.RetrieveMoveInput() != 0 && !onDashDuration)
            {
                isDashing = true;
                StartCoroutine(SetDashCooldown());
                StartCoroutine(DashMovement(dashDuration));
            }
        }

        if (isWallJumping) return;
        
        if (isDashing == false)
        {
            ApplyNormalMovement();
        }
    }

    private void ApplyNormalMovement()
    {
        onGround = ground.GetOnGround();

        acceleration = onGround ? maxAcceleration : maxAirAcceleration;
        maxSpeedChange = acceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
    }

    private IEnumerator DashMovement(float duration)
    {
        float timeElapsed = 0f;

        float direction = DetermineDashDirection();

        while (timeElapsed < duration && isDashing)
        {
            velocity.y = 0;
            velocity.x = dashSpeed * direction;

            body.velocity = velocity;

            yield return new WaitForFixedUpdate();

            timeElapsed += Time.deltaTime;
        }

        body.velocity = new Vector2(maxSpeed * direction, body.velocity.y);
        isDashing = false;
    }

    private float DetermineDashDirection()
    {
        float direction = playerController.RetrieveMoveInput();

        if (jump.GetIsWallSliding())
        {
            direction = direction * -1;
        }
        
        return direction;
    }

    private IEnumerator SetDashCooldown()
    {
        onDashDuration = true;
        yield return new WaitForSeconds(dashCooldown);
        onDashDuration = false;
    }

    public IEnumerator WallJumpSideMovement(int direction, float duration, float speed)
    {
        float timeElapsed = 0f;
        isWallJumping = true;

        while (timeElapsed < duration && isDashing == false)
        {
            velocity.x = body.velocity.x;

            yield return new WaitForFixedUpdate();

            velocity.x = speed * direction;
            body.velocity = new Vector2(velocity.x, body.velocity.y);
            timeElapsed += Time.deltaTime;
        }

        isWallJumping = false;
    }

    public int GetPlayerDirection()
    {
        return playerDirection;
    }

    public bool GetIsDashing()
    {
        return isDashing;
    }

    public void SetDash(bool change)
    {
        isDashing = change;
    }
}
