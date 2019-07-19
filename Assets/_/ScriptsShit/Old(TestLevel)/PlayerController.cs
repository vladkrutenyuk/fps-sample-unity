using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float speedMove = 3f;
	public float mouseSensitivity = 3f;

	Vector3 velocityMove = Vector3.zero;
	Vector3 vectorRotationY = Vector3.zero;
	float componentRotationX = 0f;
	float currentComponentRotationX = 0f;
	public float maxLimitRotationX = 80f;
	public float minLimitRotationX = -70f;
	public bool SmoothLook;
	float xMove;
	float zMove;
	float rotationY;
	float rotationX;

	public float jumpForce = 5f;
	bool isGrounded;

	public Camera fpsCam;
	Rigidbody rb;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update()
	{
		

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		// Check isGrounded by Raycast
		if(Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0, -1, 0)), 1f))
		{
			isGrounded = true;
		}
        else
        {
			isGrounded = false;
		}

		// Get Vector3 movement of player
		xMove = Input.GetAxisRaw("Horizontal");
		zMove = Input.GetAxisRaw("Vertical");
		Vector3 moveHorizontal = transform.right * xMove;
		Vector3 moveVertical = transform.forward * zMove;

		velocityMove = (moveHorizontal + moveVertical) * speedMove;
		if (Mathf.Abs(velocityMove.magnitude) > speedMove)
		{
			velocityMove = (moveHorizontal + moveVertical).normalized * speedMove;
		}

		// Get Vector3 of LR-rotation for player (by Body rotation)
		// Get Vector3 component of UP-rotation for player (by Camera rotation)
		if(SmoothLook)
		{
			rotationY = Input.GetAxis("Mouse X");
			rotationX = Input.GetAxis("Mouse Y");
		} else
		{
			rotationY = Input.GetAxisRaw("Mouse X");
			rotationX = Input.GetAxisRaw("Mouse Y");
		}
		vectorRotationY = new Vector3(0f, rotationY, 0f) * mouseSensitivity;
		componentRotationX = rotationX * mouseSensitivity;

		// Apply UD-rotation of player
		RotateX();

		if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
		{
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
		}
		//Lerp(a, b, t) = a + (b - a) * t
	}

	void FixedUpdate()
	{
		// Apply movement and LR-rotation of player
		Move();
		RotateY();
	}

	void Move()
	{
		if (velocityMove != Vector3.zero)
		{
			rb.MovePosition(rb.position + velocityMove * Time.fixedDeltaTime);
		}
	}

	// Left and right rotation of player (around of Y-axis)
	void RotateY()
	{
		rb.MoveRotation(rb.rotation * Quaternion.Euler(vectorRotationY));
	}

	// Up and down rotation of camera (around of X-axis)
	void RotateX()
	{
		if (fpsCam != null)
		{
			currentComponentRotationX -= componentRotationX;
			currentComponentRotationX = Mathf.Clamp(currentComponentRotationX, minLimitRotationX, maxLimitRotationX);
			fpsCam.transform.localEulerAngles = new Vector3(currentComponentRotationX, 0f, 0f);
		}
	}

}
