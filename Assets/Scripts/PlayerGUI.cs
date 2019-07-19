using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
	public Text fpsText;

	public RaycastShoot playerShoot;
	public Text currentAmmoText;

	public PlayerCharController playerCharController;
	public Text currentSpeedText;

	public Animator crosshairAnimator;
	float blendValue = 0f;

	void Start ()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	void Update ()
	{
		float fps = 1f / Time.deltaTime;
		fpsText.text = fps.ToString();

		// ON/OFF CURSOR
		if (Input.GetKeyDown(KeyCode.C))
		{
			if (Cursor.visible)
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			} else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
		}

		currentAmmoText.text = playerShoot.currentAmmo.ToString();

		currentSpeedText.text = playerCharController.velocity.magnitude.ToString();

		if(crosshairAnimator != null)
		{
			blendValue = Mathf.Lerp(blendValue, playerShoot.i * 1f / (playerShoot.mltUpDvnt * 1f), Time.deltaTime * 20f);
			crosshairAnimator.SetFloat("Blend", blendValue);
		}

	}
}
