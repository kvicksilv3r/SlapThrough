using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public CharacterController characterController;

    public float jumpPower = 10f;
    public float gravity = 5f;

    private Vector3 horizontalVelocity;
    private Vector3 verticalVelocity;

    private float joystick;

    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    public float horizontalControl;

    public float groundControl = 1;
    public float airControl = 0.25f;

    public float directionChangeBoost = 2;

    private bool justJumped = false;

    public float deadzone = 0.05f;

    public float direction = 1;

    protected Vector3 lastPosition;

    private bool disabled = false;

    public float visibilityTime = 0.4f;

    public float lastVisibilityToggle = -9999;
    public float lastAttack = -9999;

    public float attackCoolDown = 1f;

    public bool visible = false;

    public Vector3 punchHolsterOrigin;

    public Renderer rend;

    public Transform punchHolster;
    public BoxCollider punchCollider;


    private void Awake()
    {
        OnCreate();
    }

    private void OnCreate()
    {
        SetupMaterial();
        //Disable();
        SetSpawnPosition();
        punchHolsterOrigin = punchHolster.localPosition;
    }

    public void Disable()
    {
        disabled = true;
    }

    public void Enable()
    {
        disabled = false;
    }

    void Start()
    {

    }

    private void SetupMaterial()
    {
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", GameManager.Instance.WhatColorAmI(this));
    }

    public void SetSpawnPosition()
    {
        characterController.enabled = false;
        transform.position = GameManager.Instance.GetSpawnPosition(this);
        characterController.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled)
        {
            return;
        }

        HandleVertical();
        HandleHorizontal();
        MovePlayer();
        HandleVisibility();
        SetFacing();

        if (justJumped)
        {
            justJumped = false;
        }

        if ((transform.position - lastPosition).y == 0 && verticalVelocity.y > 0)
        {
            verticalVelocity = Vector3.zero;
        }
    }

    private void SetFacing()
    {
        punchHolster.localPosition = punchHolsterOrigin * direction;
    }

    private void MovePlayer()
    {
        lastPosition = transform.position;
        Vector3 totalVelocity = horizontalVelocity + verticalVelocity;
        totalVelocity.z = 0;
        characterController.Move(totalVelocity * Time.deltaTime);
    }

    private void HandleVertical()
    {
        if (characterController.isGrounded)
        {
            if (!justJumped)
            {
                verticalVelocity = Vector3.down;
            }
        }

        else
        {
            verticalVelocity += Vector3.down * gravity * Time.deltaTime;
        }
    }

    private void HandleHorizontal()
    {
        if (characterController.isGrounded)
        {
            horizontalControl = groundControl;
        }
        else
        {
            horizontalControl = airControl;
        }

        var directionDifference = Vector3.Dot(horizontalVelocity.normalized, (Vector3.right * direction).normalized);

        var directionalBoost = directionDifference < 0 ? directionChangeBoost : 1;

        if (Mathf.Abs(joystick) > deadzone)
        {
            if (joystick > 0) { direction = 1; }
            if (joystick < 0) { direction = -1; }

            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.right * maxSpeed * direction, acceleration * directionalBoost * horizontalControl * Time.deltaTime);
        }

        else
        {
            //decelerate
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, Vector3.zero, deceleration * horizontalControl * Time.deltaTime);
        }
    }

    private void HandleVisibility()
    {
        var showPlayer = visible || Time.time < (lastVisibilityToggle + visibilityTime);
        rend.enabled = showPlayer;
    }

    IEnumerator ThrowPunch()
    {
        punchCollider.enabled = true;

        yield return new WaitForSeconds(0);

        punchCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != this && other.CompareTag("Player"))
        {
            print("You punched the fucker");
        }
    }

    public void OnJump()
    {
        if (!characterController.isGrounded)
        {
            print("NotGrounded");
            return;
        }

        print("Yump");
        verticalVelocity = Vector3.up * jumpPower;
        justJumped = true;
    }

    public void OnMove(InputValue input)
    {
        joystick = input.Get<Vector2>().x;
    }

    public void OnCrouch(InputValue input)
    {
        lastVisibilityToggle = Time.time;
    }

    public void OnAttack(InputValue input)
    {
        if (Time.time > lastAttack + attackCoolDown)
        {
            StartCoroutine(ThrowPunch());
        }
    }
}
