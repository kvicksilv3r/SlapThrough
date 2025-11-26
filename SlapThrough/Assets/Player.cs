using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public CharacterController characterController;

    public float jumpPower = 10f;
    public float gravity = 5f;

    private float horizontalVelocity;
    private float verticalVelocity;

    private float joystick;

    public float acceleration;
    public float deceleration;
    public float maxSpeed;

    public float horizontalControl;

    public float groundControl = 1;
    public float airControl = 0.5f;

    private bool justJumped = false;

    public float deadzone = 0.05f;

    public float direction;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleVertical();
        HandleHorizontal();

        Vector3 totalVelocity = new Vector3(horizontalVelocity, verticalVelocity);
        characterController.Move(totalVelocity * Time.deltaTime);

        if (justJumped)
        {
            justJumped = false;
        }
    }

    private void HandleVertical()
    {
        if (characterController.isGrounded)
        {
            if (!justJumped)
            {
                verticalVelocity = -1;
            }
        }

        else
        {
            verticalVelocity += -1 * gravity * Time.deltaTime;
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

        if (Mathf.Abs(joystick) > deadzone)
        {
            if (joystick > 0) { direction = 1; }
            if (joystick < 0) { direction = -1; }

            Vector3 test = Vector3.MoveTowards(new Vector3(horizontalVelocity, 0), Vector3.right * maxSpeed * direction, acceleration * horizontalControl * Time.deltaTime);
            horizontalVelocity = test.x;
        }

        else
        {
            //decelerate
            Vector3 test = Vector3.MoveTowards(new Vector3(horizontalVelocity, 0), Vector3.zero, deceleration * horizontalControl * Time.deltaTime);
            horizontalVelocity = test.x;
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
        verticalVelocity = jumpPower;
        justJumped = true;
    }

    public void OnMove(InputValue input)
    {
        joystick = input.Get<Vector2>().x;
    }
}
