using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRocks : MonoBehaviour
{
	public List<Rigidbody> rigidbodies;
	public Rigidbody BigRocks;
	public float bigRockDelay = 2f; // ©µ¿ð¬í¼Æ

	private bool triggered = false;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space) && !triggered)
		{
			triggered = true;
			ActivateSmallRocks();
			StartCoroutine(ActivateBigRockAfterDelay(bigRockDelay));
		}
	}

	void ActivateSmallRocks()
	{
		foreach (var rb in rigidbodies)
		{
			rb.isKinematic = false;
		}
	}

	IEnumerator ActivateBigRockAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		BigRocks.isKinematic = false;
	}
}

