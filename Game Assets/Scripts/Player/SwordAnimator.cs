using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimator : MonoBehaviour
{
    [SerializeField] float animationTime = 0.24f;
    [SerializeField] float SwordStartDistance = 1.5f;
    [SerializeField] float verticalDistanceNotSide = 0.5f; // when swinging up/down move the sword x amount forward

    float direction;
    float horizontalDirection;

    [SerializeField] GameObject player;
    Animator animator;
    SpriteRenderer spriteRenderer;

    [Header("Debugging")]
    [SerializeField] bool swingAnimationPlaying;

    private void Start() 
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SwordSwingAnimation(float horizontalDirectionCarry, float directionCarry)
    {
        horizontalDirection = horizontalDirectionCarry;
        direction = directionCarry;

        float xOffset = 0f;
        float yOffset = 0f;
        GetXAndYOffset(out xOffset, out yOffset);

        StartCoroutine(EnableAnimation(xOffset, yOffset));
    }

    private void GetXAndYOffset(out float xOffset, out float yOffset)
    {
        if (direction == 0) //is vertical
        {
            xOffset = SwordStartDistance * horizontalDirection;
            yOffset = 0;
        }
        else if (direction == -1)
        {
            xOffset = verticalDistanceNotSide * horizontalDirection;
            yOffset = -SwordStartDistance;
        }
        else
        {
            xOffset = verticalDistanceNotSide * horizontalDirection;
            yOffset = SwordStartDistance;
        }
    }

    private IEnumerator EnableAnimation(float xOffset, float yOffset)
    {
        // horizontalDirection -1 = left, 1 = right
        // direction -1 = up, 0 = horizontal, 1 = down

        FlipSword();
        swingAnimationPlaying = true;
        animator.SetBool("SwordSwing", true);

        StartCoroutine(MoveSwordToPlayer(xOffset, yOffset));

        yield return new WaitForSeconds(animationTime);

        spriteRenderer.flipX = false;

        swingAnimationPlaying = false;
        spriteRenderer.flipY = false;
        animator.SetBool("SwordSwing", false);
        transform.position = new Vector3(0, 0, 0);
    }

    private void FlipSword()
    {
        if (horizontalDirection == -1 && direction == 0)
        {
            spriteRenderer.flipX = true;
        }
        if (direction != 0 && horizontalDirection == -1)
        {
            spriteRenderer.flipY = true;
        }
    }

    private IEnumerator MoveSwordToPlayer(float xOffset, float yOffset)
    {
        while (swingAnimationPlaying)
        {
            transform.position = player.transform.position + new Vector3(xOffset, yOffset, 0);
            transform.rotation = Quaternion.Euler(0, 0, 90 * direction);
            yield return new WaitForEndOfFrame();
        }
    }
}
