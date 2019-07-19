using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacingBillboards : MonoBehaviour 
{
	public Camera fpsCam;

	void Update()
	{
		transform.LookAt
			(transform.position + fpsCam.transform.rotation * Vector3.forward, fpsCam.transform.rotation * Vector3.up);
	}
}
