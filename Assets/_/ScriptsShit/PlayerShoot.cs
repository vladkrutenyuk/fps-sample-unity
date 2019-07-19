﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
	private const string PLAYER_TAG = "Player";

	[SerializeField]
	private Camera cam;

	[SerializeField]
	private LayerMask mask;

	private PlayerWeapon currentWeapon;
	private WeaponManager weaponManager;

	int indexHit; // 0 - objects, 1 - people

	private void Start()
	{
		if(cam == null)
		{
			Debug.LogError("PlayerShoot: No camera referenced");
			this.enabled = false;
		}

		weaponManager = GetComponent<WeaponManager>();
	}

	void Update()
	{	
		currentWeapon = weaponManager.GetCurrentWeapon();

		if (PauseMenu.IsOn)
			return;

		if(currentWeapon.fireRate <= 0f)
		{
			if (Input.GetButtonDown("Fire1"))
			{
				Shoot();
			}
		} else
		{
			if (Input.GetButtonDown("Fire1"))
			{
				InvokeRepeating("Shoot", 0f, 60f/currentWeapon.fireRate);
			} else if (Input.GetButtonUp("Fire1"))
			{
				CancelInvoke("Shoot");
			}
		}
		
	}

	// Is called on the server when a player shoots
	[Command]
	void CmdOnShoot()
	{
		RpcDoShootEffects();
	}
	// Is called on all clients when we need 
	// to do a shoot effect
	[ClientRpc]
	void RpcDoShootEffects()
	{
		weaponManager.GetCurrentGraphics().muzzleFlash.Play();
	}


	// Is called on the server when we hit something
	// Takes in the hit point and the normal of the surface
	[Command]
	void CmdOnHit (Vector3 _pos, Vector3 _normal)
	{
		RpcDoHitEffects(_pos, _normal);
	}
	// Is called on all clients
	// Here we can spawn in cool effects
	[ClientRpc]
	void RpcDoHitEffects(Vector3 _pos, Vector3 _normal)
	{
		if (indexHit == 0)
		{
			GameObject _hitEffect = (GameObject)Instantiate
			(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
			Destroy(_hitEffect, 2f);
		} else if (indexHit == 1)
		{
			GameObject _hitEffect = (GameObject)Instantiate
			(weaponManager.GetCurrentGraphics().hitEffectBloodPrefab, _pos, Quaternion.LookRotation(_normal));
			Destroy(_hitEffect, 2f);
		}
	}


	[Client]
	void Shoot()
	{
		if (!isLocalPlayer)
			return;

		// We are shooting, call the OnShoot Method on the server
		CmdOnShoot();
		
		RaycastHit _hit;
		if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
		{
			Debug.Log("Shot in " + _hit.collider.name);
			indexHit = 0;
			if(_hit.collider.tag == PLAYER_TAG)
			{
				indexHit = 1;
				CmdPlayerShot(_hit.collider.name, currentWeapon.damage);
			}
			CmdOnHit(_hit.point, _hit.normal);
		}
	}

	[Command]
	void CmdPlayerShot (string _playerID, int _damage)
	{
		Debug.Log(_playerID + " has been shot.");

		Player _player = GameManager.GetPlayer(_playerID);
		_player.RpcTakeDamage(_damage);
	}

}