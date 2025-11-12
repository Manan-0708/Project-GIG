using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    private bool isGrounded;
    public float speed = 5.0f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    //Sprint and crouch variables 
    public float sprintSpeed = 8.0f;
    public float crouchSpeed = 2.5f;
    private float originalSpeed;

    private float originalHeight;
    public float crouchHeight = 1.0f;
    private bool isCrouching = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        originalHeight = controller.height;
        originalSpeed = speed;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        
    }
    // recieve inputs from Input Manager.cs and apply them to character controller
    public void ProcessMove(Vector2 input)
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        //gravity
        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; //small negative value to keep grounded
        }
        controller.Move(playerVelocity * Time.deltaTime);
        Debug.Log(playerVelocity.y);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void StartSprinting()
    {
        speed = sprintSpeed;
    }

    public void StopSprinting()
    {
        speed = originalSpeed;
    }
    public void StartCrouching()
    {
        if (!isCrouching)
        {
            controller.height = crouchHeight;
            speed = crouchSpeed;
            isCrouching = true;
        }
    }
    public void StopCrouching()
    {
        if (isCrouching)
        {
            controller.height = originalHeight;
            speed = originalSpeed;
            isCrouching = false;
        }
    }
}
