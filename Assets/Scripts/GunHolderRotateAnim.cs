using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunHolderRotateAnim : MonoBehaviour
{
	float rotateX = 0f;
	float rotateY = 0f;
	private Animator animator;

	void Start ()
	{
		animator = GetComponent<Animator>();
	}
	
	void Update ()
	{
		#region Rotate Animation Blend Tree
		float MouseY = Input.GetAxisRaw("Mouse Y");
		float MouseX = Input.GetAxisRaw("Mouse X");
		MouseX = Mathf.Clamp(MouseX, -1, 1);
		MouseY = Mathf.Clamp(MouseY, -1, 1);

		rotateX = Mathf.Lerp(rotateX, MouseY, Time.deltaTime * 3f);
		rotateY = Mathf.Lerp(rotateY, MouseX, Time.deltaTime * 3f);
		
		animator.SetFloat("RotateY", rotateY);
		animator.SetFloat("RotateX", rotateX);
		#endregion Rotate Animation Blend Tree
	}
}
