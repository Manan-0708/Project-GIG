using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    private PlayerLook look;
    private WeaponManager weaponManager;

    // new: event other systems can subscribe to
    public event Action OnInteractPerformed;
    // added: reload event
    public event Action OnReloadPerformed;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        weaponManager = GetComponent<WeaponManager>();

        onFoot.Jump.performed += ctx => motor.Jump();

        onFoot.Sprint.performed += ctx => motor.StartSprinting();
        onFoot.Sprint.canceled += ctx => motor.StopSprinting();

        onFoot.Crouch.performed += ctx => motor.StartCrouching();
        onFoot.Crouch.canceled += ctx => motor.StopCrouching();

        onFoot.Shoot.performed += ctx => weaponManager.FireCurrentWeapon();
        onFoot.Melee.performed += ctx => weaponManager.UseMelee();

        onFoot.WeaponScroll.performed += ctx =>
            weaponManager.ScrollSwitch(ctx.ReadValue<float>());

        onFoot.GamepadWeaponNext.performed += ctx => weaponManager.NextWeapon();
        onFoot.GamepadWeaponPrevious.performed += ctx => weaponManager.PreviousWeapon();

        onFoot.Slide.performed += ctx => motor.StartSlide();
        onFoot.Slide.canceled += ctx => motor.StopSlide();

        // new: interact action - make sure the PlayerInput asset has an "Interact" action
        // this will fire when the player presses the Interact button (e.g. F)
        onFoot.Interact.performed += ctx => OnInteractPerformed?.Invoke();

        // added: reload action - make sure the PlayerInput asset has a "Reload" action
        onFoot.Reload.performed += ctx => OnReloadPerformed?.Invoke();
    }

    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    void OnEnable() => onFoot.Enable();
    void OnDisable() => onFoot.Disable();
}
