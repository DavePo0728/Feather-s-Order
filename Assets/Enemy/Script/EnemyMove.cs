using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    [SerializeField]
    protected float speed;
    [SerializeField]
    protected float rotationSpeed;
    [SerializeField]
    protected float radius;
    protected float angle;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //CircularMovement();
    }
    void FixedUpdate()
    {
        transform.Translate(-Vector3.forward * speed * Time.deltaTime);
    }
}
