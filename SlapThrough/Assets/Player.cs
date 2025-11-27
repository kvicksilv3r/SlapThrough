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

	public float direction;

	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		HandleVertical();
		HandleHorizontal();

		Vector3 totalVelocity = horizontalVelocity + verticalVelocity;
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
}
