using System.Collections;
using UnityEngine;
public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 movePosition;
    [SerializeField, Range(0f, 10f)] private float velocity;
    [SerializeField] private float distanceThreshold;
    [SerializeField] private AudioClip[] enemySounds;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private AudioSource audioSource;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, PlayerBehaviour.Instance.GetPlayerPosition());
        if (distanceFromPlayer <= distanceThreshold)
        {
            SetIsMovingAnimParameter(true);
            Vector2 playerPosition = PlayerBehaviour.Instance.GetPlayerPosition();
            //Vector2.MoveTowards(transform.position, playerPosition, 0.5f);
            transform.Translate(new Vector2(playerPosition.x, 0).normalized * velocity * Time.deltaTime * -1);
        }

        if (rigidbody.velocity.magnitude == 0)
        {
            SetIsMovingAnimParameter(false);
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
