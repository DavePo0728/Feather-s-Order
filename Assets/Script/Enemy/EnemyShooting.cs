using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    [SerializeField]
    BulletScreenGenerator bulletScreenGenerator;
    [SerializeField]
    GameObject bullet;

    [SerializeField]
    GameObject originShootingPoint;
    [SerializeField]
    GameObject gun;
    Vector3 origin = new Vector3(-10.5f, 6f, 100);
    [SerializeField]
    private int cloumns; // 列數
    [SerializeField]
    private int rows; // 行數
    [SerializeField]
    private float spacing; // 間隔

    [SerializeField]
    List<Vector3> EnemySpawnPosList;
    [SerializeField]
    List<Vector3> chosenEnemySpawnPosList;
    Vector3[,] enemyPosition2DArray = new Vector3[13, 22];

    [SerializeField]
    List<GameObject> chosenList;
    float shootDuration = 0.1f;
    float shootingCoolDown = 0.5f;
    bool canShoot = false;
    private void Awake()
    {
        
        //bulletParent = this.gameObject;
        GenerateEnemySpawnPos();
        bulletScreenGenerator.GenerateBulletMap();
    }
    void Start()
    {

        SpawnChosenEnemy();
        //SpawnEnemy();
        canShoot = true;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (canShoot)
        {
            StartCoroutine(ShootRoutine());
        }
    }
    void GenerateEnemySpawnPos()
    {
        for (int i = 0; i < cloumns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                
                Vector3 spawnPosition = new Vector3(origin.x + j * spacing, origin.y - i * spacing, transform.position.z);
                enemyPosition2DArray[i, j] = spawnPosition;
                Add2DarrayTolist(i, j);
            }
        }
    }
    void SpawnEnemy()
    {
        GameObject tmp;
        for (int i = 0; i < EnemySpawnPosList.Count; i++)
        {
            tmp = Instantiate(originShootingPoint, EnemySpawnPosList[i], Quaternion.identity,gun.transform);
            tmp.transform.position = EnemySpawnPosList[i];
            chosenList.Add(tmp);
        }
    }
    void SpawnChosenEnemy()
    {

        GameObject tmp;
        for (int i = 0; i < chosenEnemySpawnPosList.Count; i++)
        {
            tmp = Instantiate(originShootingPoint, chosenEnemySpawnPosList[i], Quaternion.identity, gun.transform);
            chosenList.Add(tmp);
        }
    }
    void Add2DarrayTolist(int x, int y)
    {
        EnemySpawnPosList.Add(enemyPosition2DArray[x, y]);
    }
    public void AddChosen2DArrayToList(int x, int y)
    {
        chosenEnemySpawnPosList.Add(enemyPosition2DArray[x, y]);
        //Debug.Log(enemyPosition2DArray[x, y]);
    }
    public void ClearChosenList()
    {
        chosenList.Clear();
    }
    IEnumerator ShootRoutine()
    {
        //canShoot = false; // Disable shooting
        if (chosenList != null)
        {
            foreach (GameObject enemy in chosenList)
            {
                if (enemy != null)
                {
                    Shoot enemysShooting = enemy.GetComponent<Shoot>();
                    enemysShooting.ShootBullet();
                    //enemysShooting.ShootBreakableBullet();
                }
            }
        }
        yield return new WaitForSeconds(shootingCoolDown);// Wait for cooldown
        canShoot = true;// Enable shooting again
    }
}
