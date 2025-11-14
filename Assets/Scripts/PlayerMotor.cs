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

    [Header("Slide Settings")]
    public float slideSpeed = 10f;
    public float slideDuration = 0.6f;
    private bool isSliding = false;
    private float slideTimer;


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

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;

            // Move forward faster
            controller.Move(transform.forward * speed * Time.deltaTime);

            // smooth slowdown
            speed = Mathf.Lerp(speed, originalSpeed, Time.deltaTime * 3f);

            if (slideTimer <= 0)
            {
                StopSlide();
            }

            return; // do NOT process normal movement
        }


        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        //gravity
        playerVelocity.y += gravity * Time.deltaTime;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; //small negative value to keep grounded
        }

        controller.Move(playerVelocity * Time.deltaTime);
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
        if (!isCrouching)
            speed = sprintSpeed;
    }

    public void StopSprinting()
    {
        if (!isCrouching)
            speed = originalSpeed;
    }

    public void StartCrouching()
    {
        controller.height = crouchHeight;
        speed = crouchSpeed;
        isCrouching = true;
    }

    public void StopCrouching()
    {
        controller.height = originalHeight;
        speed = originalSpeed;
        isCrouching = false;
    }

    public void StartSlide()
    {
        // Only slide if moving & sprinting
        if (isGrounded && speed == sprintSpeed)
        {
            isSliding = true;
            slideTimer = slideDuration;
            speed = slideSpeed;   // temporary boost

            StartCrouching();     // slide = crouch animation
        }
    }

    public void StopSlide()
    {
        if (isSliding)
        {
            isSliding = false;
            speed = originalSpeed;
            StopCrouching();
        }
    }


    // ---- Helper Functions ----

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public bool IsMoving()
    {
        Vector3 flatVel = new Vector3(controller.velocity.x, 0, controller.velocity.z);
        return flatVel.magnitude > 0.2f;
    }

    public bool IsSprinting()
    {
        return speed == sprintSpeed;
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }
}
