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
    public List<GameObject> spawnPoints = new List<GameObject>();
    List<Player> players = new List<Player>();
    public TMP_InputField input;
    public int score;

    int numSpectators = 0;

	public static GameManager instance { get; set; }

    void Awake()
    {
        instance = this;
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
        foreach(Player p in FindObjectsOfType<Player>())
		{
            if(p.isSpectator)
			{
                numSpectators++;
			}
		}

        if(numSpectators == PhotonNetwork.CurrentRoom.PlayerCount)
		{
            if(PhotonNetwork.IsMasterClient)
			{
                PhotonNetwork.LoadLevel("Lobby");
			}
		}
    }
}
