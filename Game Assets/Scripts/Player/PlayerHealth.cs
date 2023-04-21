using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int playerStartingHealth = 5;
    [SerializeField] int currentPlayerHealth;
    [SerializeField] float invincibilitySeconds = 0.5f;
    [SerializeField] GameObject baguette;

    [Header("Debugging")]
    [SerializeField] bool isInvincible;

    Respawn respawn;
    PlayerController playerController;

    private void Start()
    {
        respawn = GetComponent<Respawn>();
        playerController = GetComponent<PlayerController>();
        currentPlayerHealth = playerStartingHealth;

        Physics2D.IgnoreLayerCollision(7, 8, true);
        Physics2D.IgnoreLayerCollision(7, 7, false);
    }

    private void Update()
    {
        if (currentPlayerHealth <= 0)
        {
            respawn.ProcessDeath();
        }

        //test button
        if (playerController.RetrieveJInput())
        {
            ModifyPlayerHealth(-1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        CheckCollisions(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        CheckCollisions(other);
    }

    private void CheckCollisions(Collider2D other)
    {
        if (isInvincible) return;

        Debug.Log("hit");

        StartCoroutine(ApplyInvincibility());

        if (other.tag == "Enemy")
        {
            Debug.Log("damage");
            ModifyPlayerHealth(other.GetComponentInParent<EnemyDamageScript>().GetDamage());
        }
    }

    private IEnumerator ApplyInvincibility()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilitySeconds);

        isInvincible = false;
    }

    public void ModifyPlayerHealth(int damage)
    {
        Debug.Log(damage);

        currentPlayerHealth += damage;
        if (damage < 0)
        {
            BroadcastMessage("DamageTaken",SendMessageOptions.DontRequireReceiver);
            SummonBaguette();
        }
    }

    private void SummonBaguette()
    {
        Instantiate(baguette, transform.position, Quaternion.identity);
    }

    public void SpikeDamageRespawn(int damage)
    {
        ModifyPlayerHealth(damage);
        respawn.RespawnPointRespawn();
    }
}
