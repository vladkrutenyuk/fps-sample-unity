using UnityEngine;

public class PlayerUI : MonoBehaviour
{
	private PlayerCharController controller;

	[SerializeField]
	GameObject pauseMenu;

	public void SetController (PlayerCharController _controller)
	{
		controller = _controller;
	}

	private void Start()
	{
		PauseMenu.IsOn = false;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePauseMenu();
		}
	}

	void TogglePauseMenu ()
	{
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		PauseMenu.IsOn = pauseMenu.activeSelf;
	}
}
