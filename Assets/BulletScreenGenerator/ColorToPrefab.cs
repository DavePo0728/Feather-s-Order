using UnityEngine;

[System.Serializable]
public class ColorToPrefab
{
	[SerializeField]
	public code code;
	public GameObject prefab;
}

public enum code { N=0,A=1 }
