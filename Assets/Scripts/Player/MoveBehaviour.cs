using UnityEngine;

// MoveBehaviour inherits from GenericBehaviour. This class corresponds to basic walk and run behaviour, it is the default behaviour.
public class MoveBehaviour : GenericBehaviour
{
    public float walkSpeed = 0.15f;                 // Default walk speed.
    public float runSpeed = 1.0f;                   // Default run speed.
    public float sprintSpeed = 2.0f;                // Default sprint speed.
    public float speedDampTime = 0.1f;              // Default damp time to change the animations based on current speed.
    public string jumpButton = "Jump";              // Default jump button.
    public float jumpHeight = 1.5f;                 // Default jump height.
    public float jumpIntertialForce = 10f;          // Default horizontal inertial force when jumping.

    [Header("Sound")]
    public AudioClip walkClip;
    public AudioClip jumpClip;


    private float speed, speedSeeker;               // Moving speed.
    private int jumpBool;                           // Animator variable related to jumping.
    private int groundedBool;                       // Animator variable related to whether or not the player is on ground.
    private bool jump;                              // Boolean to determine whether or not the player started a jump.
    private bool isColliding;                       // Boolean to determine if the player has collided with an obstacle.
    private int idleNormalTrigger;
    #region Old
    //private bool isAttacking;
    //private int idleAttackTrigger;
    //private int kickAttackTrigger;
    //private int specialAttackTrigger;
    #endregion
    private AimBehaviourBasic aimBehaviour;
    private AudioSource audio;

    // Start is always called after any Awake functions.
    void Start()
    {
        // Set up the references.
        jumpBool = Animator.StringToHash("Jump");
        groundedBool = Animator.StringToHash("Grounded");
        idleNormalTrigger = Animator.StringToHash("idle_normal");
        #region Old
        //idleAttackTrigger = Animator.StringToHash("idle_attack");
        //kickAttackTrigger = Animator.StringToHash("kick_attack");
        //specialAttackTrigger = Animator.StringToHash("special_punch_attack");
        #endregion
        aimBehaviour = GetComponent<AimBehaviourBasic>();
        audio = GetComponent<AudioSource>();

        behaviourManager.GetAnim.SetBool(groundedBool, true);

        // Subscribe and register this behaviour as the default behaviour.
        behaviourManager.SubscribeBehaviour(this);
        behaviourManager.RegisterDefaultBehaviour(this.behaviourCode);
        speedSeeker = runSpeed;
    }

    // Update is used to set features regardless the active behaviour.
    void Update()
    {
        if (GameManager.instance.isGameOperational)
        {
            // Get jump input.
            //!isAttacking &&
            if (!jump && Input.GetButtonDown(jumpButton) && behaviourManager.IsCurrentBehaviour(this.behaviourCode) && !behaviourManager.IsOverriding())
            {
                jump = true;
            }
        }
        else
        {
            AudioHandler(false);
        }
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        if (GameManager.instance.isGameOperational)
        {
            #region Old

            //if (!aimBehaviour.aim && Input.GetAxisRaw(aimBehaviour.aimButton) == 0)
            //{
            //    if (isAttacking && Input.GetKeyDown(KeyCode.Q))
            //    {
            //        behaviourManager.GetAnim.SetTrigger(specialAttackTrigger);
            //    }
            //    else if (!isAttacking && Input.GetMouseButtonDown(0))
            //    {
            //        isAttacking = true;
            //        behaviourManager.GetAnim.SetTrigger(idleAttackTrigger);
            //        behaviourManager.GetAnim.SetTrigger(kickAttackTrigger);
            //    }
            //    else if (isAttacking && Input.anyKeyDown)
            //    {
            //        isAttacking = false;
            //    }
            //}
            //else
            //{
            //    isAttacking = false;
            //}

            //if (!isAttacking)
            //{

            #endregion

            behaviourManager.GetAnim.SetTrigger(idleNormalTrigger);
            // Call the basic movement manager.
            MovementManagement(behaviourManager.GetH, behaviourManager.GetV);

            // Call the jump manager.
            JumpManagement();
            //}

            if (audio.isPlaying && audio.clip.name == jumpClip.name)
            {
                return;
            }
            if (behaviourManager.IsMoving() && !behaviourManager.GetAnim.GetBool(jumpBool))
            {
                if (!audio.isPlaying)
                {
                    AudioHandler(true, walkClip, 0.2f);
                }
            }
            else if ((audio.clip != null && audio.clip.name == walkClip.name) || behaviourManager.GetAnim.GetBool(jumpBool))
            {
                AudioHandler(false);
            }
        }
    }

    // Execute the idle and walk/run jump movements.
    void JumpManagement()
    {
        // Start a new jump.
        if (jump && !behaviourManager.GetAnim.GetBool(jumpBool) && behaviourManager.IsGrounded())
        {
            // Set jump related parameters.
            behaviourManager.LockTempBehaviour(this.behaviourCode);
            behaviourManager.GetAnim.SetBool(jumpBool, true);

            // Is a locomotion jump?
            if (behaviourManager.GetAnim.GetFloat(speedFloat) > 0.1)
            {
                // Temporarily change player friction to pass through obstacles.
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0f;
                GetComponent<CapsuleCollider>().material.staticFriction = 0f;
                // Set jump vertical impulse velocity.
                float velocity = 2f * Mathf.Abs(Physics.gravity.y) * jumpHeight;
                velocity = Mathf.Sqrt(velocity);
                behaviourManager.GetRigidBody.AddForce(Vector3.up * velocity, ForceMode.VelocityChange);

                AudioHandler(true, jumpClip, 1f);
            }
        }
        // Is already jumping?
        else if (behaviourManager.GetAnim.GetBool(jumpBool))
        {
            // Keep forward movement while in the air.
            if (!behaviourManager.IsGrounded() && !isColliding && behaviourManager.GetTempLockStatus())
            {
                behaviourManager.GetRigidBody.AddForce(transform.forward * jumpIntertialForce * Physics.gravity.magnitude * sprintSpeed, ForceMode.Acceleration);
            }
            // Has landed?
            if ((behaviourManager.GetRigidBody.velocity.y < 0) && behaviourManager.IsGrounded())
            {
                behaviourManager.GetAnim.SetBool(groundedBool, true);
                // Change back player friction to default.
                GetComponent<CapsuleCollider>().material.dynamicFriction = 0.6f;
                GetComponent<CapsuleCollider>().material.staticFriction = 0.6f;
                // Set jump related parameters.
                jump = false;
                behaviourManager.GetAnim.SetBool(jumpBool, false);
                behaviourManager.UnlockTempBehaviour(this.behaviourCode);
            }
        }
    }

    // Deal with the basic player movement
    void MovementManagement(float horizontal, float vertical)
    {
        // On ground, obey gravity.
        if (behaviourManager.IsGrounded())
            behaviourManager.GetRigidBody.useGravity = true;

        // Call function that deals with player orientation.
        Rotating(horizontal, vertical);

        // Set proper speed.
        Vector2 dir = new Vector2(horizontal, vertical);
        speed = Vector2.ClampMagnitude(dir, 1f).magnitude;
        // This is for PC only, gamepads control speed via analog stick.
        speedSeeker += Input.GetAxis("Mouse ScrollWheel");
        speedSeeker = Mathf.Clamp(speedSeeker, walkSpeed, runSpeed);
        speed *= speedSeeker;
        if (behaviourManager.IsSprinting())
        {
            speed = sprintSpeed;
        }

        behaviourManager.GetAnim.SetFloat(speedFloat, speed, speedDampTime, Time.deltaTime);
    }

    // Rotate the player to match correct orientation, according to camera and key pressed.
    Vector3 Rotating(float horizontal, float vertical)
    {
        // Get camera forward direction, without vertical component.
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);

        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Calculate target direction based on camera forward and direction key.
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        Vector3 targetDirection;
        targetDirection = forward * vertical + right * horizontal;

        // Lerp current direction to calculated target direction.
        if ((behaviourManager.IsMoving() && targetDirection != Vector3.zero))
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

            Quaternion newRotation = Quaternion.Slerp(behaviourManager.GetRigidBody.rotation, targetRotation, behaviourManager.turnSmoothing);
            behaviourManager.GetRigidBody.MoveRotation(newRotation);
            behaviourManager.SetLastDirection(targetDirection);
        }
        // If idle, Ignore current camera facing and consider last moving direction.
        if (!(Mathf.Abs(horizontal) > 0.9 || Mathf.Abs(vertical) > 0.9))
        {
            behaviourManager.Repositioning();
        }

        return targetDirection;
    }

    public void AudioHandler(bool startNew, AudioClip clip = null, float volume = 1f)
    {
        if (audio.isPlaying)
            audio.Stop();

        if (startNew)
        {
            audio.volume = volume;
            audio.clip = clip;
            audio.PlayOneShot(clip);
        }
    }

    // Collision detection.
    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }
}
