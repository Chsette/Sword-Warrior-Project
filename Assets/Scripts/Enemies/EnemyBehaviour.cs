using System;
using System.Collections;
using UnityEngine;
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField, Range(0f, 10f)] private float velocity;
    [SerializeField] private float distanceThreshold;
    [SerializeField] private AudioClip[] enemySounds;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, PlayerBehaviour.Instance.GetPlayerPosition());
        if (distanceFromPlayer <= distanceThreshold)
        {
            SetIsMovingAnimParameter(true);
            Vector2 playerPosition = PlayerBehaviour.Instance.GetPlayerPosition();
            transform.position = Vector2.MoveTowards(transform.position, playerPosition, velocity * Time.deltaTime);
            print("enemy move: " + transform.position.magnitude);
        }

        if (rigidbody.velocity.magnitude == 0)
        {
            SetIsMovingAnimParameter(false);
        }

        CheckEnemySprite();
        print("enemy sprite velocity: " + rigidbody.velocity.x);
    }

    private void CheckEnemySprite()
    {
        if (rigidbody.velocity.normalized.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (rigidbody.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void SetIsMovingAnimParameter(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }

    private void StartDeathAnim()
    {
        animator.SetTrigger("onDeath");
    }

    private void PlayWalkSound()
    {
        audioSource.Stop();
        audioSource.clip = enemySounds[0];
        audioSource.Play();
    }

    public void PlayDeathSound()
    {
        audioSource.Stop();
        audioSource.clip = enemySounds[1];
        audioSource.Play();
        StartCoroutine(Die());
        StartDeathAnim();
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(0.35f);
        Destroy(this.gameObject);
    }
}
