using System;
using System.Collections;
using UnityEngine;

public enum WalkingMode { Idle = 0, Walk = 1, Run = 2, Sprint = 3, Backstep = 4 }
public class PlayerLogic : MonoBehaviour
{
    //Public props.
    public float speed = 1.0f;
    public float runSpeed = 2.5f;
    public float jumpSpeed = 2.0f;
    public float gravity = 20.0f;
    public float rotateSpeed = 200.0f;
    //Private props.
    private WalkingMode currentWalkingMode;
    private Animator _animator;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    //private Rigidbody rbody;
    bool enterAttackMode = false;
    bool jumping = false;

    void Start()
    {
        _animator = GetComponent<Animator>();
       controller = GetComponent<CharacterController>();
       // rbody = GetComponent<Rigidbody>();
        // let the gameObject fall down
        //gameObject.transform.position = new Vector3(0, 5, 0);
    }

    #region Actions Dictionary

    //W - forth
    //S - back
    //A - left
    //D - right
    //Space - jump
    //Mouse Right Click - enter attack mode, kick attack
    //Q - in attack mode only, special punch attack

    #endregion

    void Update()
    {
        //if (controller.isGrounded)
        //{
        if (enterAttackMode && Input.GetKeyDown(KeyCode.Q))
        {
            _animator.SetTrigger("special_punch_attack");
        }

        if (Input.GetMouseButtonDown(1))
        {
            enterAttackMode = true;

            _animator.SetTrigger("idle_attack");
            _animator.SetTrigger("kick_attack");
        }
        else
        {
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A)
                || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                enterAttackMode = false;
                _animator.SetTrigger("idle_normal");
            }

            if (!enterAttackMode)
            {
                SetPlayerWalkingMode();

                moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
                if (currentWalkingMode == WalkingMode.Backstep)
                    moveDirection = new Vector3(0, 0, -0.2f);
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= currentWalkingMode == WalkingMode.Idle ? 0 :
                                 currentWalkingMode == WalkingMode.Run ? speed :
                                 currentWalkingMode == WalkingMode.Sprint ? runSpeed : 1;

                if (Input.GetButton("Jump"))
                {
                    if (!jumping)
                    {
                        jumping = true;
                        _animator.SetTrigger("jump");
                        moveDirection.y = jumpSpeed;

                        StartCoroutine(ExecuteAfterTime(1, () => jumping = false));
                    }
                }

                moveDirection.y -= gravity * Time.deltaTime;
                //rbody.MovePosition(moveDirection * Time.deltaTime);
                controller.Move(moveDirection * Time.deltaTime);

                transform.Rotate(0, Input.GetAxis("Horizontal") * rotateSpeed * Time.deltaTime, 0);
                moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);
                //rbody.MovePosition(moveDirection * Time.deltaTime);
                controller.Move(moveDirection * Time.deltaTime);
            }
        }
        //}
    }

    private void SetPlayerWalkingMode()
    {
        var wPressed = Input.GetKey(KeyCode.W);
        var sPressed = Input.GetKey(KeyCode.S);
        var leftShiftPressed = Input.GetKey(KeyCode.LeftShift);

        if (wPressed && leftShiftPressed)
        {
            currentWalkingMode = WalkingMode.Sprint;
        }
        else if (wPressed)
        {
            currentWalkingMode = WalkingMode.Run;
        }
        else if (sPressed)
        {
            currentWalkingMode = WalkingMode.Backstep;
        }
        else
        {
            currentWalkingMode = WalkingMode.Idle;
        }

        _animator.SetInteger("walking_mode", (int)currentWalkingMode);
    }

    private IEnumerator ExecuteAfterTime(float time, Action callback)
    {
        yield return new WaitForSeconds(time);
        callback.Invoke();
    }
}