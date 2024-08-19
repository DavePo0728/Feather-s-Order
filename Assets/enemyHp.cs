using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHp : MonoBehaviour
{
    float Maxhp = 20;
    float currentHp;
    // Start is called before the first frame update
    void Start()
    {
        currentHp = Maxhp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt()
    {
        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            currentHp--;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerBullet")
        {
            Hurt();
        }
    }
}
