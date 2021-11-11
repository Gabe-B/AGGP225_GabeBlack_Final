using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class Lobby : MonoBehaviour
{
    public TMP_InputField input;
    public TMP_Text field;
    public TMP_Text names;
    public GameObject startButton;

    string gameLevel = "TestLevel";

    public static Lobby instance { get; set; }

	void Awake()
	{
        instance = this;

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
        gameObject.GetPhotonView().RPC("UpdateNames", RpcTarget.AllBuffered, PhotonManager.instance.username.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
		{
            Submit();
		}
    }

    public void Submit()
    {
        if (!string.IsNullOrEmpty(input.text))
        {
            PhotonManager.instance.gameObject.GetPhotonView().RPC("UsernameRPC", RpcTarget.AllBuffered, PhotonManager.instance.username.ToString(), input.text);
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
            PhotonNetwork.LoadLevel(gameLevel);
        }
    }

    [PunRPC]
    public void UpdateNames(string _username)
	{
        names.text += _username + "\n";
    }
}
