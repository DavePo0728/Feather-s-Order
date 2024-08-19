using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public class BulletScreenGenerator : MonoBehaviour
{
	[SerializeField]
	EnemyShooting enemyShooting;

	string[,] bullet2DArray = new string[13,22];

	public ColorToPrefab[] codeMappings;
	string path = "Assets/BulletScreenGenerator/BulletScreenMap/BulletScreenMap1.txt";

	public void GenerateBulletMap()
	{
		ReadTextFile(path);
		enemyShooting.ClearChosenList();
		//testEnemy.ClearChosenList();
        for (int x = 0; x < 13; x++)
        {
            for (int y = 0; y < 22; y++)
            {
                GenerateTile(x, y);
            }
        }
    }
	public void ReadTextFile(string filePath)
	{
		string[] lines = File.ReadAllLines(path);
		for (int i = 0; i < lines.Length; ++i)
		{
			string[] items = lines[i].Split('	');
			//Debug.Log(items.Length);
			for (int j = 0; j < items.Length; ++j)
            {
				bullet2DArray[i, j] = items[j];
				//Debug.Log(items[j]);
			}
		}
	}

	void GenerateTile(int x, int y)
	{
		string bulletCode = bullet2DArray[x,y];

		if (bulletCode == "0")
		{
			return;
		}
		foreach (ColorToPrefab codeMapping in codeMappings)
		{
			if (codeMapping.code.Equals("A"))
			{
				enemyShooting.AddChosen2DArrayToList(x, y);
				//testEnemy.Add2DArrayToList(x, y);
				//Debug.Log("x :"+x+"y :"+y);
			}
		}
	}
}
