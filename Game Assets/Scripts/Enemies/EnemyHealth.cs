using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] float startingHealth = 1;
    [SerializeField] float xKnockbackMultiplier = 1;
    [SerializeField] float yKnockbackMultiplier = 1;

    [Header("Debugging")]
    [SerializeField] float currentHealth;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        currentHealth = startingHealth;
    }

    public void TakeDamage(float damage, float verticalLook, float horizontalLook, float knockBack)
    {
        currentHealth += damage;

        ApplyHitMomentum(verticalLook, horizontalLook, knockBack);

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void ApplyHitMomentum(float verticalLook, float horizontalLook, float knockBack)
    {
        if (verticalLook != 0)
        {
            Debug.Log("upHit");
            rb.velocity += new Vector2(0, knockBack * verticalLook * xKnockbackMultiplier);
            return;
        }

        if (horizontalLook != 0)
        {
            Debug.Log("sideHit");
            rb.velocity += new Vector2(knockBack * horizontalLook * yKnockbackMultiplier, 0);
            return;
        }
    }
}
