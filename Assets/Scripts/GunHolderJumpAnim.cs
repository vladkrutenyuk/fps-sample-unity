using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolderJumpAnim : MonoBehaviour {

	private Animator animator;
	public CharacterController characterController;
	public PlayerCharController playerCharController;

	bool inJumped = false;
	float jumpFloat = 0;
	bool lastIsGrounded;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	void Update ()
	{
		if(animator != null)
		{
			animator.SetBool("isGrounded", playerCharController.IsGrounded());
			if (lastIsGrounded & !playerCharController.IsGrounded())
			{
				if (playerCharController.isJumping)
				{
					animator.SetBool("isJumping", playerCharController.isJumping);
				} else
				{
					animator.SetBool("isFalling", true);
				}
			}

			if (!lastIsGrounded & playerCharController.IsGrounded())
			{
				animator.SetBool("isJumping", false);
				animator.SetBool("isFalling", false);
			}

			lastIsGrounded = playerCharController.IsGrounded();
		}
	}
}
