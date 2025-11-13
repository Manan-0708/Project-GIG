using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCameraEffects : MonoBehaviour
{
    [Header("References")]
    public Transform cameraHolder; //transform we move for bobbing
    public PlayerMotor motor; // reference to player motor script
    public Camera cam; //main camera

    [Header("Bobbing Settings")]
    public float bobFrequency = 8f; //how fast the bobbing is
    public float bobHorizontal = .04f; //sideways amplitude
    public float bobVertical = .06f; //up and down amplitude
    public float bobSmoothing = 10f; //how quickly camera catches up 
    public float sprintBobMultiplier = 1.5f; //how much faster bobbing is when sprinting
    public float crouchBobMultiplier = 0.5f; //how much slower bobbing is when crouching

    [Header("FOV Settings")]
    public float baseFOV = 75f; //default FOV
    public float sprintFOV = 90f; //FOV when sprinting
    public float crouchFOV = 65f; //FOV when crouching
    public float fovSmoothing = 6f; //how quickly FOV changes

    //internal states
    private Vector3 startLocalPos;
    private float bobTimer = 0f;
    private float targetFOV;

    private void Start()
    {
        if ( cam == null)
            cam = GetComponent<Camera>(); //safeguard

        //record original local position

        if (cameraHolder== null)
            cameraHolder = this.transform;
        startLocalPos = cameraHolder.localPosition;
        targetFOV = baseFOV;
        if (cameraHolder != null) cam.fieldOfView = baseFOV;
    }

    void Update()
    {
        HandleHeadBob();
        HandleFOV();
    }

    void HandleHeadBob()
    {
        //if not moving, or not grounded -> smoothly return to start position
        if (!motor.IsGrounded() || !motor.IsMoving())   
        {
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, startLocalPos, bobSmoothing * Time.deltaTime);
            //reset timer gradulally so bob wave restarts smoothly when moving again
            bobTimer = Mathf.Lerp(bobTimer, 0, bobSmoothing * Time.deltaTime);
            return;
        }

        //calculate bobbing speed multiplier
        float freq = bobFrequency;
        float hAmp = bobHorizontal;
        float vAmp = bobVertical;

        if (motor.IsSprinting())
        {
            freq *= sprintBobMultiplier;
            hAmp *= sprintBobMultiplier;
            vAmp *= sprintBobMultiplier;
        }
        else if (motor.IsCrouching())
        {
            freq *= crouchBobMultiplier;
            hAmp *= crouchBobMultiplier;
            vAmp *= crouchBobMultiplier;
        }
        //advance bob timer & bob offsets
        bobTimer += Time.deltaTime * freq;
        float horizontal = Mathf.Sin(bobTimer) * hAmp;
        float vertical = Mathf.Cos(bobTimer * 2) * vAmp; //cosine for vertical to offset by 90 degrees and used 'abs' so camera not inverts

        Vector3 targetPos = startLocalPos + new Vector3(horizontal, vertical, 0f);
        cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, targetPos, bobSmoothing * Time.deltaTime);
    }

    void HandleFOV()
    {
        if (motor.IsSprinting()) targetFOV = sprintFOV;
        else if (motor.IsCrouching()) targetFOV = crouchFOV;
        else targetFOV = baseFOV;

        if (cam != null)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, fovSmoothing * Time.deltaTime);
    }


}
