using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class testEnemy : MonoBehaviour
{
    [SerializeField]
    GameObject enemy;

    private void Awake()
    {

    }
    public void GetTestSpawn(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SpawnEnemy();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }
    void SpawnEnemy()
    {
        Random.Range(-10, 10);
        Vector3 pos = new Vector3(Random.Range(-10, 10),Random.Range(-10, 10),100);
        Instantiate(enemy, pos, transform.rotation);
    }
}
