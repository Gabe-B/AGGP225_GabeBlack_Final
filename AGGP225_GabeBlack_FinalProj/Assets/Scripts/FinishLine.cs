using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
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
            p.playerCam.gameObject.SetActive(false);
            p.isSpectator = true;
		}
	}
}
