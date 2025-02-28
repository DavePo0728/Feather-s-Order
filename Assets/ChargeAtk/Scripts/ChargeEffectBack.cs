using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEffectBack : MonoBehaviour
{
	public PlayerChargeShooting root;
	public void back()
	{
		GetComponent<Animator>().SetBool("back", false);
	}
	//public void canshoot()
	//{
	//	root.ChargeComplete();
	//}
	//public void Onshoot()
	//{
	//	root.shoot();
	//}
}
