using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
	public static PhotonManager instance { get; private set; }

	public string username;
	public int MAX_PLAYERS = 4;

	RoomOptions roomOptions = new RoomOptions();

	string gameVersion = "1";
	string gameLevel = "Lobby";

	void Awake()
	{
		if (instance)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			gameObject.AddComponent<PhotonView>();
			gameObject.GetPhotonView().ViewID = 999;
			DontDestroyOnLoad(this);
		}

		SceneManager.sceneLoaded += OnSceneLoaded;

		PhotonNetwork.AutomaticallySyncScene = true;
		roomOptions.MaxPlayers = (byte)MAX_PLAYERS;
	}

	void Start()
	{
		Connect();
	}

	/// <summary>
	/// Connects user to master server
	/// </summary>
	public void Connect()
	{
		if (!PhotonNetwork.IsConnected)
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = gameVersion;
			MainMenu.instance.connectedText.SetActive(true);
			MainMenu.instance.hasBeenConnected = true;
		}
		else
		{
			MainMenu.instance.connectedText.SetActive(true);
			MainMenu.instance.hasBeenConnected = true;
		}
	}

	public void CreateRoom()
	{
		Debug.Log("[PhotonManager][CreateRoom][Trying to create room]");

		PhotonNetwork.NickName = MainMenu.instance.inputField.text;

		PhotonNetwork.CreateRoom("Test Room", roomOptions);
	}

	public void JoinRandomRoom()
	{
		Debug.Log("[PhotonManager][JoinRandomRoom][Trying to join random room]");

		PhotonNetwork.NickName = MainMenu.instance.inputField.text;

		PhotonNetwork.JoinRandomRoom();
	}

	public void JoinChatroom()
	{
		Debug.Log("[PhotonManager][JoinChatroom][Trying to join random room]");

		PhotonNetwork.JoinRandomRoom();
	}

	#region Photon Callbacks
	public override void OnConnectedToMaster()
	{
		Debug.Log("[PhotonManager][Connected to Master]");
	}

	public override void OnCreatedRoom()
	{
		Debug.Log("[PhotonManager][OnCreatedRoom]");
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("[PhotonManager][OnJoinedRoom]");

		if (PhotonNetwork.IsMasterClient)
		{
			PhotonNetwork.LoadLevel(gameLevel);
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.Log("[PhotonManager][OnDisconnected] " + cause);
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.Log("[PhotonManager][OnCreateRoomFailed] " + message);
		JoinRandomRoom();
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		Debug.Log("[PhotonManager][OnJoinRandomFailed] " + message);
		CreateRoom();
	}

	public override void OnLeftRoom()
	{
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
		SceneManager.LoadScene("MainMenu");
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (instance != this)
		{
			//PhotonNetwork.Destroy(gameObject);
		}
	}
	#endregion

	#region RPC's

		[PunRPC]
		void UsernameRPC(string _username, string _chat)
		{
			Lobby.instance.field.text += _username + ":	" + _chat + "\n";
		}

	#endregion
}