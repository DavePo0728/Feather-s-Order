using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashDetect : MonoBehaviour
{

    void Awake()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

        }
    }
}
