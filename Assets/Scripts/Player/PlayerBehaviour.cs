using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
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
    [FormerlySerializedAs("jumpFallGravityScale")] 
    [SerializeField, Range(1f, 10f)] private float jumpFallGravityMultiplier = 3f;
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
        rigidBody.velocity = new Vector2(moveDirection.x * velocity, rigidBody.velocity.y);
        isMoving = moveDirection.x != 0;
    }

    private void HandleJump(InputAction.CallbackContext inputContext)
    {
        if (IsGrounded())
        {
            rigidBody.velocity += Vector2.up * jumpForce;
            isJumping = true;
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
        foreach (Collider2D hittedEnemie in hittedEnemies)
        {
            if (hittedEnemie.TryGetComponent(out EnemyBehaviour enemy))
            {
                enemy.PlayDeathSound();
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

        if (!IsGrounded() && animator.GetBool(isJumpingAnimatorHash) == false)
        {
            animator.SetBool(isJumpingAnimatorHash, true);
        }
        else if (animator.GetBool(isJumpingAnimatorHash) == true && IsGrounded())
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

    public Vector2 GetPlayerPosition()
    {
        return transform.position;
    }

    private void GravityHandler()
    {
        if (IsGrounded())
        {
            isJumping = false;
            canAttack = true;
            rigidBody.gravityScale = initialGravityScale;
        }
        else if (isJumping && rigidBody.velocity.y < 0f)
        {
            print("increasing gravity velocity");
            isJumping = true;
            rigidBody.gravityScale = initialGravityScale * jumpFallGravityMultiplier;
        }
        else
        {
            isJumping = true;
        }
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

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.OverlapBox(groundCheckPos.position, new Vector2(1, 0.2f), groundLayer);
        return isGrounded;
    }

    private void OnDrawGizmos()
    {
        if (hitPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheckPos.position, new Vector3(1, 0.2f));
    }
}
