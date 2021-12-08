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

    public float time = 20f;

    float r, g, b;
    bool isTimerRunning = true;

    string gameLevel1 = "Level1";
    string gameLevel2 = "Level2";
    string gameLevel3 = "Level3";

    public static Lobby instance { get; set; }

	void Awake()
	{
        instance = this;

        gameObject.GetPhotonView().RPC("UpdateNames", RpcTarget.AllBuffered, PhotonManager.instance.username.ToString());

        if (PhotonNetwork.IsMasterClient)
        {
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
        if(Input.GetKeyDown(KeyCode.Return))
		{
            Submit();
		}

        /*if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonManager.instance.roomOptions.MaxPlayers)
        {
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

                    LoadGame();
                }
            }
        }*/
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
        if (PhotonNetwork.IsMasterClient)
        {
            //winnerText.gameObject.SetActive(true);

            int randNum = Random.Range(0, 2);
            string level = "";

            switch(randNum)
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

                    PhotonNetwork.LoadLevel(level);
                }
            }
        }
    }

    [PunRPC]
    public void UpdateNames(string _username)
	{
        names.text += _username + "\n";
    }
}
