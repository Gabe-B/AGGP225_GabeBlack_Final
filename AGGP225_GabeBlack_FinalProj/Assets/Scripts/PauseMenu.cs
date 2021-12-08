using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
	public GameObject pauseMenu;
	public Player p;

    public void Resume()
	{
		p.canMove = true;
		pauseMenu.SetActive(false);
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
	}

	public void LeaveGame()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
