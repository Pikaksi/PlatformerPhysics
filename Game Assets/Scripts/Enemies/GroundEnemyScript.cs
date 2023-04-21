using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemyScript : MonoBehaviour
{
    [SerializeField] float aggroRange = 20f;
    [SerializeField] float attackRange = 5f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] Vector2 jumpVector;
    [SerializeField] float jumpAttackCooldown = 1f;
    [SerializeField] float gravityScale = 5f;
    [SerializeField] LayerMask groundLayer;


    [Header("Debugging")]
    [SerializeField] int enemyState = 0; // 0 = idle, 1 = walk towards player, 2 = attack1,
    [SerializeField] bool cannotChangeState;
    float x;
    float y;
    Rigidbody2D rb;
    GameObject player;
    [SerializeField] bool onJumpAttackCooldown;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        CollisionCheck(other);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        CollisionCheck(other);
    }

    void FixedUpdate()
    {
        rb.gravityScale = gravityScale;
        x = player.transform.position.x - transform.position.x;
        y = player.transform.position.y - transform.position.y;
        SetEnemyState();

        if (enemyState == 0)
        {
            ApplyWandering();
        }
        else if (enemyState == 1)
        {
            ApplyWalking();
        }
        else if (enemyState == 2)
        {
            FirstAttack();
        }
    }

    private void CollisionCheck(Collision2D other)
    {
        Debug.Log(other.gameObject.layer);
        Debug.Log(6);
        if (other.gameObject.layer == 6)
        {
            Debug.Log("2");
            cannotChangeState = false;
        }
    }

    private void SetEnemyState()
    {
        if (cannotChangeState) { return; }

        if (Mathf.Sqrt(x * x + y * y) <= aggroRange)
        {
            enemyState = 1; // walk towards player
        }
        else
        {
            enemyState = 0; // idle
        }

        if (enemyState == 1 && Mathf.Sqrt(x * x + y * y) <= attackRange && !onJumpAttackCooldown)
        {
            enemyState = 2;
        }
    }

    private void FirstAttack()
    {
        if (transform.position.x < player.transform.position.x)
        {
            jumpVector.x = Mathf.Abs(jumpVector.x);
        }
        else
        {
            jumpVector.x = Mathf.Abs(jumpVector.x) * -1;
        }
        rb.velocity = jumpVector;
        enemyState = 999;
        cannotChangeState = true;
        StartCoroutine(ApplyJumpCooldown());
    }

    private void ApplyWalking()
    {
        if (transform.position.x < player.transform.position.x)
        {
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + acceleration * Time.deltaTime, -maxSpeed, maxSpeed), rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + -acceleration * Time.deltaTime, -maxSpeed, maxSpeed), rb.velocity.y);
        }
    }

    private void ApplyWandering()
    {

    }

    private IEnumerator ApplyJumpCooldown()
    {
        onJumpAttackCooldown = true;
        yield return new WaitForSeconds(jumpAttackCooldown);
        onJumpAttackCooldown = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
