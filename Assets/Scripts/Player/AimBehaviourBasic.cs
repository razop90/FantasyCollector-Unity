using UnityEngine;
using System.Collections;

// AimBehaviour inherits from GenericBehaviour. This class corresponds to aim and strafe behaviour.
public class AimBehaviourBasic : GenericBehaviour
{
    public string aimButton = "Aim";
    public Texture2D crosshair;
    public ParticleSystem crosshairEffect;
    public float aimTurnSmoothing = 0.15f;                                // Speed of turn response when aiming to match camera facing.
    public Vector3 aimPivotOffset = new Vector3(0.8f, 3.5f, 1f);         // Offset to repoint the camera when aiming.
    public Vector3 aimCamOffset = new Vector3(0f, 0.4f, -0.7f);         // Offset to relocate the camera when aiming.
    public bool disableAim;

    private int aimBool;                                                  // Animator variable related to aiming.
    public bool aim { get; private set; }                                                     // Boolean to determine whether or not the player is aiming.
    public bool isAimEnabled = true;
    public float rotAngle = 0f;
    public float rotSpeed = 130f;

    // Start is always called after any Awake functions.
    void Start()
    {
        // Set up the references.
        aimBool = Animator.StringToHash("Aim");
    }

    // Update is used to set features regardless the active behaviour.
    private void Update()
    {
        if (GameManager.instance.isGameOperational)
        {
            if (disableAim)
            {
                if (aim)
                    StartCoroutine(ToggleAimOff());
                return;
            }

            rotAngle += rotSpeed * Time.deltaTime;
            if (rotAngle > 360)
                rotAngle = 0f;

            if (!isAimEnabled)
            {
                StartCoroutine(ToggleAimOff());
                return;
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (aim)
                {
                    StartCoroutine(ToggleAimOff());
                }
                else
                {
                    StartCoroutine(ToggleAimOn());
                }
            }

            // No sprinting while aiming.
            canSprint = !aim;

            // Set aim boolean on the Animator Controller.
            behaviourManager.GetAnim.SetBool(aimBool, aim);
        }
    }

    private IEnumerator mouseDown()
    {
        while (Input.GetMouseButton(0))
        {
            print("mouse button held down");
            yield return null;  // Give up control  
        }
    }

    // Co-rountine to start aiming mode with delay.
    private IEnumerator ToggleAimOn()
    {
        yield return new WaitForSeconds(0.05f);
        // Aiming is not possible.
        if (behaviourManager.GetTempLockStatus(this.behaviourCode) || behaviourManager.IsOverriding(this))
            yield return false;

        // Start aiming.
        else
        {
            aim = true;
            int signal = 1;
            aimCamOffset.x = Mathf.Abs(aimCamOffset.x) * signal;
            aimPivotOffset.x = Mathf.Abs(aimPivotOffset.x) * signal;
            yield return new WaitForSeconds(0.1f);
            behaviourManager.GetAnim.SetFloat(speedFloat, 0);
            // This state overrides the active one.
            behaviourManager.OverrideWithBehaviour(this);
        }
    }

    // Co-rountine to end aiming mode with delay.
    public IEnumerator ToggleAimOff()
    {
        yield return new WaitForSeconds(0.3f);
        if (!Input.GetMouseButton(1))
        {
            aim = false;
            behaviourManager.cameraInfo.ResetTargetOffsets();
            behaviourManager.cameraInfo.ResetMaxVerticalAngle();
            yield return new WaitForSeconds(0.05f);
            behaviourManager.RevokeOverridingBehaviour(this);
        }
    }

    // LocalFixedUpdate overrides the virtual function of the base class.
    public override void LocalFixedUpdate()
    {
        if (GameManager.instance.isGameOperational)
        {
            // Set camera position and orientation to the aim mode parameters.
            if (aim)
                behaviourManager.cameraInfo.SetTargetOffsets(aimPivotOffset, aimCamOffset);
        }
    }

    // LocalLateUpdate: manager is called here to set player rotation after camera rotates, avoiding flickering.
    public override void LocalLateUpdate()
    {
        if (GameManager.instance.isGameOperational)
        {
            AimManagement();
        }
    }

    // Handle aim parameters when aiming is active.
    private void AimManagement()
    {
        // Deal with the player orientation when aiming.
        Rotating();
    }

    // Rotate the player to match correct orientation, according to camera.
    private void Rotating()
    {
        Vector3 forward = behaviourManager.playerCamera.TransformDirection(Vector3.forward);
        // Player is moving on ground, Y component of camera facing is not relevant.
        forward.y = 0.0f;
        forward = forward.normalized;

        // Always rotates the player according to the camera horizontal rotation in aim mode.
        Quaternion targetRotation = Quaternion.Euler(0, behaviourManager.cameraInfo.GetH, 0);

        float minSpeed = Quaternion.Angle(transform.rotation, targetRotation) * aimTurnSmoothing;

        // Rotate entire player to face camera.
        behaviourManager.SetLastDirection(forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, minSpeed * Time.deltaTime);

    }

    // Draw the crosshair when aiming.
    private void OnGUI()
    {
        if (crosshair)
        {
            float mag = behaviourManager.cameraInfo.GetCurrentPivotMagnitude(aimPivotOffset);
            if (mag < 0.05f)
            {
                Vector2 pivot = new Vector2(Screen.width / 2, Screen.height / 2);
                GUIUtility.RotateAroundPivot(rotAngle % 360, pivot);
                GUI.DrawTexture(new Rect(Screen.width / 2 - (crosshair.width * 0.5f),
                                         Screen.height / 2 - (crosshair.height * 0.5f),
                                         crosshair.width, crosshair.height), crosshair);
            }
        }
    }
}
