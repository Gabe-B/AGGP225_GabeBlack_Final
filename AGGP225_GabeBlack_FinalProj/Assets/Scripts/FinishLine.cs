using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class FinishLine : MonoBehaviour
{
    public int placement = 0;

    public List<Player> entered = new List<Player>();

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
                p.playerCam.gameObject.GetComponent<Camera>().enabled = false;
                p.isSpectator = true;

                PhotonManager.instance.players[placement] += placement;

                //p._Placement.Add(p.gameObject.GetPhotonView().Owner.NickName, placement);

                p.rb.velocity = Vector3.zero;
            }

            Debug.Log(p.gameObject.GetPhotonView().Owner.NickName);

            entered.Add(p);

            placement++;
        }
	}
}
