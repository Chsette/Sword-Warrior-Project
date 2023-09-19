using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerBehaviour : MonoBehaviour
{
    private PlayerControls playerControls;
#region Private Variables
    #region PlayerComponents
    private Animator animator;
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    #endregion

    private Vector2 moveDirection;
    private bool isMoving;

    #region Animatior variables
    private int isMovingAnimationHash;
    #endregion
#endregion

    #region SerializedField Variables
    [SerializeField] private float velocity;

    #endregion


    private void Awake()
    {        
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
            spriteRenderer.flipX = false;
        }
        else if(moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        transform.Translate(moveDirection * velocity * Time.deltaTime);
    }

    private void AnimatePlayer()
    {
        if (isMoving && animator.GetBool(isMovingAnimationHash) == false)
        {
            animator.SetBool(isMovingAnimationHash, true);
        }
        else if(isMoving == false &&  animator.GetBool(isMovingAnimationHash) == true)
        {
            animator.SetBool(isMovingAnimationHash, false);
        }
    }

    private void GetPlayerComponents()
    {
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void SetInputParameters()
    {
        playerControls = new PlayerControls();
        playerControls.Movement.Move.started += GetInputInfo;
        playerControls.Movement.Move.performed += GetInputInfo;
        playerControls.Movement.Move.canceled += GetInputInfo;
    }

    private void GetAnimatorParametersHash()
    {
        isMovingAnimationHash = Animator.StringToHash("isMoving");
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
