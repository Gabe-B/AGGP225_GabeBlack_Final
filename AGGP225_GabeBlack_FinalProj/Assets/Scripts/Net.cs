using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Net : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Player p = other.GetComponent<Player>();

		if(p)
		{
			p.gameObject.transform.position = new Vector3(0, 2f, 0);
		}
	}
}
