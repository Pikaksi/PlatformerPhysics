using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] float attackHight = 1f;
    [SerializeField] float attackLenght = 1f;
    [SerializeField] float swordDamage = 5;
    [SerializeField] float attackCooldown = 0.5f;
    [SerializeField] float knockBack = 5f;

    [SerializeField] SwordAnimator swordAnimator;

    PlayerController playerController;
    PlayerMovement playerMovement;
    Ground ground;
    Jump jump;

    bool desiredAttack;
    bool onGround;
    bool onAttackCooldown;
    float verticalLook;
    float horizontalLook;

    private void Start() 
    {
        playerController = GetComponent<PlayerController>();
        playerMovement = GetComponent<PlayerMovement>();
        ground = GetComponent<Ground>();
        jump = GetComponent<Jump>();
    }

    private void Update() 
    {
        desiredAttack |= playerController.RetrieveAttackInput();
    }

    private void FixedUpdate() 
    {
        onGround = ground.GetOnGround();

        if (desiredAttack)
        {
            desiredAttack = false;

            if (onAttackCooldown) return;

            StartCoroutine(ApplyAttackCooldown());
            desiredAttack = false;
            ProcessAttack();
        }
    }

    private IEnumerator ApplyAttackCooldown()
    {
        onAttackCooldown = true;
        yield return new WaitForSeconds(attackCooldown);
        onAttackCooldown = false;
    }

    private void ProcessAttack()
    {
        verticalLook = playerController.RetrieveVerticalLookInput();
        horizontalLook = playerMovement.GetPlayerDirection();

        if (verticalLook == 1)
        {
            ProcessOverlappingArea(new Vector2(-attackHight, 0), new Vector2(attackHight, attackLenght), 1);
        }
        else if (verticalLook == -1 && onGround == false)
        {
            ProcessOverlappingArea(new Vector2(-attackHight, 0), new Vector2(attackHight, -attackLenght), -1);
        }
        else if (horizontalLook == 1)
        {
            ProcessOverlappingArea(new Vector2(0, attackHight), new Vector2(attackLenght, -attackHight), 0);
        }
        else if (horizontalLook == -1)
        {
            ProcessOverlappingArea(new Vector2(0, -attackHight), new Vector2(-attackLenght, attackHight), 0);
        }
    }

    // swordDirection 1 = up, 0 = left/right, -1 = down
    private void ProcessOverlappingArea(Vector2 firstVector, Vector2 secondVector, float swordDirection)
    {
        Vector2 position = new Vector2(transform.position.x, transform.position.y);

        //Debug.Log(Physics2D.OverlapArea(position + firstVector, position + secondVector, enemyLayerMask));
        //Debug.Log(position + firstVector);
        //Debug.Log(position + secondVector);

        Collider2D targetEnemy = Physics2D.OverlapArea(position + firstVector, position + secondVector, enemyLayerMask);

        swordAnimator.SwordSwingAnimation(horizontalLook, swordDirection);

        IfDealtDamage(targetEnemy, swordDirection);
    }

    private void IfDealtDamage(Collider2D targetEnemy, float swordDirection)
    {
        if (targetEnemy == null) return;

        EnemyHealth enemyHealth = targetEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null) return;

        enemyHealth.TakeDamage(swordDamage, verticalLook, horizontalLook, knockBack);

        playerMovement.SetDash(false);

        if (swordDirection == -1)
        {
            jump.PogoJumpAction();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        Vector3 position = transform.position;
        Gizmos.DrawLine(position + new Vector3(0, 0, 0), position + new Vector3(attackLenght, 0, 0));
        Gizmos.DrawLine(position + new Vector3(attackLenght, -attackHight, 0), position + new Vector3(attackLenght, attackHight, 0));
    }

    
}
