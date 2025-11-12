using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        //Jump
        onFoot.Jump.performed += ctx => motor.Jump();

        //Sprint
        onFoot.Sprint.performed += ctx => motor.StartSprinting();
        onFoot.Sprint.canceled += ctx => motor.StopSprinting();

        //Crouch
        onFoot.Crouch.performed += ctx => motor.StartCrouching();
        onFoot.Crouch.canceled += ctx => motor.StopCrouching();
    }
    void FixedUpdate()
    {
        //tell player motor to move using values from our movement action
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        //tell player look to rotate using values from our look action
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }  

    private void OnDisable()
    {
        onFoot.Disable();
    }
}
