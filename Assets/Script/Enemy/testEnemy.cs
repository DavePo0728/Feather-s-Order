using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testEnemy : MonoBehaviour
{
    [SerializeField]
    BulletScreenGenerator bulletScreenGenerator;
    [SerializeField]
    GameObject enemy;
    GameObject enemyParent;
    Vector3 origin = new Vector3(-8.4f, 8.1f, 100);

    public int cloumns = 13; // 列數
    public int rows = 22; // 行數
    public float spacing = 7.5f; // 間隔

    //Vector3 List
    [SerializeField]
    List<Vector3> chosenEnemySpawnPosList;
    Vector3[,] enemyPosition2DArray = new Vector3[13, 22];

    //GameObject List
    [SerializeField][HideInInspector]
    List<GameObject> enemyList;
    [SerializeField]
    List<GameObject> chosenList;
    //GameObject[,] enemy2DArray = new GameObject[13, 22];

    float shootDuration = 1.0f;
    float shootingCoolDown = 0.5f;
    bool canShoot = false;
    //[SerializeField]
    //Matrix enemyMatrix;

    private void Awake()
    {
        enemyParent = this.gameObject;
        GenerateEnemySpawnPos();
        
        
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //SpawnEnemy();
        bulletScreenGenerator.GenerateBulletMap();
        SpawnChosenEnemy();
        canShoot = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canShoot)
        {
            Shoot();
            StartCoroutine(CoolDown());
        }
    }
    void Shoot()
    {
        if (chosenList != null)
            foreach (GameObject enemy in chosenList)
            {
                if (enemy != null)
                {
                    EnemyShooting enemysShooting = enemy.transform.GetChild(0).GetComponent<EnemyShooting>();
                    //enemysShooting.Shoot();
                }
            }
    }
    void GenerateEnemySpawnPos()
    {
        for (int i = 0; i < cloumns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Vector3 spawnPosition = new Vector3(origin.x + j * spacing, origin.y - i * spacing, 100);
                enemyPosition2DArray[i, j] = spawnPosition;
                Debug.Log("1. "+enemyPosition2DArray[i, j]);
            }
        }
    }
    //void SpawnEnemy()
    //{
    //    GameObject tmp;
    //    for (int i = 0; i < cloumns; i++)
    //    {
    //        for (int j = 0; j < rows; j++)
    //        {
    //            tmp = Instantiate(enemy, enemyPosition2DArray[i,j], Quaternion.identity, enemyParent.transform);
    //            enemyList.Add(tmp);
    //        }
    //    }
    //    AddEnemyTo2DArray();
    //}
    void SpawnChosenEnemy()
    {
        
        GameObject tmp;
        for (int i = 0; i < chosenEnemySpawnPosList.Count; i++)
        {
            Debug.Log("spawn: "+chosenEnemySpawnPosList[i]);
            tmp = Instantiate(enemy, chosenEnemySpawnPosList[i], Quaternion.identity);
            chosenList.Add(tmp);
        }
    }
    //void AddEnemyTo2DArray()
    //{
    //    int listIndex =0;
    //    for (int i = 0; i < cloumns; i++)
    //    {
    //        for (int j = 0; j < rows; j++)
    //        {
    //            enemy2DArray[i, j] = enemyList[listIndex];
    //            if (listIndex < (cloumns * rows))
    //            {
    //                listIndex++;
    //                //Debug.Log(i+"    "+j+"    "+listIndex);
    //            }
    //        }
    //    }
    //    //Debug.Log(enemy2DArray.Length);
    //}
    public void Add2DArrayToList(int x, int y)
    {
        chosenEnemySpawnPosList.Add(enemyPosition2DArray[x, y]);
        Debug.Log("Array :"+enemyPosition2DArray[x, y]);
    }

    public void ClearChosenList()
    {
        chosenList.Clear();
    }
    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(shootDuration);
        canShoot = false; // Disable shooting
        yield return new WaitForSeconds(shootingCoolDown); // Wait for cooldown
        canShoot = true; // Enable shooting again
    }
}
