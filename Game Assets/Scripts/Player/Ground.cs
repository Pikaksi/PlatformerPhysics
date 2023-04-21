using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private bool onGround;
    [SerializeField] private bool onwWall;
    [SerializeField] private bool onCealing;
    private float friction;

    Jump jump;

    private void Start() 
    {
        jump = GetComponent<Jump>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        onGround = false;
        onwWall = false;
        onCealing = false;
        friction = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            onGround |= normal.y >= 0.9f;
            onwWall |= normal.y == 0f;
            onCealing |= normal.y < -0.5f;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        if (collision.rigidbody == null) return;

        PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

        friction = 0;

        if (material == null) return;

        friction = material.friction;
    }

    public bool GetOnGround()
    {
        return onGround;
    }
    public bool GetOnWall()
    {
        return onwWall;
    }
    public bool GetOnCealing()
    {
        return onCealing;
    }
    public float GetFriction()
    {
        return friction;
    }
}