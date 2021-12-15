using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerInstace;
    public TMP_InputField input;
    public int score;
    public List<GameObject> spawnPoints = new List<GameObject>();
    public List<Player> finished = new List<Player>();
    public FinishLine fl;

    public bool roundOver = false;

    public int numSpectators = 0;

    public int temp;

    public static GameManager instance { get; set; }

    void Awake()
    {
        instance = this;
        roundOver = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Main Menu");

            return;
        }
        else
        {
            if (instance == this)
            {
                if (playerPrefab)
                {
                    GameObject spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count - 1)];
                    GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.transform.position, Quaternion.identity);
                    player.GetComponent<Player>().playerCam.gameObject.SetActive(true);
                    player.GetComponent<Player>().spawnPoints = spawnPoints;
                }
                else
                {
                    Debug.Log("[FPSGameManager] There is no player prefab attached");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(fl.entered.Count == PhotonNetwork.CurrentRoom.PlayerCount)
		{
            if(PhotonNetwork.IsMasterClient && !roundOver)
			{
                roundOver = true;

                if(PhotonManager.instance.rounds == 3)
				{
                    bool first = true;

                    foreach (int entry in PhotonManager.instance.players)
                    {
                        if (first)
                        {
                            temp = entry;
                            first = false;
                        }
                        else
                        {
                            if (entry < temp)
                            {
                                temp = entry;
                            }
                        }
                    }
                    PhotonManager.instance.winnerName = "The winner was the one with " + temp + " points!";
                    PhotonManager.instance.matchDone = true;
                }

                PhotonNetwork.LoadLevel("Lobby");
			}
		}
    }
}
