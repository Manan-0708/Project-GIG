using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    public float walkInterval = 0.5f;  // time between steps
    public float sprintInterval = 0.35f;
    public float crouchInterval = 0.8f;

    private PlayerMotor motor;
    private float timer;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        timer = 0f;
    }

    void Update()
    {
        // must be grounded & moving
        if (!motor.IsGrounded() || !motor.IsMoving())
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            PlayFootstep();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Pressed T");

            if (footstepClips.Length == 0)
                Debug.Log("NO FOOTSTEP CLIPS");

            if (audioSource == null)
                Debug.Log("AUDIO SOURCE MISSING");

            if (footstepClips.Length > 0 && audioSource != null)
            {
                Debug.Log("Trying to play sound...");
                audioSource.PlayOneShot(footstepClips[0]);
            }
        }


        void PlayFootstep()
        {
            if (footstepClips.Length == 0) return;

            // pick a random sound
            AudioClip clip = footstepClips[Random.Range(0, footstepClips.Length)];

            // random pitch variation
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            audioSource.PlayOneShot(clip);

            // set interval depending on movement state
            if (motor.IsSprinting()) timer = sprintInterval;
            else if (motor.IsCrouching()) timer = crouchInterval;
            else timer = walkInterval;
        }
    }
}