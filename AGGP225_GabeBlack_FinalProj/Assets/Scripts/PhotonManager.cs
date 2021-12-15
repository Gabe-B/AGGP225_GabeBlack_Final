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
	public RoomOptions roomOptions = new RoomOptions();


	public TMP_InputField red;
	public TMP_InputField green;
	public TMP_InputField blue;
	public GameObject panel;

	public string username;
	public int MAX_PLAYERS = 4;
	public float RED, GREEN, BLUE;

	public string winnerName;
	public bool matchDone = false;
	public int rounds;

	public List<int> players = new List<int>();
	public bool allJoined = false;

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

		//SceneManager.sceneLoaded += OnSceneLoaded;

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

		PhotonNetwork.NickName = MainMenu.instance.inputField.text;

		PhotonNetwork.JoinRandomRoom();
	}

	public void ChangeColor()
	{
		float redVal, greenVal, blueVal;

		if (!string.IsNullOrEmpty(red.ToString()))
		{
			float.TryParse(red.text.ToString(), out float resultRed);
			redVal = resultRed;
			RED = redVal;
			red.text = "";
		}
		else
		{
			redVal = 0;
			RED = 0;
		}

		if (!string.IsNullOrEmpty(green.ToString()))
		{
			float.TryParse(green.text.ToString(), out float resultGreen);
			greenVal = resultGreen;
			GREEN = greenVal;
			green.text = "";
		}
		else
		{
			greenVal = 0;
			GREEN = 0;
		}

		if (!string.IsNullOrEmpty(blue.ToString()))
		{
			float.TryParse(blue.text.ToString(), out float resultBlue);
			blueVal = resultBlue;
			BLUE = blueVal;
			blue.text = "";
		}
		else
		{
			blueVal = 0;
			BLUE = 0;
		}

		panel.GetComponent<Image>().color = new Color32((byte)redVal, (byte)greenVal, (byte)blueVal, (byte)255);
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
	void LobbyChatRPC(string _username, string _chat)
	{
		Lobby.instance.field.text += _username + ":	" + _chat + "\n";
	}

	[PunRPC]
	void UpdateLobbyTimer(float t)
	{
		foreach (Lobby l in FindObjectsOfType<Lobby>())
		{
			l.timer.text = Mathf.Round(t).ToString();
		}
	}
	#endregion
}