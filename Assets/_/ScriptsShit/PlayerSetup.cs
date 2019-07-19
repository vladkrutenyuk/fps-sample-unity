using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerSetup : NetworkBehaviour
{
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "RemotePlayer";

	[SerializeField]
	public string dontDrawLayerName = "DontDraw";
	[SerializeField]
	GameObject playerLocalGraphics;
	[SerializeField]
	GameObject playerRemoteGraphics;

	[SerializeField]
	GameObject PlayerUIPrefab;
	[HideInInspector]
	public GameObject playerUIInstance; 

	private void Start()
	{
		// WE ARE REMOTE PLAYER
		if(!isLocalPlayer)
		{
			DisableComponents();
			AssignRemoteLayer();

			// Set correct (local/remote) player graphics
			SetLayerGraphicsRecurs(playerLocalGraphics, LayerMask.NameToLayer(dontDrawLayerName));
			SetLayerGraphicsRecurs(playerRemoteGraphics, LayerMask.NameToLayer(remoteLayerName));
		
		} else
		// WE ARE LOCAL PLAYER
		{

			// Set correct (local/remote) player graphics
			SetLayerGraphicsRecurs(playerRemoteGraphics, LayerMask.NameToLayer(dontDrawLayerName));

			// Create UI
			playerUIInstance = Instantiate(PlayerUIPrefab);
			playerUIInstance.name = PlayerUIPrefab.name;

			GetComponent<Player>().SetupPlayer();
		}
	}

	void SetLayerGraphicsRecurs (GameObject obj1, int Layer1)
	{
		obj1.layer = Layer1;
		foreach (Transform child in obj1.transform)
		{
			SetLayerGraphicsRecurs(child.gameObject, Layer1);
		}

	}

	public override void OnStartClient()
	{
		base.OnStartClient();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player>();

		GameManager.RegisterPlayer(_netID, _player);
	}

	// Set LayerMask for Remote players
	void AssignRemoteLayer()
	{
		gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
	}

	// Disable components for Remote players
	void DisableComponents()
	{
		for (int i = 0; i < componentsToDisable.Length; i++)
		{
			componentsToDisable[i].enabled = false;
		}
	}

	// When we are destroyed
	private void OnDisable()
	{
		Destroy(playerUIInstance);

		if (isLocalPlayer)
			GameManager.instance.SetSceneCameraActive(true);

		GameManager.UnRegisterPlayer(transform.name);
	}

}
