using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Collider2D = UnityEngine.Collider2D;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerBehaviour : MonoBehaviour
{
    private const float gravityValue = -9.81f;
    private PlayerControls playerControls;
    public static PlayerBehaviour Instance;

    #region Private Variables
    #region PlayerComponents
    private Animator animator;
    private Rigidbody2D rigidBody;
    private PlayerSounds playerSounds;
    #endregion

    #region Movement variables
    private Vector2 moveDirection;
    private float initialGravityScale;
    private bool isMoving;
    private bool isJumping;
    private bool canJump;
    #endregion

    #region Combat variables
    private bool canAttack;
    private bool isAttacking;
    #endregion

    #region Animatior variables
    private int isMovingAnimatorHash;
    private int isJumpingAnimatorHash;
    private int attackAnimatorHash;
    #endregion
    #endregion

    #region SerializedField Variables
    [SerializeField] private float velocity;

    [Header("Jump properties")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpFallGravityScale = 3f;
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private LayerMask groundLayer;

    [Header("Attack properties")] 
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask attackMask;
    #endregion
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        GetPlayerComponents();
        SetInputParameters();
        GetAnimatorParametersHash();

        initialGravityScale = rigidBody.gravityScale;
    }

    private void Update()
    {
        MovePlayer();
        AnimatePlayer();
        GravityHandler();
    }

    private void MovePlayer()
    {
        if (moveDirection == null) return;

        if (moveDirection.x > 0)
        {
            transform.rotation = quaternion.identity;
        }
        else if (moveDirection.x < 0)
        {
            transform.rotation = new Quaternion(0, -180, 0, 0);
        }

        moveDirection.x = playerControls.Movement.Move.ReadValue<float>();
        rigidBody.velocity = new Vector2(moveDirection.x, 0) * velocity;
        isMoving = moveDirection.x != 0;
    }

    private void HandleJump(InputAction.CallbackContext inputContext)
    {
        isJumping = inputContext.ReadValueAsButton();
        print(IsGrounded());
        if (isJumping == true && canJump == true)
        {
            rigidBody.velocity += Vector2.up * jumpForce;
            canJump = false;
            canAttack = false;
            playerSounds.PlayJumpSound();
        }
    }

    private void HandleAttack(InputAction.CallbackContext inputContext)
    {
        isAttacking = inputContext.ReadValueAsButton();
        if (isAttacking == true && canAttack == true)
        {
            animator.SetTrigger(attackAnimatorHash);
        }
    }

    private void AttackHandler()
    {
        Collider2D[] hittedEnemies = Physics2D.OverlapCircleAll(hitPoint.position, attackRange, attackMask);
        print($"Hitted {hittedEnemies.Length} enemies and killed them");
        foreach (Collider2D hittedEnemie in hittedEnemies)
        {
            if (hittedEnemie.GetComponent<EnemyBehaviour>())
            {
                hittedEnemie.GetComponent<EnemyBehaviour>().PlayDeathSound();
            }
        }
    }

    private void AnimatePlayer()
    {
        if (isMoving && animator.GetBool(isMovingAnimatorHash) == false)
        {
            animator.SetBool(isMovingAnimatorHash, true);
        }
        else if (isMoving == false && animator.GetBool(isMovingAnimatorHash) == true)
        {
            animator.SetBool(isMovingAnimatorHash, false);
        }

        if (isJumping == true && animator.GetBool(isJumpingAnimatorHash) == false)
        {
            animator.SetBool(isJumpingAnimatorHash, true);
        }
        else if (animator.GetBool(isJumpingAnimatorHash) == true && isJumping == false && canJump == true)
        {
            animator.SetBool(isJumpingAnimatorHash, false);
        }
    }

    private void GetPlayerComponents()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        playerSounds = GetComponent<PlayerSounds>();
    }

    private void SetInputParameters()
    {
        playerControls = new PlayerControls();

        playerControls.Movement.Jump.started += HandleJump;
        playerControls.Movement.Jump.canceled += HandleJump;

        playerControls.Combat.SimpleAttack.started += HandleAttack;
        playerControls.Combat.SimpleAttack.canceled += HandleAttack;
    }

    private void GetAnimatorParametersHash()
    {
        isMovingAnimatorHash = Animator.StringToHash("isMoving");
        isJumpingAnimatorHash = Animator.StringToHash("isJumping");
        attackAnimatorHash = Animator.StringToHash("attack");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.collider.CompareTag("Floor"))
        {
            canJump = true;
            canAttack = true;
        }
    }

    public Vector2 GetPlayerPosition()
    {
        return transform.position;
    }

    private bool IsGrounded()
    {
        Collider2D[] hittedThings = Physics2D.OverlapCircleAll(groundCheckPos.position, 0.1f, groundLayer);
        foreach (Collider2D thing in hittedThings)
        {
            if (thing.gameObject.layer == groundLayer)
            {
                return true;
            }
        }
        return false;
    }

    #region OnEnable/Disable Functions
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Movement.Jump.started -= HandleJump;
        playerControls.Movement.Jump.canceled -= HandleJump;

        playerControls.Combat.SimpleAttack.started -= HandleAttack;
        playerControls.Combat.SimpleAttack.canceled -= HandleAttack;
        playerControls.Disable();
    }

    #endregion

    private void GravityHandler()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (hitPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
    }
}
