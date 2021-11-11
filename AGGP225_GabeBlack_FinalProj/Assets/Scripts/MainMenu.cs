using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class MainMenu : MonoBehaviour
{

	public TMP_InputField inputField;
	public GameObject pleaseInputText;
	public GameObject connectedText;

	public static MainMenu instance;

	public bool hasBeenConnected = false;

	private void Awake()
	{
		instance = this;
	}

	void Start()
	{
		if(hasBeenConnected)
		{
			connectedText.SetActive(true);
		}
	}

	public void OnCreateRoomClick()
	{
		if (!string.IsNullOrEmpty(inputField.text))
		{
			PhotonManager.instance.username = inputField.text;

			if (PhotonManager.instance != null)
			{
				PhotonManager.instance.CreateRoom();
			}
		}
		else
		{
			pleaseInputText.SetActive(true);
		}
	}

	public void OnJoinRandomRoomClick()
	{
		if (!string.IsNullOrEmpty(inputField.text))
		{
			PhotonManager.instance.username = inputField.text;

			if (PhotonManager.instance != null)
			{
				PhotonManager.instance.JoinRandomRoom();
			}
		}
		else
		{
			pleaseInputText.SetActive(true);
		}
	}

	public void OnJoinChatroomClick()
	{
		if (!string.IsNullOrEmpty(inputField.text))
		{
			PhotonManager.instance.username = inputField.text;

			if (PhotonManager.instance != null)
			{
				PhotonManager.instance.JoinChatroom();
			}
		}
		else
		{
			pleaseInputText.SetActive(true);
		}
	}

	public void Quit()
	{
		Application.Quit();
	}
}