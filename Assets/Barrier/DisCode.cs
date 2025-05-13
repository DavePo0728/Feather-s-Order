using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisCode : MonoBehaviour
{

	public GameObject player;
	public Material dismat;
	private void Update()
	{
		dismat.SetVector("_PlayerPosition", player.transform.position);
	}

}
