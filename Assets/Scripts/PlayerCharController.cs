using UnityEngine;

public class PlayerCharController : MonoBehaviour
{
	public float speedMove = 5f;// м/с
	public float speedJump = 3f;// м/с
	float g = 9.81f;// м/с^2
	public float mouseSensitivity = 3f;

	float currentRotationX;
	public float minRotX = -70f;
	public float maxRotX = 80f;

	public Vector3 velocity = Vector3.zero;

	public bool isJumping = false;
	bool isSloping = false;

	CharacterController controller;
	Camera fpsCam;

	void Start ()
	{
		controller = GetComponent<CharacterController>();
		fpsCam = GetComponentInChildren<Camera>();
	}
	
	void Update ()
	{
		if (controller != null)
		{
			#region Основная физика движения
			if (IsGrounded())
			{
				velocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
				velocity = transform.TransformDirection(velocity);
				ConsiderSlopeForVelocityY();

				if (Mathf.Abs(velocity.magnitude) > 1)
				{
					velocity = velocity.normalized * speedMove;
				} else
				{
					velocity *= speedMove;
				}

				isJumping = false;
				if (Input.GetKey(KeyCode.Space))
				{
					velocity.y = speedJump;
					isJumping = true;
				}

				Vector3 center = transform.position + (Vector3.up * controller.radius);
				Debug.DrawRay(center, velocity, Color.blue);

			}
			else
			{
				velocity.y -= g * Time.deltaTime;

				//velocity.x += Input.GetAxisRaw("Horizontal");
				//velocity.z += Input.GetAxisRaw("Vertical");
			}
			//Debug.Log("x= " + controller.velocity.x + ", y = " + controller.velocity.y + ", z= " + controller.velocity.z);
			controller.Move(velocity * Time.deltaTime);
			#endregion

			#region Вращение игрока вокруг оси Y (влево-вправо)
			Vector3 rotationY = new Vector3(0, Input.GetAxis("Mouse X"), 0) * mouseSensitivity;
			controller.transform.Rotate(rotationY);
			#endregion

			#region Вращение камеры вокруг оси X (вверх-вниз)
			if (fpsCam != null)
			{
				float rotationX = Input.GetAxis("Mouse Y") * mouseSensitivity;
				currentRotationX -= rotationX;
				currentRotationX = Mathf.Clamp(currentRotationX, minRotX, maxRotX);
				fpsCam.transform.localEulerAngles = new Vector3(currentRotationX, 0, 0);
			}
			#endregion
		}
	}

	public bool IsGrounded()
	{
		RaycastHit sphereHit;
		Vector3 center = transform.position + (Vector3.up * controller.radius);
		return Physics.SphereCast(center, controller.radius, Vector3.down, out sphereHit, 0.05f);
	}

	public float CosAngleBetweenVectors3d (Vector3 v1, Vector3 v2)
	{
		float cos_a = (v1.x * v2.x + v1.y * v2.y + v1.z * v2.z) /
			(Mathf.Sqrt(v1.x * v1.x + v1.y * v1.y + v1.z * v1.z) * Mathf.Sqrt(v2.x * v2.x + v2.y * v2.y + v2.z * v2.z));
		return cos_a;
	}

	public void ConsiderSlopeForVelocityY()
	{
		RaycastHit hit;
		Vector3 center = transform.position + (Vector3.up * controller.radius);
		if (Physics.SphereCast(center, controller.radius, Vector3.down, out hit, 0.1f))
		{
			//float cosA = CosAngleBetweenVectors3d(hit.normal, velocity); // Косинус угла между нормалью и направлением скорости	
			float A = Mathf.Acos(CosAngleBetweenVectors3d(hit.normal, velocity)) * 180f / Mathf.PI; // Сам угл в градусах
			float B;
			//Debug.Log("угол = " + A);
			Debug.DrawRay(hit.point, hit.normal, Color.red);
			
			if(A > 91f & A <= 135f) // Подъем по склону
			{
				B = A - 90f;
				velocity.y = Mathf.Tan(B * Mathf.PI / 180f) * velocity.magnitude;

			}else if (A > 45f & A < 89f) // Спуск по склону
			{	
				B = 90f - A;
				velocity.y = -Mathf.Tan(B * Mathf.PI / 180f) * velocity.magnitude * 1.1f;
			}
		}
	}
}
