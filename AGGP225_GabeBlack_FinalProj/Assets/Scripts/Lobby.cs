using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
	public TMP_InputField input;
	public TMP_Text field;
	public TMP_Text names;
	public GameObject startButton;
	public GameObject panel;
	public TMP_Text timer;
	public TMP_Text winnerText;

	public GameObject startGameButton;
	public GameObject resetButton;

	public Dictionary<string, int> players = new Dictionary<string, int>();

	public float time = 20f;

	public int ROUNDS;

	static int rounds = 0;

	int TIME;
	string level = "";

	float r, g, b;
	bool isTimerRunning = true;
	bool startGame = false;
	bool randChosen = false;
	bool reset = false;

	string gameLevel1 = "Level1";
	string gameLevel2 = "Level2";
	string gameLevel3 = "Level3";

	public static Lobby instance { get; set; }

	void Awake()
	{
		instance = this;

		gameObject.GetPhotonView().RPC("UpdateNames", RpcTarget.AllBuffered, PhotonManager.instance.username.ToString());

		instance.players.Add(PhotonManager.instance.username.ToString(), 0);

		if (PhotonNetwork.IsMasterClient)
		{
			TIME = (int)time;

			ROUNDS = rounds;

			startButton.SetActive(true);
		}
		else
		{
			startButton.SetActive(false);
		}
	}

	// Start is called before the first frame update
	void Start()
	{
		//if(gameObject.GetPhotonView().IsMine)
		//{
		r = PhotonManager.instance.RED;
		g = PhotonManager.instance.GREEN;
		b = PhotonManager.instance.BLUE;

		panel.GetComponent<Image>().color = new Color32((byte)r, (byte)g, (byte)b, (byte)255);
		//}
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			Submit();
		}

		ROUNDS = rounds;

		if(rounds < 3)
		{
			if (startGame)
			{
				if (PhotonNetwork.IsMasterClient)
				{
					//winnerText.gameObject.SetActive(false);

					if (!randChosen)
					{
						int randNum = Random.Range(0, 3);

						switch (randNum)
						{
							case 0:
								level = gameLevel1;
								break;

							case 1:
								level = gameLevel2;
								break;

							case 2:
								level = gameLevel3;
								break;
						}

						randChosen = true;
					}

					if (isTimerRunning)
					{
						if (time > 0)
						{
							time -= Time.deltaTime;

							PhotonManager.instance.gameObject.GetPhotonView().RPC("UpdateLobbyTimer", RpcTarget.AllBuffered, time);
						}
						else
						{
							time = 0;
							isTimerRunning = false;
							randChosen = false;

							rounds++;

							PhotonNetwork.LoadLevel(level);
						}
					}
				}
			}
		}
		else
		{
			if(!reset)
			{
				KeyValuePair<string, int> temp;
				bool first = true;

				foreach (KeyValuePair<string, int> entry in players)
				{
					if (first)
					{
						temp = entry;
						first = false;
					}
					else if (entry.Value < temp.Value)
					{
						temp = entry;
					}
				}

				//winnerText.gameObject.SetActive(true);

				winnerText.text = "The winner is: " + temp.Key + "!!!";

				reset = true;
			}
			else
			{
				if(PhotonNetwork.IsMasterClient)
				{
					startGameButton.SetActive(false);
					resetButton.SetActive(true);
				}
			}
		}
	}

	public void Submit()
	{
		if (!string.IsNullOrEmpty(input.text))
		{
			PhotonManager.instance.gameObject.GetPhotonView().RPC("LobbyChatRPC", RpcTarget.AllBuffered, PhotonManager.instance.username.ToString(), input.text);
			input.text = "";
		}
	}

	public void Leave()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void LoadGame()
	{
		startGame = true;
	}

	public void ResetLobby()
	{
		rounds = 0;
		isTimerRunning = true;
		startGame = false;
		randChosen = false;
		reset = false;

		foreach (KeyValuePair<string, int> entry in players)
		{
			players[entry.Key.ToString()] = 0;
		}

		winnerText.gameObject.SetActive(false);
	}

	[PunRPC]
	public void UpdateNames(string _username)
	{
		names.text += _username + "\n";
	}
}
