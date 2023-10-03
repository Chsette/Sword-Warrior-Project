using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerBehaviour : MonoBehaviour
{
    private PlayerControls playerControls;
    public static PlayerBehaviour instance;
    #region Private Variables
    #region PlayerComponents
    private Animator animator;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private PlayerSounds playerSounds;
    #endregion

    #region Movement variables
    private Vector2 moveDirection;
    private bool isMoving;
    private bool isJumping;
    private bool canJump;
    #endregion

    #region Combat
    private bool canAttack;
    private bool isAttacking;
    #endregion

    #region Animatior variables
    private int isMovingAnimatorHash;
    private int isJumpingAnimatorHash;
    private int attackAnimatorHash;
    #endregion
#endregion

    #region SerializeField Variables
    [SerializeField] private float velocity;
    [SerializeField] private float jumpForce;

    [FormerlySerializedAs("attackPoint")]
    [Header("Attack Properties")] 
    [SerializeField] private Transform hitPoint;
    [SerializeField] private float attackRange;
    [SerializeField] private LayerMask atttackMask;
    #endregion

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        GetPlayerComponents();
        SetInputParameters();
        GetAnimatorParametersHash();
    }

    private void Update()
    {
        MovePlayer();
        AnimatePlayer();
    }
    
    private void GetInputInfo(InputAction.CallbackContext inputContext)
    {
        moveDirection.x = inputContext.ReadValue<float>();
        isMoving = moveDirection.x != 0;
    }

    private void MovePlayer()
    {
        if(moveDirection.x > 0)
        {
            transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if(moveDirection.x < 0)
        {
            transform.rotation = new Quaternion(0, -180, 0, 0);
        }
        transform.Translate(moveDirection * (velocity * Time.deltaTime));
    }

    private void HandleJump(InputAction.CallbackContext inputContext)
    {
        isJumping = inputContext.ReadValueAsButton();
        if(isJumping == true && canJump == true)
        {
            rigidBody.AddForce(Vector2.up * jumpForce);
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

    private void AnimatePlayer()
    {
        if (isMoving && animator.GetBool(isMovingAnimatorHash) == false)
        {
            animator.SetBool(isMovingAnimatorHash, true);
        }
        else if(isMoving == false &&  animator.GetBool(isMovingAnimatorHash) == true)
        {
            animator.SetBool(isMovingAnimatorHash, false);
        }

        if(isJumping == true && animator.GetBool(isJumpingAnimatorHash) == false)
        {
            animator.SetBool(isJumpingAnimatorHash, true);
        }
        else if(animator.GetBool(isJumpingAnimatorHash) == true && isJumping == false && canJump == true)
        {
            animator.SetBool(isJumpingAnimatorHash, false);
        }
    }

    private void GetPlayerComponents()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerSounds = GetComponent<PlayerSounds>();
    }

    private void SetInputParameters()
    {
        playerControls = new PlayerControls();
        playerControls.Movement.Move.started += GetInputInfo;
        playerControls.Movement.Move.performed += GetInputInfo;
        playerControls.Movement.Move.canceled += GetInputInfo;

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
        TilemapRenderer tileRenderer = collision.collider.GetComponent<TilemapRenderer>();
        if(tileRenderer.sortingLayerName == "Enviroment")
        {
            canJump = true;
            canAttack = true;
        }
    }

    public Vector2 GetPlayerPosision()
    {
        return transform.position;
    }

    private void OnDrawGizmos()
    {
        if (hitPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitPoint.position, attackRange);
    }

    #region OnEnable/Disable Functions   
    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Movement.Move.started -= GetInputInfo;
        playerControls.Movement.Move.performed -= GetInputInfo;
        playerControls.Movement.Move.canceled -= GetInputInfo;
        playerControls.Disable();  
    }
    #endregion
}
