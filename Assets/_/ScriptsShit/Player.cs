using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
	[SyncVar]
	private bool _isDead = false;
	public bool isDead
	{
		get { return _isDead;  }
		protected set { _isDead = value; }
	}
	
	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currentHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnabled;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathEffect;
	[SerializeField]
	private GameObject spawnEffect;

	private bool firstSetup = true;

	public void SetupPlayer()
	{
		if(isLocalPlayer)
		{
			// Switch cameras
			GameManager.instance.SetSceneCameraActive(false);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
		}

		CmdBroadcastNewPlayerSetup();
	}

	[Command]
	private void CmdBroadcastNewPlayerSetup ()
	{
		RpcSetupPlayerOnAllClients();
	}

	[ClientRpc]
	private void RpcSetupPlayerOnAllClients()
	{
		if(firstSetup)
		{
			wasEnabled = new bool[disableOnDeath.Length];
			for (int i = 0; i < wasEnabled.Length; i++)
			{
				wasEnabled[i] = disableOnDeath[i].enabled;
			}

			firstSetup = false;
		}
		
		SetDefaults();
	}

	// Получение урона игроком
	[ClientRpc]
	public void RpcTakeDamage(int _amount)
	{
		if (isDead)
			return;

		currentHealth -= _amount;
		Debug.Log(transform.name + " now has " + currentHealth + " health.");

		if (currentHealth <= 0)
		{
			Die();
		}
	}

	// Смерть игрока
	private void Die()
	{
		// Dissable components
		Debug.Log(transform.name + " is Dead!");

		isDead = true;
		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = false;
		}

		// Disable GameObjs
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
		{
			disableGameObjectsOnDeath[i].SetActive(false);
		}

		//Dissable the CharacterController (Collider)
		CharacterController _charCont = GetComponent<CharacterController>();
		if (_charCont != null)
			_charCont.enabled = false;
		//Spawn a death effect
		GameObject _gfxIns = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity);
		Destroy(_gfxIns, 2f);

		// Switch cameras
		if (isLocalPlayer)
		{
			GameManager.instance.SetSceneCameraActive(true);
			GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
		}

		StartCoroutine(Respawn());
	}

	// Возрождение игрока
	private IEnumerator Respawn()
	{
		yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		yield return new WaitForSeconds(0.1f);

		SetupPlayer();

		Debug.Log(transform.name + " respawned.");

	}

	
	void SetDefaults()
	{
		isDead = false;

		// Enable components and set gameobjs active
		currentHealth = maxHealth;
		for (int i = 0; i < disableOnDeath.Length; i++)
		{
			disableOnDeath[i].enabled = wasEnabled[i];
		}
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
		{
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		// Enable CharCont
		CharacterController _charCont = GetComponent<CharacterController>();
		if (_charCont != null)
			_charCont.enabled = true;

		//Spawn a spawn(lol kek) effect
		GameObject _gfxInsSpwn = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
		Destroy(_gfxInsSpwn, 2f);
	}
}
