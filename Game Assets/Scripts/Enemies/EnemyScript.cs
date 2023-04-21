using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] float maxYSpeed = 3f;
    [SerializeField] float maxXSpeed = 5f;
    [SerializeField] float maxYAcceleration = 0.5f;
    [SerializeField] float maxXAcceleration = 0.5f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] float agroRange = 18;

    GameObject player;
    Rigidbody2D rb;
    Vector3 velocity;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");

        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        float x = player.transform.position.x - transform.position.x;
        float y = player.transform.position.y - transform.position.y;
        if (Mathf.Sqrt(x*x + y*y) > agroRange) { return; }

        velocity = rb.velocity;

        float desiredVelocityX = Mathf.Clamp(player.transform.position.x - transform.position.x, -maxXAcceleration, maxXAcceleration);
        float desiredVelocityY = Mathf.Clamp(player.transform.position.y - transform.position.y, -maxYAcceleration, maxYAcceleration);

        velocity.x = Mathf.Clamp((desiredVelocityX + velocity.x), -maxXSpeed, maxXSpeed);
        velocity.y = Mathf.Clamp((desiredVelocityY + velocity.y), -maxYSpeed, maxYSpeed);

        rb.velocity = velocity;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireSphere(transform.position, agroRange);
    }
}
