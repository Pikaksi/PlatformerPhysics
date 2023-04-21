using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Vector3 startPoint;
    [SerializeField] Vector3 endPoint;
    [SerializeField] float maxSpeed;
    [Header("Choose one or zero")]
    [SerializeField] bool moveUponPlayerTouch;
    [SerializeField] bool moveContinuously;

    [SerializeField] bool teleportToStart;

    Vector3 locationBeforeMoveTowards;
    Vector2 distanceMoved;
    GameObject player;

    [Header("Debugging")]
    [SerializeField] bool isTransportingPlayer;
    [SerializeField] int platformDirection = 1;    // -1 going back, 0 stationary, 1 going forward
    [SerializeField] Vector2 desiredPoint;
    [SerializeField] bool desiredElevatorMove;
    float speedAddon;
    bool wasTransportingPlayer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        startPoint = transform.position;
        endPoint += transform.position;

        if (moveUponPlayerTouch)
        {
            platformDirection = -1;
        }
    }

    void FixedUpdate()
    {
        if (isTransportingPlayer != wasTransportingPlayer && isTransportingPlayer)
        {
            desiredElevatorMove = true;
        }
        wasTransportingPlayer = isTransportingPlayer;

        MovePlatform();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CollisionCheck(other);
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        CollisionCheck(other);
    }

    private void CollisionCheck(Collision2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        for (int i = 0; i < other.contactCount; i++)
        {
            Vector2 normal = other.GetContact(i).normal;
            if (normal.y <= -0.9f)
            {
                isTransportingPlayer = true;
                player.transform.parent = transform;
                return;
            }

            if (normal.y == 0 && other.gameObject.GetComponent<Jump>().GetIsWallSliding())       // player contacts wall
            {
                isTransportingPlayer = true;
                player.transform.parent = transform;
                return;
            }

            if (distanceMoved.x > 0 && normal.x == -1)
            {
                isTransportingPlayer = true;
                player.transform.parent = transform;
                return;
            }
            if (distanceMoved.x < 0 && normal.x == 1)
            {
                isTransportingPlayer = true;
                player.transform.parent = transform;
                return;
            }
        }

        isTransportingPlayer = false;
        player.transform.parent = null;
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            isTransportingPlayer = false;
            player.transform.parent = null;
        }
    }

    private void MovePlatform()
    {
        if (isTransportingPlayer)
        {
            player.transform.parent = transform;
        }

        locationBeforeMoveTowards = transform.position;
        GetDesiredPoint();
        transform.position = Vector3.MoveTowards(transform.position, desiredPoint, maxSpeed * Time.deltaTime + speedAddon);

        HandleTeleportingToStart();

        distanceMoved = transform.position - locationBeforeMoveTowards;

        if (moveContinuously)
        {
            ChangeDirectionAtEnd();
        }

        if (desiredElevatorMove)
        {
            Debug.Log("touch");
            desiredElevatorMove = false;

            if (isTransportingPlayer && moveUponPlayerTouch)
            {
                ChangeDirectionAtEnd();
            }
        }

        if (isTransportingPlayer)
        {
            player.transform.parent = null;
        }
    }

    private void HandleTeleportingToStart()
    {
        var h = transform.position == endPoint || platformDirection == -1;
        if (h && teleportToStart)
        {
            var j = player.transform.parent;
            player.transform.parent = null;

            transform.position = startPoint;
            if (moveUponPlayerTouch) { SetDirection(-1); }
            

            player.transform.parent = j;
        }
    }

    private void ChangeDirectionAtEnd()
    {
        Vector2 positionVector2 = transform.position;
        if (positionVector2 == desiredPoint)
        {
            ChangeDirection();
        }
    }

    private void GetDesiredPoint()
    {
        switch (platformDirection)
        {
            case 1:
                desiredPoint = endPoint;
                break;
            case -1:
                desiredPoint = startPoint;
                break;
            case 0:
                desiredPoint = transform.position;
                break;
        }
    }

    public void ChangeDirection()
    {
        Debug.Log("change");
        platformDirection *= -1;
    }

    public void SetDirection(int direction)
    {
        platformDirection = Mathf.Clamp(direction, -1, 1);
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireSphere(endPoint + transform.position, 0.5f);
    }
}
