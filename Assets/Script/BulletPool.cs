using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{

    public static BulletPool poolInstance;

    [Header("PlayerBulletPool")]
    [SerializeField]
    GameObject playerObjectToPool;
    [SerializeField]
    GameObject playerBulletPoolParent;
    [SerializeField]
    public int playerAmountToPool;
    List<GameObject> playerBulletPool;

    [Header("BlackBulletPool")]
    [SerializeField]
    GameObject blackObjectToPool;
    [SerializeField]
    GameObject blackBulletPoolParent;
    [SerializeField]
    int blackBulletAmountToPool;
    [SerializeField]
    List<GameObject> blackBulletPool;
    [Header("RedBulletPool")]
    [SerializeField]
    GameObject RedBulletToPool;
    [SerializeField]
    GameObject redBulletPoolParent;
    [SerializeField]
    int RedBulletAmountToPool;
    [SerializeField]
    List<GameObject> redBulletPool;
    [Header("PurpleBulletPool")]
    [SerializeField]
    GameObject purpleBulletToPool;
    [SerializeField]
    GameObject purpleBulletPoolParent;
    [SerializeField]
    int PurpleBulletAmountToPool;
    [SerializeField]
    List<GameObject> purpleBulletPool;

    //public List<GameObject> enemyActivePooledObject = new List<GameObject>();
    //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private void Awake()
    {
        poolInstance = this;
        blackBulletPool = new List<GameObject>();
        playerBulletPool = new List<GameObject>();
        redBulletPool = new List<GameObject>();
        purpleBulletPool = new List<GameObject>();
        GameObject tmp, tmp1;
        for (int i = 0; i < blackBulletAmountToPool; i++)
        {
            tmp = Instantiate(blackObjectToPool);
            tmp.SetActive(false);
            tmp.transform.SetParent(blackBulletPoolParent.transform);
            blackBulletPool.Add(tmp);
        }
        for (int i = 0; i < RedBulletAmountToPool; i++)
        {
            tmp = Instantiate(RedBulletToPool);
            tmp.SetActive(false);
            tmp.transform.SetParent(redBulletPoolParent.transform);
            redBulletPool.Add(tmp);
        }
        for (int i = 0; i < PurpleBulletAmountToPool; i++)
        {
            tmp = Instantiate(purpleBulletToPool);
            tmp.SetActive(false);
            tmp.transform.SetParent(purpleBulletPoolParent.transform);
            purpleBulletPool.Add(tmp);
        }
        for (int i = 0; i < playerAmountToPool; i++)
        {
            tmp1 = Instantiate(playerObjectToPool);
            tmp1.SetActive(false);
            tmp1.transform.SetParent(playerBulletPoolParent.transform);
            playerBulletPool.Add(tmp1);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("hi");

    }
    public GameObject GetBulletPoolInstance(BulletType type)
    {
        switch (type)
        {
            case BulletType.Black:
                for (int i = 0; i < blackBulletAmountToPool; i++)
                {
                    if (!blackBulletPool[i].activeInHierarchy)
                    {
                        return blackBulletPool[i];
                    }
                }
                return null;
            case BulletType.Red:
                for (int i = 0; i < RedBulletAmountToPool; i++)
                {
                    if (!redBulletPool[i].activeInHierarchy)
                    {
                        //Debug.Log(i);
                        return redBulletPool[i];
                    }
                }
                return null;
            case BulletType.Purple:
                for (int i = 0; i < PurpleBulletAmountToPool; i++)
                {
                    if (!purpleBulletPool[i].activeInHierarchy)
                    {
                        return purpleBulletPool[i];
                    }
                }
                return null;
            case BulletType.BlackRed:
                for (int i = 0; i < playerAmountToPool; i++)
                {
                    if (!playerBulletPool[i].activeInHierarchy)
                    {
                        return playerBulletPool[i];
                    }
                }
                return null;
            default:
                Debug.LogError("BulletType not found");
                return null;
        }
    }
    public GameObject GetBlackBulletPooledObject()
    {
        for (int i = 0; i < blackBulletAmountToPool; i++)
        {
            if (!blackBulletPool[i].activeInHierarchy)
            {
                return blackBulletPool[i];
            }
        }
        return null;
    }

    public GameObject GetRedBulletPooledObject()
    {
        for (int i = 0; i < RedBulletAmountToPool; i++)
        {
            if (!redBulletPool[i].activeInHierarchy)
            {
                return redBulletPool[i];
            }
        }
        return null;
    }
    public GameObject GetPurpleBulletPooledObject()
    {
        for (int i = 0; i < PurpleBulletAmountToPool; i++)
        {
            if (!purpleBulletPool[i].activeInHierarchy)
            {
                return purpleBulletPool[i];
            }
        }
        return null;
    }
    public GameObject GetPlayerPooledObject()
    {
        for (int i = 0; i < playerAmountToPool; i++)
        {
            if (!playerBulletPool[i].activeInHierarchy)
            {
                return playerBulletPool[i];
            }
        }
        return null;
    }
    public void ClearAllBullet()
    {
        for (int i = 0; i < blackBulletAmountToPool; i++)
        {
            if(blackBulletPool[i].activeInHierarchy)
            {
                blackBulletPool[i].SetActive(false);
            }
        }
        for (int i = 0; i < RedBulletAmountToPool; i++)
        {
            if (redBulletPool[i].activeInHierarchy)
            {
                redBulletPool[i].SetActive(false);
            }
        }
        for (int i = 0; i < PurpleBulletAmountToPool; i++)
        {
            if (purpleBulletPool[i].activeInHierarchy)
            {
                purpleBulletPool[i].SetActive(false);
            }
        }
        for (int i = 0; i < playerAmountToPool; i++)
        {
            if (playerBulletPool[i].activeInHierarchy)
            {
                playerBulletPool[i].SetActive(false);
            }
        }
    }
}
