using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CameraRotation camera;
    CharacterController characterController;
    Animator animator;
    PlayerInput playerInput;

    Vector2 currentMovementInput;
    Vector3 currentMovement;
    Vector3 currentRunMovemnt;

    
    float rotationFactorPerFrame = 1.5f;
    float speed = 3.0f;
    float speedRun = 1.5f;
    bool isMovementPressed;
    bool isRunPressed;

    void Awake()
    {
        playerInput = new PlayerInput();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();


        playerInput.CharacterControls.Move.started += onMovementInput;
        playerInput.CharacterControls.Move.canceled += onMovementInput;
        playerInput.CharacterControls.Move.performed += onMovementInput;

        playerInput.CharacterControls.Run.started += onRun;
        playerInput.CharacterControls.Run.canceled += onRun;

    }
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        currentRunMovemnt.x = currentMovementInput.x * speedRun;
        currentRunMovemnt.z = currentMovementInput.y * speedRun;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }
    void onRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();

    }
    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0f;
        positionToLookAt.z = currentMovement.z;

        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }
    }
    void handleAnimation()
    {
        bool isWalking = animator.GetBool("IsWalking");
        bool isRunning = animator.GetBool("IsRunning");
        if(isMovementPressed && !isWalking)
        {
            animator.SetBool("IsWalking", true);
        }
        else if(!isMovementPressed && isWalking)
        {
            animator.SetBool("IsWalking", false);
        }

        if(isRunPressed && !isRunning)
        {
            animator.SetBool("IsRunning", true);
        }
        else if(!isRunPressed && isRunning)
        {
            animator.SetBool("IsRunning", false);
        }
    }


    void Update()
    {
        
        handleRotation();
        handleAnimation();
        if (isRunPressed)
        {
            characterController.Move(currentRunMovemnt * Time.deltaTime * speed);
        }
        characterController.Move(currentMovement * Time.deltaTime * speed);
        
    }

    void OnEnable()
    {
        playerInput.Enable();
    }

    void OnDisable()
    {
        playerInput.Disable();
    }
}

