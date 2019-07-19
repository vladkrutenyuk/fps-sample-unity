using UnityEngine;

public class GunHolderAnim : MonoBehaviour {

	private Animator animator;
	public CharacterController characterController;
	public PlayerCharController playerCharController;

	float moveX = 0f;
	float moveZ = 0f;

	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	void Update ()
	{
		
		if (animator != null)
		{
			//Check on isWalking for transitions from Default State to Move Anim Blend Tree
				animator.SetBool("isWalking", IsWalking());	

			#region Move Animation Blend Tree
			moveX = Mathf.Lerp(moveX, Input.GetAxisRaw("Horizontal"), Time.deltaTime * 3);
			moveZ = Mathf.Lerp(moveZ, Input.GetAxisRaw("Vertical"), Time.deltaTime * 3);
			animator.SetFloat("MoveZ", moveZ);
			animator.SetFloat("MoveX", moveX);
			#endregion Move Animation Blend Tree

		}

	}

	public bool IsWalking()
	{
		return characterController.velocity.magnitude > 0.2f & playerCharController.IsGrounded();
	}
}
