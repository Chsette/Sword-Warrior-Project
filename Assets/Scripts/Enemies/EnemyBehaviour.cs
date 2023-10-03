using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private Vector2 movePosition;
    [SerializeField, Range(0f, 10f)] private float velocity;
    [SerializeField] private float distanceThreshold;

    private Rigidbody2D rigidbody;
    private Animator animator;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float distanceFromPlayer = Vector2.Distance(transform.position, PlayerBehaviour.instance.GetPlayerPosision());
        if (distanceFromPlayer <= distanceThreshold)
        {
            print("moving to player");
            Vector2 playerPosition = PlayerBehaviour.instance.GetPlayerPosision();
            //Vector2.MoveTowards(transform.position, playerPosition, 0.5f);
            transform.Translate(new Vector2(playerPosition.x, 0).normalized * velocity * Time.deltaTime * -1);
        }
    }

    private void SetIsMovingAnimParameter(bool isMoving)
    {
        animator.SetBool("isMoving", isMoving);
    }
}
