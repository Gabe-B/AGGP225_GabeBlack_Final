using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class FinishLine : MonoBehaviour
{
    public int placement = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnTriggerEnter(Collider other)
	{
        Player p = other.GetComponent<Player>();

		if(p)
		{
            if(p.gameObject.GetPhotonView().IsMine)
			{
                p.playerCam.gameObject.SetActive(false);
                p.isSpectator = true;

                p._Placement["place"] = placement;
                PhotonNetwork.LocalPlayer.CustomProperties = p._Placement;

                p.place.text = PhotonNetwork.LocalPlayer.CustomProperties["place"].ToString();
            }

            placement++;
        }
	}
}
